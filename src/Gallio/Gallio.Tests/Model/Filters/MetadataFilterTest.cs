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
using Gallio.Common.Collections;
using Gallio.Tests;

using Gallio.Model.Filters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(MetadataFilter<ITestDescriptor>))]
    public class MetadataFilterTest : BaseTestWithMocks
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            new MetadataFilter<ITestDescriptor>(null, new EqualityFilter<string>("expectedValue"));
        }

        [Test]
        [Row(true, new string[] { "expectedValue" })]
        [Row(true, new string[] { "this", "that", "expectedValue" })]
        [Row(false, new string[] { "otherValue" })]
        [Row(false, new string[] { })]
        public void IsMatchCombinations(bool expectedMatch, string[] values)
        {
            PropertyBag metadata = new PropertyBag();
            foreach (string value in values)
                metadata.Add("key", value);

            ITestDescriptor component = Mocks.StrictMock<ITestDescriptor>();
            SetupResult.For(component.Metadata).Return(metadata);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch, new MetadataFilter<ITestDescriptor>("key",
                new EqualityFilter<string>("expectedValue")).IsMatch(component));
        }

        [Test]
        [Row("Key1", "Member1")]
        [Row("Key1", "Member2")]
        public void ToStringTest(string key, string value)
        {
            MetadataFilter<ITestDescriptor> filter = new MetadataFilter<ITestDescriptor>(key,
                new EqualityFilter<string>(value));
            Assert.AreEqual("Metadata('" + key + "', Equality('" + value + "'))", filter.ToString());
        }
    }
}