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

using Gallio.Runner;
using Gallio.TDNetRunner.Core;
using MbUnit.Framework;

namespace Gallio.TDNetRunner.Tests
{
    /// <summary>
    /// Makes it possible to stub out the test execution portion of the test runner.
    /// </summary>
    internal class StubbedLocalTestRunner : LocalProxyTestRunner
    {
        public delegate TestLauncherResult RunLauncherDelegate(TestLauncher launcher);
        private RunLauncherDelegate action;

        public void SetRunLauncherAction(RunLauncherDelegate action)
        {
            this.action = action;
        }

        internal override IProxyTestRunner CreateRemoteProxyTestRunner()
        {
            return new StubbedRemoteProxyTestRunner() { Action = action };
        }

        private class StubbedRemoteProxyTestRunner : RemoteProxyTestRunner
        {
            public RunLauncherDelegate Action { get; set; }

            public StubbedRemoteProxyTestRunner()
            {
            }

            internal override TestLauncherResult RunLauncher(TestLauncher launcher)
            {
                Assert.IsNotNull(Action, "The run launcher method should not have been called because no action was set.");
                return Action(launcher);
            }
        }
    }
}
