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
using System.Linq;
using System.Collections.Generic;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ComparerAttribute))]
    [RunSample(typeof(NonComparableStubSample))]
    public class ComparerAttributeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void Run()
        {
            TestStepRun testRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(NonComparableStubSample).GetMethod("Test")));
            Assert.AreEqual(TestOutcome.Passed, testRun.Result.Outcome);
            AssertLogContains(testRun, "CustomComparer: x = 123, y = 456");
            AssertLogContains(testRun, "CustomComparer: x = 456, y = 123");
        }

        public class NonComparableStub
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public NonComparableStub(int value)
            {
                this.value = value;
            }
        }

        [Explicit("Sample")]
        internal class NonComparableStubSample
        {
            [Test]
            public void Test()
            {
                var stub1 = new NonComparableStub(123);
                var stub2 = new NonComparableStub(456);
                Assert.GreaterThan(stub2, stub1);
                Assert.LessThan(stub1, stub2);
            }
        }
    }
}
