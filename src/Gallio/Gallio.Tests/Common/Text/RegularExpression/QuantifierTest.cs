﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using System.Text;
using Gallio.Framework.Data.Generation;
using MbUnit.Framework;
using Gallio.Framework;
using Gallio.Common.Text.RegularExpression;

namespace Gallio.Tests.Common.Text.RegularExpression
{
    [TestFixture]
    [TestsOn(typeof(Quantifier))]
    public class QuantifierTest
    {
        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_constant_should_throw_exception()
        {
            new Quantifier(-1);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_minimum_should_throw_exception()
        {
            new Quantifier(-1, 10);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_maximum_should_throw_exception()
        {
            new Quantifier(10, -1);
        }

        [Test]
        [ExpectedArgumentException]
        public void Constructs_with_minimum_greater_than_maximum_should_throw_exception()
        {
            new Quantifier(10, 1);
        }

        [Test]
        public void Constructs_with_range_ok()
        {
            var quantifier = new Quantifier(2, 8);
            Assert.AreEqual(2, quantifier.Minimum);
            Assert.AreEqual(8, quantifier.Maximum);
        }

        [Test]
        public void Constructs_with_constant_ok()
        {
            var quantifier = new Quantifier(5);
            Assert.AreEqual(5, quantifier.Minimum);
            Assert.AreEqual(5, quantifier.Maximum);
        }

        [Test]
        [Row(1, 10)]
        [Row(3, 3)]
        [Row(0, 1)]
        [Row(0, 0)]
        [Row(0, 3)]
        [Row(3, 5)]
        public void GetRandomQuantity(int minimum, int maximum)
        {
            var quantifier = new Quantifier(minimum, maximum);
            var random = new Random();

            Assert.Multiple(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    var actual = quantifier.GetRandomRepeat(random);
                    Assert.Between(actual, minimum, maximum);
                }
            });
        }
    }
}
