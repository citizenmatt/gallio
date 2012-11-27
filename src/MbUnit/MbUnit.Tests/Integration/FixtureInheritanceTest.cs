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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.TestResources.Fixtures;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [TestFixture]
    [RunSample(typeof(FixtureInheritanceSample))]
    [RunSample(typeof(DerivedFixture))]
    [RunSample(typeof(FixtureInheritanceSample.NestedFixture))]
    public class FixtureInheritanceTest : BaseTestWithSampleRunner
    {
        [Test]
        public void BaseTestOnBaseFixture()
        {
            AssertOutput("BaseTestFixtureSetUp\nBaseTestFixtureTearDown\n",
                typeof(FixtureInheritanceSample), null);
            AssertOutput("BaseSetUp\nBaseTest\nBaseTearDown\n",
                typeof(FixtureInheritanceSample), "BaseTest");
        }

        [Test]
        public void BaseTestOnDerivedFixtureIncludesBaseFixtureContributionsFirst()
        {
            AssertOutput("BaseTestFixtureSetUp\nDerivedTestFixtureSetUp\nDerivedTestFixtureTearDown\nBaseTestFixtureTearDown\n",
                typeof(DerivedFixture), null);
            AssertOutput("BaseSetUp\nDerivedSetUp\nBaseTest\nDerivedTearDown\nBaseTearDown\n",
                typeof(DerivedFixture), "BaseTest");
        }

        [Test]
        public void DerivedTestOnDerivedFixtureIncludesBaseFixtureContributionsFirst()
        {
            AssertOutput("BaseTestFixtureSetUp\nDerivedTestFixtureSetUp\nDerivedTestFixtureTearDown\nBaseTestFixtureTearDown\n",
                typeof(DerivedFixture), null);
            AssertOutput("BaseSetUp\nDerivedSetUp\nDerivedTest\nDerivedTearDown\nBaseTearDown\n",
                typeof(DerivedFixture), "DerivedTest");
        }

        [Test]
        public void NestedTestRunsInTheContextOfTheDeclaringTest()
        {
            AssertOutput("BaseSetUp\nNestedTestFixtureSetUp\nNestedTestFixtureTearDown\nBaseTearDown\n",
                typeof(DerivedFixture.NestedFixture), null);
            AssertOutput("NestedTest\n",
                typeof(DerivedFixture.NestedFixture), "NestedTest");
        }

        private void AssertOutput(string expectedOutput, Type fixtureType, string methodName)
        {
            CodeReference codeReference = methodName != null
                ? CodeReference.CreateFromMember(fixtureType.GetMethod(methodName))
                : CodeReference.CreateFromType(fixtureType);

            TestStepRun testStepRun = Runner.GetPrimaryTestStepRun(codeReference);
            Assert.AreEqual(TestStatus.Passed, testStepRun.Result.Outcome.Status);

            string actualOutput = testStepRun.TestLog.GetStream(MarkupStreamNames.ConsoleOutput).ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}