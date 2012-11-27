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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Gallio.UI.ControlPanel;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Base class for components that present preference panels.
    /// </summary>
    public partial class PreferencePane : SettingsEditor
    {
        /// <summary>
        /// Creates a preference pane.
        /// </summary>
        public PreferencePane()
        {
            InitializeComponent();
        }
    }
}
