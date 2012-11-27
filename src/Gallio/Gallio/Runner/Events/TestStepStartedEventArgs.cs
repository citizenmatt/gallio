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
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test step has started execution.
    /// </summary>
    public sealed class TestStepStartedEventArgs : TestStepEventArgs
    {
        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="test">The test data.</param>
        /// <param name="testStepRun">The test step run.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>, <paramref name="test"/>
        /// or <paramref name="testStepRun"/> is null.</exception>
        public TestStepStartedEventArgs(Report report, TestData test, TestStepRun testStepRun)
            : base(report, test, testStepRun)
        {
        }
    }
}
