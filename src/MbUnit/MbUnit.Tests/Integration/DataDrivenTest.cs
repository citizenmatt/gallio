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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [RunSample(typeof(NoDataSample))]
    public class DataDrivenTest : BaseTestWithSampleRunner
    {
        [Test]
        public void WhenNoData_MarksTestAsSkipped()
        {
            var run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(NoDataSample).GetMethod("NoData")));

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(run);
                Assert.Count(0, run.Children);

                Assert.AreEqual(TestOutcome.Skipped, run.Result.Outcome);
                AssertLogContains(run, "Test skipped because it is parameterized but no data was provided.", MarkupStreamNames.Warnings);
            });
        }

        [Explicit("Sample")]
        public class NoDataSample
        {
            [Test]
            public void NoData(int dummy)
            {
            }
        }
    }
}