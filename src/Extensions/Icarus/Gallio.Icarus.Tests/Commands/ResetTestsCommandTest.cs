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

using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Tests.Utilities;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ResetTestsCommand))]
    public class ResetTestsCommandTest
    {
        [Test]
        public void Execute_should_reset_test_statuses()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var resetTestsCommand = new ResetTestsCommand(testController);
            var progressMonitor = MockProgressMonitor.Instance;

            resetTestsCommand.Execute(progressMonitor);

            testController.AssertWasCalled(tc => tc.ResetTestStatus(progressMonitor));
        }
    }
}
