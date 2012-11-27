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
using System.Reflection;
using System.Collections.Generic;
using Gallio.Model.Helpers;
using Gallio.Runtime.Hosting;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.MSTestAdapter.Model
{
    /// <summary>
    /// Test driver for MSTest.
    /// </summary>
    internal class MSTestDriver : SimpleTestDriver
    {
        protected override string FrameworkName
        {
            get { return "MSTest"; }
        }

        protected override TestExplorer CreateTestExplorer()
        {
            return new MSTestExplorer();
        }

        protected override TestController CreateTestController()
        {
            return new DelegatingTestController(test =>
            {
                var topTest = test as MSTestAssembly;
                return topTest != null ? MSTestController.CreateController(topTest.FrameworkVersion) : null;
            });
        }

        protected override void ConfigureHostSetup(HostSetup hostSetup, TestPackage testPackage,
            string assemblyPath, AssemblyMetadata assemblyMetadata)
        {
            base.ConfigureHostSetup(hostSetup, testPackage, assemblyPath, assemblyMetadata);

            if (hostSetup.ProcessorArchitecture == ProcessorArchitecture.Amd64
                || hostSetup.ProcessorArchitecture == ProcessorArchitecture.IA64)
                throw new ModelException("Cannot run MSTest tests compiled for 64bit because MSTest.exe throws exceptions when run without isolation as a 64bit process.");

            hostSetup.ProcessorArchitecture = ProcessorArchitecture.X86;
        }
    }
}