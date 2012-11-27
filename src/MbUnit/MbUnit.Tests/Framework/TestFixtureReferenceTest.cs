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
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model.Schema;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(TestFixtureReference))]
    //[RunSample(typeof(DynamicSample))]
    [RunSample(typeof(StaticSample))]
    public class TestFixtureReferenceTest : BaseTestWithSampleRunner
    {
        [Test]
        public void ConstructorRequiresFixtureType()
        {
            Assert.Throws<ArgumentNullException>(() => new TestFixtureReference(null));

            TestFixtureReference reference = new TestFixtureReference(typeof(TestFixtureReferenceTest));
            Assert.AreEqual(typeof(TestFixtureReferenceTest), reference.TestFixtureType);
        }

        [Test, Pending("Dynamic references to test fixtures not supported yet.")]
        public void DynamicRun()
        {
        }

        [Test]
        public void StaticRun()
        {
            TestData fixtureTest = Runner.GetTestData(CodeReference.CreateFromType(typeof(StaticSample)));
            Assert.Count(1, fixtureTest.Children);

            TestData referenceData = fixtureTest.Children[0];
            Assert.AreEqual("ReferencedFixture", referenceData.Name);
            Assert.IsFalse(referenceData.IsTestCase);
            Assert.Count(1, referenceData.Children);

            TestData testData = referenceData.Children[0];
            Assert.AreEqual("Test", testData.Name);

            TestStepRun fixtureRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromType(typeof(StaticSample)));
            Assert.Count(1, fixtureRun.Children);

            TestStepRun suiteRun = fixtureRun.Children[0];
            Assert.AreEqual("ReferencedFixture", suiteRun.Step.Name);
            Assert.IsFalse(suiteRun.Step.IsDynamic);
            Assert.IsTrue(suiteRun.Step.IsPrimary);
            Assert.IsFalse(suiteRun.Step.IsTestCase);
            Assert.AreEqual("", suiteRun.TestLog.ToString());
            Assert.Count(1, suiteRun.Children);

            TestStepRun testRun = suiteRun.Children[0];
            Assert.AreEqual("Test", testRun.Step.Name);
            Assert.AreEqual("*** Log ***\n\nExecute\n", testRun.TestLog.ToString());
        }

        private static readonly Test[] tests = new Test[]
        {
            new TestFixtureReference(typeof(ReferencedFixture))
        };

        [Explicit("Sample")]
        public class DynamicSample
        {
            [DynamicTestFactory]
            public IEnumerable<Test> Factory()
            {
                return tests;
            }
        }

        [Explicit("Sample")]
        public class StaticSample
        {
            [StaticTestFactory]
            public static IEnumerable<Test> Factory()
            {
                return tests;
            }
        }

        public class ReferencedFixture
        {
            [Test]
            public void Test()
            {
                TestLog.WriteLine("Execute");
            }
        }
    }
}
