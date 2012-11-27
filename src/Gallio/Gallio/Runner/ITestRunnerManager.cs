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
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner
{
    /// <summary>
    /// A test runner manager enumerates the names of the
    /// <see cref="ITestRunnerFactory" /> services that are available and
    /// provides a mechanism for creating <see cref="ITestRunner" /> instances.
    /// </summary>
    public interface ITestRunnerManager
    {
        /// <summary>
        /// Gets handles for all registered test runner factories.
        /// </summary>
        IList<ComponentHandle<ITestRunnerFactory, TestRunnerFactoryTraits>> TestRunnerFactoryHandles { get; }

        /// <summary>
        /// Gets the factory by name, or null if none.
        /// </summary>
        /// <param name="factoryName">The name of the test runner factory, matched case-insensitively.</param>
        /// <returns>The test runner.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryName"/> is null.</exception>
        ITestRunnerFactory GetFactory(string factoryName);

        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <param name="factoryName">The name of the test runner factory, matched case-insensitively.</param>
        /// <returns>The test runner.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryName"/> is null.</exception>
        ITestRunner CreateTestRunner(string factoryName);
    }
}
