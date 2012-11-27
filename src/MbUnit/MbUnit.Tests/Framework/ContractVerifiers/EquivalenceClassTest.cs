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
using System.Linq;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using Gallio.Common.Collections;

namespace MbUnit.Tests.Framework.ContractVerifiers
{
    [TestFixture]
    [TestsOn(typeof(EquivalenceClass))]
    public class EquivalenceClassTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructsWithNullInitializerForValueType()
        {
            new EquivalenceClass(null);
        }

        [Test, ExpectedArgumentException]
        public void ConstructsWithInitializerContainingNoObjects()
        {
            new EquivalenceClass(EmptyArray<object>.Instance);
        }

        [Test, ExpectedArgumentException]
        public void ConstructsWithInitializerContainingNullReference()
        {
            new EquivalenceClass(new Object(), new Object(), null);
        }

        [Test]
        public void ConstructsOk()
        {
            var object1 = new object();
            var object2 = new object();
            var object3 = new object();
            var target = new EquivalenceClass(object1, object2, object3);
            Assert.AreElementsSame(new[] { object1, object2, object3 }, target.Cast<object>());
        }
    }
}
