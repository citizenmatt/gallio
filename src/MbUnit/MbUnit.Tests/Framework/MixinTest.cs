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
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

#if false // pending implementation

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(MixinAttribute))]
    public class MixinTest : BaseTestWithSampleRunner
    {
        [Test]
        public void VerifySampleOutput(Type fixtureType, string sampleName, string[] output)
        {
            IList<TestStepRun> runs = Runner.GetTestCaseRunsWithin(
                CodeReference.CreateFromMember(fixtureType.GetMethod(sampleName)));

            Assert.AreEqual(output.Length, runs.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
                AssertLogContains(runs[i], output[i]);
        }
    }
}
#endif