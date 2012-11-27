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
using System.Text;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to verify that the plugin metadata and installation parameters are correct.
    /// </summary>
    public class VerifyInstallationUtilityCommand : BaseUtilityCommand<object>
    {
        private readonly IRuntime runtime;

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="runtime">The runtime, not null.</param>
        public VerifyInstallationUtilityCommand(IRuntime runtime)
        {
            this.runtime = runtime;
        }

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, object arguments)
        {
            return runtime.VerifyInstallation() ? 0 : 1;
        }
    }
}
