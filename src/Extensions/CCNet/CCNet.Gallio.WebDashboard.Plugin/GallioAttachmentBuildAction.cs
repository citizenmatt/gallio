// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace CCNet.Gallio.WebDashboard.Plugin
{
    [ReflectorType("gallioAttachmentBuildAction")]
    public class GallioAttachmentBuildAction : ICruiseAction, IConditionalGetFingerprintProvider
    {
        private const string NamespaceUri = "http://www.gallio.org/";
        private readonly IBuildRetriever buildRetriever;
        private readonly IFingerprintFactory fingerprintFactory;

        public GallioAttachmentBuildAction(IBuildRetriever buildRetriever, IFingerprintFactory fingerprintFactory)
        {
            this.buildRetriever = buildRetriever;
            this.fingerprintFactory = fingerprintFactory;
        }

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string stepId = cruiseRequest.Request.GetText(@"testStepId");
            
            if (stepId.Length == 0)
                throw new InvalidOperationException("Missing test step id.");

            string attachmentName = cruiseRequest.Request.GetText(@"attachmentName");
            
            if (attachmentName.Length == 0)
                throw new InvalidOperationException("Missing attachment name.");

            Build build = buildRetriever.GetBuild(cruiseRequest.BuildSpecifier, cruiseRequest.RetrieveSessionToken());
            var document = new XPathDocument(new StringReader(build.Log));
            XPathNavigator rootNavigator = document.CreateNavigator();
            var nsmgr = new XmlNamespaceManager(rootNavigator.NameTable);
            nsmgr.AddNamespace(@"g", NamespaceUri);
            XPathNavigator testStepNavigator = FindTestStepNode(rootNavigator, nsmgr, stepId);
            XPathNavigator attachmentNavigator = FindAttachmentNode(testStepNavigator, nsmgr, attachmentName);
            return CreateResponseFromAttachment(attachmentNavigator);
        }

        public ConditionalGetFingerprint GetFingerprint(IRequest request)
        {
            return fingerprintFactory.BuildFromDate(DateTime.MinValue);
        }

        private static XPathNavigator FindTestStepNode(XPathNavigator rootNavigator, IXmlNamespaceResolver resolver, string stepId)
        {
            foreach (XPathNavigator testStepNavigator in rootNavigator.Select(@"//g:report/g:testPackageRun/descendant::g:testStepRun/g:testStep", resolver))
            {
                if (testStepNavigator.GetAttribute(@"id", "") == stepId)
                    return testStepNavigator;
            }

            throw new InvalidOperationException("The step id is not valid.");
        }

        private static XPathNavigator FindAttachmentNode(XPathNavigator testStepNavigator, IXmlNamespaceResolver resolver, string attachmentName)
        {
            foreach (XPathNavigator attachmentNavigator in testStepNavigator.Select(@"../g:testLog/g:attachments/g:attachment", resolver))
            {
                if (attachmentNavigator.GetAttribute(@"name", "") == attachmentName)
                    return attachmentNavigator;
            }

            throw new InvalidOperationException("The attachment name is not valid.");
        }

        private static IResponse CreateResponseFromAttachment(XPathNavigator attachmentNavigator)
        {
            string contentDisposition = attachmentNavigator.GetAttribute(@"contentDisposition", "");
            
            if (contentDisposition != @"inline")
                throw new InvalidOperationException("The attachment was not inlined into the XML report.");

            string contentType = attachmentNavigator.GetAttribute(@"contentType", "");
            string type = attachmentNavigator.GetAttribute(@"type", "");
            string encodedContent = attachmentNavigator.Value;
            byte[] content;

            if (type == @"binary")
            {
                content = Convert.FromBase64String(encodedContent);
            }
            else
            {
                content = Encoding.UTF8.GetBytes(encodedContent);
                contentType += @"; charset=utf-8";
            }

            return new TypedBinaryResponse(content, contentType);
        }
    }
}
