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

#pragma warning disable 618

namespace MbUnit.Compatibility.Tests.Framework.Xml {
	using MbUnit.Framework;	
	using MbUnit.Framework.Xml;
	using System.IO;
    using System.Xml;
    using System.Xml.Schema;
    


    [TestFixture]
    public class DiffConfigurationTests 
	{
        private static string xmlWithWhitespace = "<elemA>as if<elemB> \r\n </elemB>\t</elemA>";
        private static string xmlWithoutWhitespaceElement = "<elemA>as if<elemB/>\r\n</elemA>";
        private static string xmlWithWhitespaceElement = "<elemA>as if<elemB> </elemB></elemA>";
        private static string xmlWithoutWhitespace = "<elemA>as if<elemB/></elemA>";
                
        [Test] public void DefaultConfiguredWithGenericDescription() {
            DiffConfiguration diffConfiguration = new DiffConfiguration();
            OldAssert.AreEqual(DiffConfiguration.DEFAULT_DESCRIPTION, 
                                   diffConfiguration.Description);
            
            OldAssert.AreEqual(DiffConfiguration.DEFAULT_DESCRIPTION, 
                                   new XmlDiff("", "").OptionalDescription);
        }
        
        //[Test] public void DefaultConfiguredToUseValidatingParser() {
        //    DiffConfiguration diffConfiguration = new DiffConfiguration();
        //    OldAssert.AreEqual(DiffConfiguration.DEFAULT_USE_VALIDATING_PARSER, 
        //                           diffConfiguration.UseValidatingParser);
            
        //    Stream controlFileStream = ValidatorTests.ValidFile;
        //    Stream testFileStream = ValidatorTests.InvalidFile;
        //    try {         
        //        XmlDiff diff = new XmlDiff(new StreamReader(controlFileStream), 
        //                                   new StreamReader(testFileStream));
        //        diff.Compare();
        //        OldAssert.Fail("Expected validation failure");
        //    } catch (XmlSchemaException e) {
        //        string message = e.Message; // to prevent 'unused variable' compiler warning 
        //    } finally {
        //        controlFileStream.Close();
        //        testFileStream.Close();
        //    }
        //}
                
        //[Test] public void CanConfigureNotToUseValidatingParser() {
        //    DiffConfiguration diffConfiguration = new DiffConfiguration(false);
        //    OldAssert.AreEqual(false, diffConfiguration.UseValidatingParser);
            
        //    Stream controlFileStream = ValidatorTests.ValidFile;
        //    Stream testFileStream = ValidatorTests.InvalidFile;
        //    try {         
        //        XmlDiff diff = new XmlDiff(new XmlInput(controlFileStream), 
        //                                   new XmlInput(testFileStream),
        //                                   diffConfiguration);
        //        diff.Compare();
        //    } catch (XmlSchemaException e) {
        //        OldAssert.Fail("Unexpected validation failure: " + e.Message);
        //    } finally {
        //        controlFileStream.Close();
        //        testFileStream.Close();
        //    }
        //}
        
        //[Test] public void DefaultConfiguredWithWhitespaceHandlingAll() {
        //    DiffConfiguration diffConfiguration = new DiffConfiguration();
        //    OldAssert.AreEqual(WhitespaceHandling.All, diffConfiguration.WhitespaceHandling);
            
        //    PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespaceElement, false);
        //    PerformAssertion(xmlWithoutWhitespace, xmlWithoutWhitespaceElement, false);
        //    PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespace, false);
        //    PerformAssertion(xmlWithoutWhitespaceElement, xmlWithWhitespaceElement, false);
        //}
        
        private void PerformAssertion(string control, string test, bool assertion) {
            XmlDiff diff = new XmlDiff(control, test);
            PerformAssertion(diff, assertion);
        }
        private void PerformAssertion(string control, string test, bool assertion, 
                                      DiffConfiguration xmlUnitConfiguration) {
            XmlDiff diff = new XmlDiff(new XmlInput(control), new XmlInput(test), 
                                       xmlUnitConfiguration);
            PerformAssertion(diff, assertion);
        }        
        private void PerformAssertion(XmlDiff diff, bool assertion) {
            OldAssert.AreEqual(assertion, diff.Compare().Equal);            
            OldAssert.AreEqual(assertion, diff.Compare().Identical);            
        }

        [Test] public void CanConfigureWhitespaceHandlingSignificant() {
            DiffConfiguration xmlUnitConfiguration = 
                new DiffConfiguration (WhitespaceHandling.Significant);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithoutWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespace, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespaceElement, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
        }
        
        [Test] public void CanConfigureWhitespaceHandlingNone() {
            DiffConfiguration xmlUnitConfiguration = 
                new DiffConfiguration(WhitespaceHandling.None);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithoutWhitespaceElement, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespace, xmlWithWhitespace, 
                             true, xmlUnitConfiguration);
            PerformAssertion(xmlWithoutWhitespaceElement, xmlWithWhitespaceElement, 
                             true, xmlUnitConfiguration);
        }        
    }
}
