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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Xml;
using Gallio.Common.Markup;
using Gallio.Common.Policies;
using Gallio.Reports;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Gallio.Tests.Reports
{
    [TestFixture]
    [TestsOn(typeof(XsltReportFormatter))]
    public class XsltReportFormatterTest : BaseTestWithMocks
    {
        private delegate void SerializeReportDelegate(XmlWriter writer, AttachmentContentDisposition contentDisposition);

        [Test, ExpectedArgumentNullException]
        public void ContentTypeCannotBeNull()
        {
            new XsltReportFormatter("ext", null, new DirectoryInfo("content"), "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ExtensionCannotBeNull()
        {
            new XsltReportFormatter(null, MimeTypes.PlainText, new DirectoryInfo("content"), "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ContentPathCannotBeNull()
        {
            new XsltReportFormatter("ext", MimeTypes.PlainText, null, "xslt", new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void XsltPathCannotBeNull()
        {
            new XsltReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), null, new string[0]);
        }

        [Test, ExpectedArgumentNullException]
        public void ResourcePathsCannotBeNull()
        {
            new XsltReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "xslt", null);
        }

        [Test, ExpectedArgumentNullException]
        public void ResourcePathsCannotContainNulls()
        {
            new XsltReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "xslt", new string[] { null });
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionIsAbsent()
        {
            XsltReportFormatter formatter = new XsltReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "xslt", new string[] { "res1", "res2" });
            Assert.AreEqual(AttachmentContentDisposition.Absent, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void TheDefaultAttachmentContentDispositionCanBeChanged()
        {
            XsltReportFormatter formatter = new XsltReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo("content"), "xslt", new string[] { "res1", "res2" });

            formatter.DefaultAttachmentContentDisposition = AttachmentContentDisposition.Inline;
            Assert.AreEqual(AttachmentContentDisposition.Inline, formatter.DefaultAttachmentContentDisposition);
        }

        [Test]
        public void FormatWritesTheTransformedReport()
        {
            string resourcePath = Path.Combine(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)), @"..\Reports");

            IReportWriter reportWriter = Mocks.StrictMock<IReportWriter>();
            IReportContainer reportContainer = Mocks.StrictMock<IReportContainer>();
            IProgressMonitor progressMonitor = NullProgressMonitor.CreateInstance();

            string reportPath = SpecialPathPolicy.For<XsltReportFormatter>().CreateTempFileWithUniqueName().FullName;
            using (Stream tempFileStream = File.OpenWrite(reportPath))
            {
                using (Mocks.Record())
                {
                    SetupResult.For(reportWriter.ReportContainer).Return(reportContainer);

                    reportWriter.SerializeReport(null, AttachmentContentDisposition.Link);
                    LastCall.Constraints(Is.NotNull(), Is.Equal(AttachmentContentDisposition.Link))
                        .Do((SerializeReportDelegate)delegate(XmlWriter writer, AttachmentContentDisposition contentDisposition)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.InnerXml = "<report>The report.</report>";
                            doc.Save(writer);
                        });

                    SetupResult.For(reportContainer.ReportName).Return("Foo");
                    Expect.Call(reportContainer.OpenWrite("Foo.ext", MimeTypes.PlainText, new UTF8Encoding(false)))
                        .Return(tempFileStream);
                    reportWriter.AddReportDocumentPath("Foo.ext");

                    Expect.Call(reportContainer.OpenWrite(@"Foo\MbUnitLogo.png", MimeTypes.Png, null)).Return(new MemoryStream());

                    reportWriter.SaveReportAttachments(null);
                    LastCall.Constraints(Is.NotNull());
                }

                using (Mocks.Playback())
                {
                    XsltReportFormatter formatter = new XsltReportFormatter("ext", MimeTypes.PlainText, new DirectoryInfo(resourcePath), "Diagnostic.xslt", new string[] { "MbUnitLogo.png" });
                    var reportFormatterOptions = new ReportFormatterOptions();
                    reportFormatterOptions.AddProperty(XsltReportFormatter.AttachmentContentDispositionOption, AttachmentContentDisposition.Link.ToString());

                    formatter.Format(reportWriter, reportFormatterOptions, progressMonitor);

                    string reportContents = File.ReadAllText(reportPath);
                    TestLog.EmbedXml("Diagnostic report contents", reportContents);
                    Assert.Contains(reportContents, "<resourceRoot>Foo</resourceRoot>");
                    Assert.Contains(reportContents, "The report.");

                    File.Delete(reportPath);
                }
            }
        }
    }
}