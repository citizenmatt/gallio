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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gallio.Common.Policies;
using Gallio.VisualStudio.Shell.Core;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Gallio.VisualStudio.Shell.UI.ToolWindows
{
    /// <summary>
    /// General purpose container for tool windows.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type cannot be subclassed because tool windows must be registered with
    /// packages using a custom attribute on the package itself so the Shell only
    /// installs one kind of pane which it reuses with different content.
    /// </para>
    /// </remarks>
    [Guid("9C9191A4-CDFB-4931-9B96-6CC7CD0BC203")]
    [ComVisible(true)]
    public sealed class ShellToolWindowPane : ToolWindowPane, IToolWindowPane
    {
        private ToolWindowContainer toolWindowContainer;

        /// <summary>
		/// Constructs the tool window.
		/// </summary>
        public ShellToolWindowPane()
            : this(ShellProxy.Instance.Package) // must always succeed if the package is installed even if the Shell is not initialized
		{
        }

        /// <summary>
        /// Constructs the tool window with an explicitly provided service provider.  (for testing only)
        /// </summary>
        internal ShellToolWindowPane(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        /// <summary>
        /// An event that is raised when the tool window is disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                EventHandlerPolicy.SafeInvoke(Disposed, this, EventArgs.Empty);

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the tool window container.
        /// </summary>
        public ToolWindowContainer ToolWindowContainer
        {
            get
            {
                EnsureToolWindowContainer();
                return toolWindowContainer;
            }
        }

        /// <summary>
        /// Gets the tool window container.
        /// </summary>
        public override IWin32Window Window
        {
            get
            {
                return ToolWindowContainer;
            }
        }

        /// <summary>
        /// Gets or sets the window caption.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Due to a bug in the VS2008 SDK, <see cref="ToolWindowPane.Caption"/> does not work properly.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public new string Caption
        {
            get
            {
                object caption;
                Frame.GetProperty((int) __VSFPROPID.VSFPROPID_Caption, out caption);
                return (string) caption;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                Frame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, value);
            }
        }

        /// <summary>
        /// Gets or sets the frame hosting the window.
        /// </summary>
        public new IVsWindowFrame Frame
        {
            get
            {
                return (IVsWindowFrame)base.Frame;
            }
            set
            {
                base.Frame = value;
            }
        }

        /// <summary>
        /// Shows the window frame.
        /// </summary>
        public void Show()
        {
            Frame.Show();
        }

        /// <summary>
        /// Hides the window frame.
        /// </summary>
        public void Hide()
        {
            Frame.Hide();
        }

        /// <summary>
        /// Closes the window frame.
        /// </summary>
        public void Close()
        {
            Frame.CloseFrame(0);
        }

        private void EnsureToolWindowContainer()
        {
            if (toolWindowContainer == null)
            {
                toolWindowContainer = new ToolWindowContainer(this);
                toolWindowContainer.Visible = true;
            }
        }
    }
}
