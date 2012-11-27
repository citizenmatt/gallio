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

using Gallio.Runtime;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner
{
    /// <summary>
    /// A test tunner factory is a service that creates an <see cref="ITestRunner" />
    /// given a set of options.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each factory has a name which is used by <see cref="ITestRunnerManager" /> to select 
    /// the particular factory to use for a given test run.
    /// </para>
    /// </remarks>
    [Traits(typeof(TestRunnerFactoryTraits))]
    public interface ITestRunnerFactory
    {
        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <returns>The test runner.</returns>
        ITestRunner CreateTestRunner();
    }
}
