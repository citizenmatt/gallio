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
using System.Windows.Forms;
using Gallio.Icarus.Models;
using Gallio.Model;
using MbUnit.Framework;
using NHamcrest.Core;

namespace Gallio.Icarus.Tests.Models
{
    public class TestTreeNodeTest
    {
        [Test]
        public void When_a_nodes_test_status_is_updated_its_parent_should_be_too()
        {
            var parent = new TestTreeNode("parent", "parent");
            var child1 = new TestTreeNode("child1", "child1");
            var child2 = new TestTreeNode("child2", "child2");
            parent.Nodes.Add(child1);
            parent.Nodes.Add(child2);

            child1.TestStatus = TestStatus.Passed;

            Assert.AreEqual(TestStatus.Passed, parent.TestStatus);
        }

        [Test]
        public void Filtering_node()
        {
            var node = new TestTreeNode("id", "text");

            node.IsFiltered = true;

            Assert.IsTrue(node.IsFiltered);
        }

        [Test]
        public void Node_should_be_unchecked_when_filtered()
        {
            var node = new TestTreeNode("id", "text");

            node.IsFiltered = true;

            Assert.AreEqual(CheckState.Unchecked, node.CheckState);
        }

        [Test]
        public void Node_check_status_should_be_unchanged_when_unfiltered()
        {
            var node = new TestTreeNode("id", "text")
            {
                CheckState = CheckState.Checked
            };

            node.IsFiltered = false;

            Assert.AreEqual(CheckState.Checked, node.CheckState);
        }

        [Test]
        public void Alphanumeric_comparison()
        {
            var node1 = new TestTreeNode("id", "AAA");
            var node2 = new TestTreeNode("id", "10A");
            var node3 = new TestTreeNode("id", "1AA");
            var node4 = new TestTreeNode("id", "ABC");
            var list = new List<TestTreeNode> { node4, node2, node3, node1 };

            list.Sort();

            Assert.That(list[0], Is.EqualTo(node3));
            Assert.That(list[1], Is.EqualTo(node2));
            Assert.That(list[2], Is.EqualTo(node1));
            Assert.That(list[3], Is.EqualTo(node4));
        }
    }
}
