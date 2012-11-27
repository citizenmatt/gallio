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

using Gallio.Tests;
using MbUnit.Framework;

using Gallio.Model.Filters;
using Gallio.Model;
using Rhino.Mocks;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(NameFilter<ITestDescriptor>))]
    public class NameFilterTest : BaseTestWithMocks
    {
        [Test]
        [Row(true, "expectedValue")]
        [Row(false, "otherValue")]
        public void IsMatchCombinations(bool expectedMatch, string value)
        {
            ITestDescriptor component = Mocks.StrictMock<ITestDescriptor>();
            SetupResult.For(component.Name).Return(value);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch,
                new NameFilter<ITestDescriptor>(new EqualityFilter<string>("expectedValue")).IsMatch(component));
        }

        [Test]
        [Row("")]
        [Row("name1212")]
        public void ToStringTest(string name)
        {
            Filter<ITestDescriptor> filter = new NameFilter<ITestDescriptor>(new EqualityFilter<string>(name));
            Assert.AreEqual("Name(Equality('" + name + "'))", filter.ToString());
        }
    }
}