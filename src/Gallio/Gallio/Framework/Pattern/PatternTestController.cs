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
using System.Diagnostics;
using Gallio.Model.Commands;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.Model.Environments;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Formatting;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Controls the execution of <see cref="PatternTest" /> instances.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestController : TestController
    {
        private readonly IConverter converter;
        private readonly IFormatter formatter;
        private readonly ITestEnvironmentManager environmentManager;

        /// <summary>
        /// Creates a pattern test controller.
        /// </summary>
        /// <param name="formatter">The formatter for data binding.</param>
        /// <param name="converter">The converter for data binding.</param>
        /// <param name="environmentManager">The test environment manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/>,
        /// <paramref name="converter"/> or <paramref name="environmentManager"/> is null.</exception>
        public PatternTestController(IFormatter formatter, IConverter converter, ITestEnvironmentManager environmentManager)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            if (converter == null)
                throw new ArgumentNullException("converter");
            if (environmentManager == null)
                throw new ArgumentNullException("environmentManager");

            this.formatter = formatter;
            this.converter = converter;
            this.environmentManager = environmentManager;
        }

        /// <inheritdoc />
        [DebuggerNonUserCode]
        protected internal override TestResult RunImpl(ITestCommand rootTestCommand, Model.Tree.TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount))
            {
                // Note: We do not check options.SkipTestExecution here because we want to build up
                // the tree of data-driven test steps.  So we actually check it later on in the
                // PatternTestExecutor.  This is different from framework adapters
                // at this time (because they do not generally support dynamically generated data-driven tests).
                Sandbox sandbox = new Sandbox();
                EventHandler canceledHandler = delegate { sandbox.Abort(TestOutcome.Canceled, "The user canceled the test run."); };
                try
                {
                    progressMonitor.Canceled += canceledHandler;

                    TestAssemblyExecutionParameters.Reset();

                    PatternTestExecutor executor = new PatternTestExecutor(options, progressMonitor, formatter, converter, environmentManager);

                    // Inlined to minimize stack depth.
                    var action = executor.CreateActionToRunTest(rootTestCommand, parentTestStep, sandbox, null);
                    action.Run();
                    return action.Result;
                }
                finally
                {
                    progressMonitor.Canceled -= canceledHandler;
                    sandbox.Dispose();
                }
            }
        }
    }
}