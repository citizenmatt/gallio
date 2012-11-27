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
using Gallio.Runtime.UtilityCommands;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Installs or uninstalls components using the <see cref="IInstallerManager"/>.
    /// </summary>
    public class SetupUtilityCommand : BaseUtilityCommand<SetupUtilityCommand.Arguments>
    {
        private readonly IInstallerManager installerManager;

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="installerManager">The installer manager, not null.</param>
        public SetupUtilityCommand(IInstallerManager installerManager)
        {
            this.installerManager = installerManager;
        }

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, Arguments arguments)
        {
            context.ProgressMonitorProvider.Run(progressMonitor =>
            {
                List<string> installerIds = arguments.InstallerIds.Length != 0
                    ? new List<string>(arguments.InstallerIds)
                    : null;

                if (arguments.Install)
                {
                    installerManager.Install(installerIds, null, progressMonitor);
                }
                else
                {
                    installerManager.Uninstall(installerIds, null, progressMonitor);
                }
            });

            return 0;
        }

        /// <inheritdoc />
        public override bool ValidateArguments(Arguments arguments, CommandLineErrorReporter errorReporter)
        {
            if (!arguments.Install && !arguments.Uninstall
                || arguments.Install && arguments.Uninstall)
            {
                errorReporter("Exactly one of the options /install or /uninstall must be specified.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// The arguments for the command.
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// When set to true, installs components.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "Installs all registered components.",
                LongName = "install",
                ShortName = "i")]
            public bool Install;

            /// <summary>
            /// When set to true, uninstalls components.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "Uninstalls all registered components.",
                LongName = "uninstall",
                ShortName = "u")]
            public bool Uninstall;

            /// <summary>
            /// The ids of installers to include.
            /// </summary>
            [DefaultCommandLineArgument(CommandLineArgumentFlags.MultipleUnique,
                Description = "The installer ids to include, or an empty list to include all installers.",
                ValueLabel = "installer id")]
            public string[] InstallerIds;
        }
    }
}
