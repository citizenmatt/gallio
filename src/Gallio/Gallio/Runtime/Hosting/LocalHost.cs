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
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// An implementation of <see cref="IHost" /> that runs code
    /// locally within the current AppDomain.
    /// </summary>
    public class LocalHost : BaseHost
    {
        private readonly IDebuggerManager debuggerManager;
        private CurrentDirectorySwitcher currentDirectorySwitcher;
        private DefaultAssemblyLoader assemblyLoader;

        /// <summary>
        /// Creates a local host.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="logger">The logger for host message output.</param>
        /// <param name="debuggerManager">The debugger manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>, <paramref name="logger"/>
        /// or <paramref name="debuggerManager"/> is null.</exception>
        public LocalHost(HostSetup hostSetup, ILogger logger, IDebuggerManager debuggerManager)
            : base(hostSetup, logger)
        {
            if (debuggerManager == null)
                throw new ArgumentNullException("debuggerManager");

            this.debuggerManager = debuggerManager;

            if (! string.IsNullOrEmpty(hostSetup.WorkingDirectory))
                currentDirectorySwitcher = new CurrentDirectorySwitcher(hostSetup.WorkingDirectory);

            if (hostSetup.HintDirectories.Count != 0)
            {
                assemblyLoader = new DefaultAssemblyLoader();
                GenericCollectionUtils.ForEach(hostSetup.HintDirectories, assemblyLoader.AddHintDirectory);
            }
        }

        /// <inheritdoc />
        public sealed override bool IsLocal
        {
            get { return true; }
        }

        /// <inheritdoc />
        protected sealed override IHostService AcquireHostService()
        {
            AttachDebuggerIfNeeded(debuggerManager, Process.GetCurrentProcess());

            return new LocalHostService();
        }

        /// <inheritdoc />
        protected sealed override void ReleaseHostService(IHostService hostService)
        {
            DetachDebuggerIfNeeded();

            LocalHostService localHostService = (LocalHostService)hostService;
            localHostService.Dispose();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (assemblyLoader != null)
            {
                assemblyLoader.Dispose();
                assemblyLoader = null;
            }

            if (currentDirectorySwitcher != null)
            {
                currentDirectorySwitcher.Dispose();
                currentDirectorySwitcher = null;
            }

            base.Dispose(disposing);
        }

        private sealed class LocalHostService : BaseHostService
        {
        }
    }
}
