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
using Gallio.Framework;
using MbUnit.Framework;

namespace MbUnit.TestResources.Fixtures
{
    /// <summary>
    /// A sample test fixture with failing setup.
    /// </summary>
    [TestFixture]
    public class ContextOutcomeFailingSetUpSample
    {
        private TestContext previousContext;

        [SetUp]
        public void SetUp()
        {
            previousContext = TestContext.CurrentContext;
            TestLog.WriteLine(TestContext.CurrentContext.Outcome);
            Assert.Fail("Boom");
        }

        [Test]
        public void Test()
        {
            Assert.Fail("Should not get here because the setup failed.");
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.WriteLine(TestContext.CurrentContext.Outcome);
        }

        [FixtureTearDown]
        public void TestFixtureTearDown()
        {
            TestLog.WriteLine(previousContext.Outcome);
        }
    }
}
