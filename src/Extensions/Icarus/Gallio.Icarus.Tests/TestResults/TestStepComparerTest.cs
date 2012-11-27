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
using Gallio.Icarus.TestResults;
using MbUnit.Framework;
using NHamcrest.Core;

namespace Gallio.Icarus.Tests.TestResults
{
    [TestsOn(typeof(TestStepComparer)), Category("TestResults")]
    public class TestStepComparerTest
    {
        private TestStepComparer comparer;

        [SetUp]
        public void SetUp()
        {
            comparer = new TestStepComparer();
        }

        [Test]
        public void Equal_if_left_and_right_are_null()
        {
            var result = comparer.Compare(null, null);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Less_if_left_is_null()
        {
            var result = comparer.Compare(null, new ListViewItem());

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void More_if_right_is_null()
        {
            var result = comparer.Compare(new ListViewItem(), null);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Comparing_durations_should_be_correct()
        {
            comparer.SortColumn = 2;
            var left = new ListViewItem("Left");
            const double durationLeft = 1.0;
            left.SubItems.AddRange(new[] { "", durationLeft.ToString() });
            var right = new ListViewItem("Right");
            const double durationRight = 2.0;
            right.SubItems.AddRange(new[] { "", durationRight.ToString() });

            var result = comparer.Compare(left, right);

            Assert.AreEqual(result, (durationLeft.CompareTo(durationRight)));
        }

        [Test]
        public void Comparing_durations_should_be_correct_when_sorted_desc()
        {
            comparer.SortColumn = 2;
            comparer.SortOrder = Icarus.Models.SortOrder.Descending;
            var left = new ListViewItem();
            const double durationLeft = 1.0;
            left.SubItems.AddRange(new[] { "", durationLeft.ToString() });
            var right = new ListViewItem();
            const double durationRight = 2.0;
            right.SubItems.AddRange(new[] { "", durationRight.ToString() });

            var result = comparer.Compare(left, right);

            Assert.AreEqual(result, (durationRight.CompareTo(durationLeft)));
        }

        [Test]
        public void Comparing_assertions_should_be_correct()
        {
            comparer.SortColumn = 3;
            const int assertionsLeft = 4;
            const int assertionsRight = 12;
            var left = new ListViewItem();
            left.SubItems.AddRange(new[] { "", "", assertionsLeft.ToString() });
            var right = new ListViewItem();
            right.SubItems.AddRange(new[] { "", "", assertionsRight.ToString() });

            var result = comparer.Compare(left, right);

            Assert.AreEqual(result, (assertionsLeft.CompareTo(assertionsRight)));
        }

        [Test]
        public void Comparing_assertions_should_be_correct_when_sorted_desc()
        {
            comparer.SortColumn = 3;
            comparer.SortOrder = Icarus.Models.SortOrder.Descending;
            const int assertionsLeft = 4;
            const int assertionsRight = 12;
            var left = new ListViewItem();
            left.SubItems.AddRange(new[] { "", "", assertionsLeft.ToString() });
            var right = new ListViewItem();
            right.SubItems.AddRange(new[] { "", "", assertionsRight.ToString() });

            var result = comparer.Compare(left, right);

            Assert.AreEqual(result, (assertionsRight.CompareTo(assertionsLeft)));
        }

        [Test]
        public void Comparing_text_should_be_correct()
        {
            comparer.SortColumn = 0;
            const string textLeft = "aaa";
            const string textRight = "zzz";
            var left = new ListViewItem(textLeft);
            var right = new ListViewItem(textRight);

            var result = comparer.Compare(left, right);

            Assert.AreEqual(result, (textLeft.CompareTo(textRight)));
        }

        [Test]
        public void Comparing_text_should_be_correct_when_sorted_desc()
        {
            comparer.SortColumn = 0;
            comparer.SortOrder = Icarus.Models.SortOrder.Descending;
            const string textLeft = "aaa";
            const string textRight = "zzz";
            var left = new ListViewItem(textLeft);
            var right = new ListViewItem(textRight);

            var result = comparer.Compare(left, right);

            Assert.AreEqual(result, (textRight.CompareTo(textLeft)));
        }
    }
}
