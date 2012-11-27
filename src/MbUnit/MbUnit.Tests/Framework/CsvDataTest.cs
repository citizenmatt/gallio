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
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(CsvDataAttribute))]
    [RunSample(typeof(CsvDataSample))]
    public class CsvDataTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row("Inline", new[] { "(abc, 123)", "(def, 456)" })]
        [Row("InlineWithNonstandardSettings", new[] { "(abc, 123)", "(def, 456)" })]
        [Row("ImplicitlyScopedResourceWithHeader", new[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("ExplicitlyScopedResourceWithHeader", new[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("AbsolutelyScopedResourceWithHeader", new[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("FileWithHeader", new[] { "Apples: 1.00", "Bananas: 1.50", "Cookies: 2.00" })]
        [Row("FileWithEmptyCellsOnStringTypes", new[] { "Sirius: <1.42> <9>", "Betelgeuse: <> <640>", "Rigel: <-6.7> <>" })]
        [Row("FileWithEmptyCellsOnNullableTypes", new[] { "Sirius: <1.42> <9>", "Betelgeuse: <null> <640>", "Rigel: <-6.7> <null>" })]
        public void VerifySampleOutput(string sampleName, string[] output)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(CsvDataSample).GetMethod(sampleName)));
            Assert.AreEqual(output.Length, run.Children.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
            {
                AssertLogContains(run.Children[i], output[i]);
                Assert.IsNotNull(run.Children[i].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            }
        }

        [Test]
        [Row("FileErrorGetFailedTestByDefault", TestStatus.Failed)]
        [Row("FileErrorGetInconclusiveTest", TestStatus.Inconclusive)]
        public void OnFileError(string sampleName, TestStatus expectedTestStatus)
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(CsvDataSample).GetMethod(sampleName)));
            Assert.AreEqual(expectedTestStatus, run.Result.Outcome.Status);
        }

        [Test]
        public void Metadata()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(CsvDataSample).GetMethod("FileWithHeader")));
            Assert.AreEqual(@"..\Framework\CsvDataTest.csv(2)", run.Children[0].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual(@"Worm", run.Children[0].Step.Metadata.GetValue("ConsumedBy"));
            Assert.AreEqual(@"..\Framework\CsvDataTest.csv(3)", run.Children[1].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual(@"Monkey", run.Children[1].Step.Metadata.GetValue("ConsumedBy"));
            Assert.AreEqual(@"..\Framework\CsvDataTest.csv(4)", run.Children[2].Step.Metadata.GetValue(MetadataKeys.DataLocation));
            Assert.AreEqual(@"Cookie Monster", run.Children[2].Step.Metadata.GetValue("ConsumedBy"));
        }

        [TestFixture, Explicit("Sample")]
        internal class CsvDataSample
        {
            [Test]
            [CsvData(Contents = "abc,123\ndef,456")]
            public void Inline(string x, int y)
            {
                TestLog.WriteLine("({0}, {1})", x, y);
            }

            [Test]
            [CsvData(Contents = "abc|123\n;Comment\ndef|456", FieldDelimiter = '|', CommentPrefix = ';')]
            public void InlineWithNonstandardSettings(string x, int y)
            {
                TestLog.WriteLine("({0}, {1})", x, y);
            }

            [Test]
            [CsvData(ResourcePath = "CsvDataTest.csv", HasHeader = true)]
            public void ImplicitlyScopedResourceWithHeader(decimal price, string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [CsvData(ResourceScope = typeof(CsvDataTest), ResourcePath = "CsvDataTest.csv", HasHeader = true)]
            public void ExplicitlyScopedResourceWithHeader(decimal price, string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [CsvData(ResourcePath = @"MbUnit.Tests\Framework\CsvDataTest.csv", HasHeader = true)]
            public void AbsolutelyScopedResourceWithHeader(decimal price, string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [CsvData(FilePath = @"..\Framework\CsvDataTest.csv", HasHeader = true)]
            public void FileWithHeader(decimal price, string item)
            {
                TestLog.WriteLine("{0}: {1}", item, price);
            }

            [Test]
            [CsvData(FilePath = @"..\Framework\CsvDataWithEmptyCellsTest.csv", HasHeader = true)]
            public void FileWithEmptyCellsOnStringTypes(string star, string magnitude, string distance)
            {
                TestLog.WriteLine("{0}: <{1}> <{2}>", star, magnitude ?? "null", distance ?? "null");
            }

            [Test]
            [CsvData(FilePath = @"..\Framework\CsvDataWithEmptyCellsTest.csv", HasHeader = true)]
            public void FileWithEmptyCellsOnNullableTypes(string star, double? magnitude, int? distance)
            {
                TestLog.WriteLine("{0}: <{1}> <{2}>", star, 
                    magnitude.HasValue ? magnitude.ToString() : "null",
                    distance.HasValue ? distance.ToString() : "null");
            }

            [Test]
            [CsvData(FilePath = @"..\Framework\FileDoesNotExist.csv", OutcomeOnFileError = OutcomeOnFileError.Inconclusive)]
            public void FileErrorGetInconclusiveTest(string x, string y)
            {
            }

            [Test]
            [CsvData(FilePath = @"..\Framework\FileDoesNotExist.csv")]
            public void FileErrorGetFailedTestByDefault(string x, string y)
            {
            }
        }
    }
}
