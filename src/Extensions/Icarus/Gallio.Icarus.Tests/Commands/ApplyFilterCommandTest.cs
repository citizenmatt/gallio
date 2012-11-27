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
using Gallio.Icarus.Services;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model.Filters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [Category("Commands"), TestsOn(typeof(ApplyFilterCommand))]
    internal class ApplyFilterCommandTest
    {
        [Test]
        public void Execute_should_call_ApplyFilterSet_on_TestController()
        {
            var filterService = MockRepository.GenerateStub<IFilterService>();
            var filterSet = new FilterSet<ITestDescriptor>(new NoneFilter<ITestDescriptor>());
            var cmd = new ApplyFilterCommand(filterService)
            {
                FilterSet = filterSet
            };

            cmd.Execute(MockProgressMonitor.Instance);

            filterService.AssertWasCalled(tc => tc.ApplyFilterSet(filterSet));
        }
    }
}
