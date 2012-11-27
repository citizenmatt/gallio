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
using System.Windows.Forms;

namespace Gallio.VisualStudio.Shell.UI.ToolWindows
{
    /// <summary>
    /// Abstract base class for tool windows.
    /// </summary>
    public class ToolWindow : UserControl
    {
        private IToolWindowPane toolWindowPane;
        private string storedCaption = string.Empty;

        /// <summary>
        /// Default constructor. (For designer.)
        /// </summary>
        public ToolWindow()
        {
        }

        /// <summary>
        /// Gets the associated tool window pane, or null if the tool window has not been opened yet.
        /// </summary>
        public IToolWindowPane ToolWindowPane
        {
            get { return toolWindowPane; }
        }

        /// <summary>
        /// Gets or sets the window caption.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string Caption
        {
            get
            {
                if (toolWindowPane != null)
                    storedCaption = toolWindowPane.Caption;
                return storedCaption;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                storedCaption = value;
                if (toolWindowPane != null)
                    toolWindowPane.Caption = value;
            }
        }

        /// <summary>
        /// Closes the window frame.
        /// </summary>
        public void Close()
        {
            ThrowIfNoPane();
            toolWindowPane.Close();
        }

        /// <inheritdoc />
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            ToolWindowContainer toolWindowContainer = Parent as ToolWindowContainer;
            if (toolWindowContainer != null)
            {
                toolWindowPane = toolWindowContainer.ToolWindowPane;
            }
            else
            {
                toolWindowPane = null;
            }
        }

        /// <inheritdoc />
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);

            if (toolWindowPane != null)
            {
                if (value)
                    toolWindowPane.Show();
                else
                    toolWindowPane.Hide();
            }
        }

        private void ThrowIfNoPane()
        {
            if (toolWindowPane == null)
                throw new InvalidOperationException("The tool window does not have an associated tool window pane.");
        }
    }
}
