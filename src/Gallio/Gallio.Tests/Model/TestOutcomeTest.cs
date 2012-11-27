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
using Gallio.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(TestOutcome))]
    public class TestOutcomeTest
    {
        [Test]
        public void ConstructorWithStatusOnly()
        {
            TestOutcome outcome = new TestOutcome(TestStatus.Failed);
            Assert.AreEqual(TestStatus.Failed, outcome.Status);
            Assert.IsNull(outcome.Category);
        }

        [Test]
        public void ConstructorWithStatusAndNonEmptyCategory()
        {
            TestOutcome outcome = new TestOutcome(TestStatus.Failed, "blah");
            Assert.AreEqual(TestStatus.Failed, outcome.Status);
            Assert.AreEqual("blah", outcome.Category);
        }

        [Test]
        public void ConstructorWithStatusAndEmptyCategory()
        {
            TestOutcome outcome = new TestOutcome(TestStatus.Failed, "");
            Assert.AreEqual(TestStatus.Failed, outcome.Status);
            Assert.IsNull(outcome.Category);
        }

        [Test]
        public void ConstructorWithStatusAndNullCategory()
        {
            TestOutcome outcome = new TestOutcome(TestStatus.Failed, null);
            Assert.AreEqual(TestStatus.Failed, outcome.Status);
            Assert.IsNull(outcome.Category);
        }

        [Test]
        public void DisplayNameReturnsCategoryIfAvailableOrStatusOtherwise()
        {
            Assert.AreEqual("passed", new TestOutcome(TestStatus.Passed, null).DisplayName);
            Assert.AreEqual("error", new TestOutcome(TestStatus.Failed, "error").DisplayName);
        }

        [Test]
        public void CombineWithChoosesOutcomeWithGreaterSeverity()
        {
            Assert.AreEqual(TestOutcome.Passed, TestOutcome.Passed.CombineWith(TestOutcome.Passed));
            Assert.AreEqual(TestOutcome.Skipped, TestOutcome.Passed.CombineWith(TestOutcome.Skipped));
            Assert.AreEqual(TestOutcome.Inconclusive, TestOutcome.Skipped.CombineWith(TestOutcome.Inconclusive));
            Assert.AreEqual(TestOutcome.Error, TestOutcome.Inconclusive.CombineWith(TestOutcome.Error));
            Assert.AreEqual(TestOutcome.Error, TestOutcome.Skipped.CombineWith(TestOutcome.Error));
            Assert.AreEqual(TestOutcome.Error, TestOutcome.Error.CombineWith(TestOutcome.Passed));
        }

        [Test]
        [Row(false, null, null)]
        [Row(false, "same", "same")]
        [Row(false, "current", "other")]
        [Row(false, "current", null)]
        [Row(true, null, "other")]
        [Row(true, null, "passed")]
        [Row(true, null, "failed")]
        [Row(true, null, "ignored")]
        [Row(true, null, "error")]
        [Row(false, "passed", "passed")]
        [Row(false, "failed", "passed")]
        [Row(false, "ignored", "passed")]
        [Row(false, "error", "passed")]
        [Row(false, "passed", "failed")]
        [Row(false, "failed", "failed")]
        [Row(false, "ignored", "failed")]
        [Row(false, "error", "failed")]
        public void CombineWithChoosesOutcomeBasedOnCategoryWhenSameSeverity(bool expectCurrentCategoryToChange, 
            string currentCategory, string otherCategory, [EnumData(typeof(TestStatus))] TestStatus status)
        {
            var expected = new TestOutcome(status, expectCurrentCategoryToChange ? otherCategory : currentCategory);
            var current = new TestOutcome(status, currentCategory);
            var other = new TestOutcome(status, otherCategory);
            var combined = current.CombineWith(other);
            Assert.AreEqual(expected, combined);
        }
        
        [Test]
        public void GeneralizeOmitsCategory()
        {
            Assert.AreEqual(TestOutcome.Failed, TestOutcome.Failed.Generalize());
            Assert.AreEqual(TestOutcome.Failed, TestOutcome.Error.Generalize());
        }

        [Test]
        [Row("Passed", TestStatus.Passed, null)]
        [Row("Failed", TestStatus.Failed, null)]
        [Row("Error", TestStatus.Failed, "error")]
        [Row("Timeout", TestStatus.Failed, "timeout")]
        [Row("Inconclusive", TestStatus.Inconclusive, null)]
        [Row("Canceled", TestStatus.Inconclusive, "canceled")]
        [Row("Skipped", TestStatus.Skipped, null)]
        [Row("Ignored", TestStatus.Skipped, "ignored")]
        [Row("Pending", TestStatus.Skipped, "pending")]
        public void BuiltInOutcomeProperties(string propertyName, TestStatus expectedStatus, string expectedCategory)
        {
            TestOutcome outcome = (TestOutcome) typeof(TestOutcome).GetProperty(propertyName).GetValue(null, null);
            Assert.AreEqual(expectedStatus, outcome.Status);
            Assert.AreEqual(expectedCategory, outcome.Category);
        }

        [Test]
        public void Equality()
        {
            Assert.IsTrue(TestOutcome.Error == TestOutcome.Error);
            Assert.IsFalse(TestOutcome.Error == TestOutcome.Failed);

            Assert.IsTrue(TestOutcome.Error != TestOutcome.Canceled);
            Assert.IsFalse(TestOutcome.Error != TestOutcome.Error);

            Assert.IsTrue(TestOutcome.Error.Equals(TestOutcome.Error));
            Assert.IsFalse(TestOutcome.Error.Equals(new TestOutcome(TestStatus.Passed, "error")));

            Assert.IsTrue(TestOutcome.Error.Equals((object)TestOutcome.Error));
            Assert.IsFalse(TestOutcome.Error.Equals((object)new TestOutcome(TestStatus.Passed, "error")));
            Assert.IsFalse(TestOutcome.Error.Equals(null));

            Assert.AreEqual(TestOutcome.Error.GetHashCode(), TestOutcome.Error.GetHashCode());
            Assert.AreEqual(TestOutcome.Passed.GetHashCode(), TestOutcome.Passed.GetHashCode());
        }

        [Test]
        public void TypeIsXmlSerializable()
        {
            Assert.IsXmlSerializableType(typeof(TestOutcome));
        }

        [Test]
        public void RoundTripXmlSerializationWhenCategoryIsNull()
        {
            Assert.AreEqual(TestOutcome.Passed, Assert.XmlSerializeThenDeserialize(TestOutcome.Passed));
        }

        [Test]
        public void RoundTripXmlSerializationWhenCategoryIsNonNull()
        {
            Assert.AreEqual(TestOutcome.Error, Assert.XmlSerializeThenDeserialize(TestOutcome.Error));
        }
    }
}
