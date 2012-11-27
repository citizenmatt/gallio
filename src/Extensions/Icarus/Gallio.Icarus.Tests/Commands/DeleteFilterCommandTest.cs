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
using Gallio.Model.Filters;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(DeleteFilterCommand))]
    internal class DeleteFilterCommandTest
    {
        [Test]
        public void Execute_should_call_DeleteFilter_on_ProjectController()
        {
            var projectController = MockRepository.GenerateStub<IProjectController>();
            var filterExpr = new NoneFilter<ITestDescriptor>().ToFilterExpr();
            var filterInfo = new FilterInfo("None", filterExpr);
            var command = new DeleteFilterCommand(projectController)
            {
                FilterInfo = filterInfo
            };

            command.Execute(MockProgressMonitor.Instance);

            projectController.AssertWasCalled(pc => pc.DeleteFilter(Arg<IProgressMonitor>.Is.Anything, 
                Arg.Is(filterInfo)));
        }
    }
}
