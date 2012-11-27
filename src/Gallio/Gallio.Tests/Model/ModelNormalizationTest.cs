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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;

namespace Gallio.Tests.Model
{
    // This test verifies that the contents of the model and its output are normalized
    // by the test runner before being incorporated into the test report.  Normalization
    // is important to ensure that invalid (non-XML encodable or unprintable)
    // characters do not end up in the report even if they happen to be emitted by the
    // test.  Without normalization, test report generation may fail due to encoding
    // errors or reports may contain garbage.
    [RunSample(typeof(ModelNormalizationSample))]
    public class ModelNormalizationTest : BaseTestWithSampleRunner
    {
        [Test]
        public void TestDataShouldBeNormalized()
        {
            TestData testData = Runner.GetTestData(CodeReference.CreateFromMember(typeof(ModelNormalizationSample).GetMethod("Test")));

            Assert.AreEqual("Test *?* Name", testData.Name, "Name should be normalized.");
            Assert.AreEqual("Value *?*", testData.Metadata.GetValue("Key *?*"), "Metadata should be normalized.");
        }

        [Test]
        public void TestStepRunShouldBeNormalized()
        {
            TestStepRun testStepRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ModelNormalizationSample).GetMethod("Test")));

            Assert.AreEqual("Test *?* Name", testStepRun.Step.Name, "Name should be normalized.");
            Assert.AreEqual("Value *?*", testStepRun.Step.Metadata.GetValue("Key *?*"), "Metadata should be normalized.");
            AssertLogContains(testStepRun, "Section *?*");
            AssertLogContains(testStepRun, "Text *?* Content");
        }

        [Test]
        public void AnnotationShouldBeNormalized()
        {
            AnnotationData annotation = Report.TestModel.Annotations.Find(x => x.CodeReference == CodeReference.CreateFromMember(typeof(ModelNormalizationSample).GetMethod("Test")));

            Assert.AreEqual("Annotation message with invalid characters *?*.", annotation.Message, "Annotation message should be normalized.");
            Assert.AreEqual("Annotation details with invalid characters *?*.", annotation.Details, "Annotation details should be normalized.");
        }

        [Test]
        public void DiagnosticLogShouldBeNormalized()
        {
            LogEntry logEntry = Report.LogEntries.Find(entry => entry.Message.Contains("Diagnostic"));

            Assert.AreEqual("Diagnostic *?* Log", logEntry.Message, "Diagnostic log message should be normalized.");
        }

        [Explicit("Sample")]
        public class ModelNormalizationSample
        {
            [Test]
            [Name("Test *\n* Name")]
            [Metadata("Key *\0*", "Value *\0*")]
            [Annotation(AnnotationType.Info, "Annotation message with invalid characters *\0*.",
                Details = "Annotation details with invalid characters *\0*.")]
            public void Test()
            {
                using (TestLog.BeginSection("Section *\n*"))
                {
                    TestLog.WriteLine("Text *\0* Content");
                }

                DiagnosticLog.WriteLine("Diagnostic *\0* Log");
            }
        }
    }
}
