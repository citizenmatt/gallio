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

using System.Collections.Generic;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using System.Windows.Forms;
using System.Drawing;
using Gallio.UI.Tree.NodeControls;

namespace Gallio.Icarus.Controls
{
    public class TestTreeView : TreeViewAdv
    {
        private readonly NodeCheckBox nodeCheckBox;
        private readonly TestNodeTextBox nodeTextBox;

        public TestTreeView()
        {
            nodeCheckBox = new NodeCheckBox();
            NodeControls.Add(nodeCheckBox);

            NodeControls.Add(new TestKindIcon());

            NodeControls.Add(new TestStatusNodeIcon());

            nodeTextBox = new TestNodeTextBox();
            NodeControls.Add(nodeTextBox);
        }

        public void SetPassedColor(Color value)
        {
            nodeTextBox.PassedColor = value;
        }

        public void SetFailedColor(Color value)
        {
            nodeTextBox.FailedColor = value;
        }

        public void SetSkippedColor(Color value)
        {
            nodeTextBox.SkippedColor = value;
        }

        public void SetInconclusiveColor(Color value)
        {
            nodeTextBox.InconclusiveColor = value;
        }

        public void SetEditEnabled(bool enabled)
        {
             nodeCheckBox.EditEnabled = enabled;
        }

        public IEnumerable<string> GetCollapsedNodes()
        {
            var collapsedNodes = new List<string>();
            foreach (var treeNode in AllNodes)
            {
                if (treeNode.IsExpanded)
                    continue;

                var id = ((TestTreeNode)treeNode.Tag).Id;
                collapsedNodes.Add(id);
            }
            return collapsedNodes;
        }

        public void Expand(TestStatus state)
        {
            BeginUpdate();
            
            CollapseAll();
            
            foreach (TreeNodeAdv node in AllNodes)
                ExpandNode(node, state);
            
            EndUpdate();
        }

        private static void ExpandNode(TreeNodeAdv node, TestStatus state)
        {
            if (((TestTreeNode)node.Tag).TestStatus == state)
                Expand(node);

            foreach (var tNode in node.Children)
                ExpandNode(tNode, state);
        }

        private static void Expand(TreeNodeAdv node)
        {
            // Loop through all parent nodes that are not already
            // expanded and expand them.
            if (node.Parent != null && !node.Parent.IsExpanded)
                Expand(node.Parent);

            node.Expand();
        }

        public void CollapseNodes(IList<string> nodes)
        {
            ExpandAll();
            foreach (var treeNode in AllNodes)
            {
                if (treeNode.IsExpanded && nodes.Contains(((TestTreeNode)treeNode.Tag).Id))
                    treeNode.Collapse();
            }
        }

        public void Select(TestStatus testStatus)
        {
            BeginUpdate();

            CollapseAll();

            foreach (TreeNodeAdv node in AllNodes)
                SelectNode(node, testStatus);

            EndUpdate();
        }

        private static void SelectNode(TreeNodeAdv node, TestStatus testStatus)
        {
            var testDataNode = node.Tag as TestDataNode;

            if (testDataNode == null)
                return;

            if (testDataNode.IsTest)
            {
                if (testDataNode.TestStatus == testStatus)
                {
                    testDataNode.CheckState = CheckState.Checked;
                    Expand(node);
                }
                else
                {
                    testDataNode.CheckState = CheckState.Unchecked;
                }
            }
            else
            {
                testDataNode.CheckState = CheckState.Unchecked;
            }

            foreach (TreeNodeAdv tNode in node.Children)
            {
                SelectNode(tNode, testStatus);
            }
        }
    }
}
