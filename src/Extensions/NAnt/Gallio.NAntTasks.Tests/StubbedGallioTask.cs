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
using Gallio.NAntTasks;
using NAnt.Core;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.NAntTasks.Tests
{
    /// <summary>
    /// Makes possible to unit test the <see cref="GallioTask" /> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The NAnt.Core.Task class is hard to unit test for a number of reasons. The
    /// main is that the Execute() method uses a Logger and a Project member. The
    /// Project property therefore must be assigned, but Project class itself
    /// requires a lot of stuff to be instantiated, and since it's a concrete class
    /// is hard to be mocked. This class therefore exposes the protected ExecuteTask
    /// method that don't use those properties.
    /// </para>
    /// <para>
    /// Also, for some reason the the Log method of the task will fail with a
    /// NullReference exception in the IsLogEnabledFor(Level messageLevel) method,
    /// so we need to avoid calling the instance Log methods directly and use a
    /// interface too.
    /// </para>
    /// </remarks>
    internal class StubbedGallioTask : GallioTask
    {
        public delegate TestLauncherResult RunLauncherDelegate(TestLauncher launcher);
        private RunLauncherDelegate action;

        // We need to instantiate our own dictionary of properties
        // WARNING: This could break in the future if a check for null is added
        // in the PropertyDictionary constructor
        private readonly PropertyDictionary properties = new PropertyDictionary(null);

        public StubbedGallioTask()
        {
            InitializeTaskConfiguration();
        }

        public override PropertyDictionary Properties
        {
            get { return properties; }
        }

        public void SetRunLauncherAction(RunLauncherDelegate action)
        {
            this.action = action;
        }

        protected override TestLauncherResult RunLauncher(TestLauncher launcher)
        {
            Assert.IsNotNull(action, "The run launcher method should not have been called because no action was set.");
            return action(launcher);
        }

        public override void Log(Level messageLevel, string message)
        {
            // Stubbed out.
        }

        public override void Log(Level messageLevel, string message, params object[] args)
        {
            // Stubbed out.
        }
    }
}
