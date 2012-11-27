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
using Gallio.Common.Policies;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime
{
    /// <summary>
    /// Initializes and shuts down the runtime.
    /// </summary>
    public static class RuntimeBootstrap
    {
        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Loads plugins and initalizes the runtime component model.  The
        /// specifics of the system can be configured by editing the appropriate
        /// *.plugin files to register new components and facilities as required.
        /// </para>
        /// </remarks>
        /// <param name="setup">The runtime setup parameters.</param>
        /// <param name="logger">The logger to attach, or null if none.</param>
        /// <returns>An object that when disposed automatically calls <see cref="Shutdown" />.
        /// This is particularly useful in combination with the C# "using" statement
        /// or its equivalent.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="setup"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the runtime has already been initialized.</exception>
        public static IDisposable Initialize(RuntimeSetup setup, ILogger logger)
        {
            if (setup == null)
                throw new ArgumentNullException("setup");

            if (RuntimeAccessor.IsInitialized)
                throw new InvalidOperationException("The runtime has already been initialized.");

            var registry = new Registry();
            var assemblyLoader = new DefaultAssemblyLoader();
            var pluginLoader = new CachingPluginLoader();
            IRuntime runtime = new DefaultRuntime(registry, pluginLoader, assemblyLoader, setup); // TODO: make me configurable via setup
            if (logger != null)
                runtime.AddLogListener(logger);

            try
            {
                RuntimeAccessor.SetRuntime(runtime);

                runtime.Initialize();

                if (!UnhandledExceptionPolicy.HasReportUnhandledExceptionHandler)
                    UnhandledExceptionPolicy.ReportUnhandledException += HandleUnhandledExceptionNotification;
            }
            catch (Exception)
            {
                RuntimeAccessor.SetRuntime(null);
                throw;
            }

            return new AutoShutdown();
        }

        /// <summary>
        /// Shuts down the runtime if it is currently initialized.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does nothing if the runtime has not been initialized.
        /// </para>
        /// </remarks>
        public static void Shutdown()
        {
            if (!RuntimeAccessor.IsInitialized)
                return;

            try
            {
                RuntimeAccessor.Instance.Dispose();
            }
            finally
            {
                UnhandledExceptionPolicy.ReportUnhandledException -= HandleUnhandledExceptionNotification;

                RuntimeAccessor.SetRuntime(null);
            }
        }

        private static void HandleUnhandledExceptionNotification(object sender, CorrelatedExceptionEventArgs e)
        {
            if (e.IsRecursive)
                return;

            RuntimeAccessor.Logger.Log(LogSeverity.Error, "Internal error: " + e.GetDescription());
        }

        private sealed class AutoShutdown : IDisposable
        {
            public void Dispose()
            {
                Shutdown();
            }
        }
    }
}
