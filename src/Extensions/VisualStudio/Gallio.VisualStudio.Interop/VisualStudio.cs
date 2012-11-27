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
using System.Threading;
using EnvDTE;
using Gallio.Common;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using Gallio.VisualStudio.Interop.Native;
using Thread=System.Threading.Thread;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides control over Visual Studio.
    /// </summary>
    public class VisualStudio : IVisualStudio
    {
        private static readonly TimeSpan ComRetryTimeout = TimeSpan.FromSeconds(30);

        private readonly ILogger logger;
        private readonly DTE dte;
        private readonly VisualStudioVersion version;
        private readonly bool wasLaunched;

        /// <summary>
        /// Creates a wrapper for a particular DTE object.
        /// </summary>
        /// <param name="dte">The DTE object to wrap.</param>
        /// <param name="version">The version of Visual Studio represented by this object.</param>
        /// <param name="wasLaunched">True if Visual Studio was launched by our code, false if an existing instance was obtained.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dte"/> or <paramref name="logger"/> is null.</exception>
        public VisualStudio(DTE dte, VisualStudioVersion version, bool wasLaunched, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (dte == null)
                throw new ArgumentNullException("dte");

            this.logger = logger;
            this.dte = dte;
            this.version = version;
            this.wasLaunched = wasLaunched;
        }

        /// <inheritdoc />
        public VisualStudioVersion Version
        {
            get { return version; }
        }

        /// <inheritdoc />
        public bool WasLaunched
        {
            get { return wasLaunched; }
        }

        /// <inheritdoc />
        public void Call(Action<DTE> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Protect(() => action(dte));
        }

        private static void Protect(Action action)
        {
            Exception exception = null;

            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                Protect(action, out exception);
            }
            else
            {
                var thread = new Thread(() => Protect(action, out exception));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }

            if (exception != null)
                throw new VisualStudioException("Could not perform the requested Visual Studio operation.", exception);
        }

        private static void Protect(Action action, out Exception exception)
        {
            exception = null;

            ComRetryMessageFilter.Install(ComRetryTimeout);
            try
            {
                action();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                ComRetryMessageFilter.Uninstall();
            }
        }

        /// <inheritdoc />
        public void BringToFront()
        {
            Protect(() =>
                {
                    // Inspired from FxCop GUI implementation.
                    Window window = dte.MainWindow;

                    IntPtr hWnd = (IntPtr)window.HWnd;
                    if (NativeMethods.IsIconic(hWnd))
                        NativeMethods.ShowWindowAsync(hWnd, NativeConstants.SW_RESTORE);

                    NativeMethods.SetForegroundWindow(hWnd);
                    Thread.Sleep(50);

                    window.Activate();
                    window.Visible = true;
                });
        }

        /// <inheritdoc />
        public void Quit()
        {
            Protect(() => dte.Quit());
        }

        /// <inheritdoc />
        public IDebugger GetDebugger(DebuggerSetup debuggerSetup)
        {
            if (debuggerSetup == null)
                throw new ArgumentNullException("debuggerSetup");

            return new VisualStudioDebugger(debuggerSetup, logger, this);
        }
    }
}
