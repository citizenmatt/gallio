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

using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.PluginNodes;
using Gallio.Runtime;

namespace Gallio.Icarus.Views.PluginBrowser
{
    public partial class PluginBrowser : UserControl
    {
        private readonly PluginTreeModel model;

        public PluginBrowser()
        {
            InitializeComponent();

            model = new PluginTreeModel(RuntimeAccessor.Registry);
            pluginBrowserTreeView.Model = model;
        }

        private void pluginBrowserTreeView_SelectionChanged(object sender, System.EventArgs e)
        {
            if (pluginBrowserTreeView.SelectedNode == null || pluginBrowserTreeView.SelectedNode.Tag == null)
                return;

            ITreeModel detailsModel = null;
            if (pluginBrowserTreeView.SelectedNode.Tag is PluginNode)
            {
                var node = (PluginNode)pluginBrowserTreeView.SelectedNode.Tag;
                var details = model.GetPluginDetails(node.Text);
                detailsModel = new PluginDetailsTreeModel(details);
            }
            else if (pluginBrowserTreeView.SelectedNode.Tag is ServiceNode)
            {
                var node = (ServiceNode)pluginBrowserTreeView.SelectedNode.Tag;
                var details = model.GetServiceDetails(node.Text);
                detailsModel = new ServiceDetailsTreeModel(details);
            }
            else if (pluginBrowserTreeView.SelectedNode.Tag is ComponentNode)
            {
                var node = (ComponentNode)pluginBrowserTreeView.SelectedNode.Tag;
                var details = model.GetComponentDetails(node.Text);
                detailsModel = new ComponentDetailsTreeModel(details);
            }
            pluginDetailsTreeView.Model = detailsModel;
        }
    }
}
