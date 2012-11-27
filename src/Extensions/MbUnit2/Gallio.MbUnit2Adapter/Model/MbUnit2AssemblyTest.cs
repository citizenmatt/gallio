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

extern alias MbUnit2;

using System;
using Gallio.Common;
using Gallio.Model;
using Gallio.Model.Commands;
using Gallio.Common.Reflection;

using MbUnit2::MbUnit.Core.Remoting;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Represents an MbUnit v2 assembly-level test.
    /// </summary>
    internal class MbUnit2AssemblyTest : MbUnit2Test
    {
        private readonly FixtureExplorer fixtureExplorer;

        /// <summary>
        /// Creates an MbUnit v2 assembly-level test.
        /// </summary>
        /// <param name="fixtureExplorer">The fixture explorer.</param>
        /// <param name="assembly">The test assembly.</param>
        public MbUnit2AssemblyTest(FixtureExplorer fixtureExplorer, IAssemblyInfo assembly)
            : base(assembly.Name, assembly, null, null)
        {
            if (fixtureExplorer == null)
                throw new ArgumentNullException("fixtureExplorer");

            this.fixtureExplorer = fixtureExplorer;

            Kind = TestKinds.Assembly;
        }

        /// <summary>
        /// Gets the fixture explorer.
        /// </summary>
        public FixtureExplorer FixtureExplorer
        {
            get { return fixtureExplorer; }
        }
    }
}
