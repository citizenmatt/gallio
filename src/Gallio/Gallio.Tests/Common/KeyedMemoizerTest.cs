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
using System.Linq;
using System.Text;
using Gallio.Common;
using MbUnit.Framework;

namespace Gallio.Tests.Common
{
    public class KeyedMemoizerTest
    {
        [Test]
        [Row(0)]
        [Row(100)]
        public void WhenValueNotPresentPopulatesAndMemoizesItByKey(int initialSize)
        {
            KeyedMemoizer<string, int> memoizer = new KeyedMemoizer<string, int>();

            for (int i = 0; i < initialSize; i++)
                Assert.AreEqual(i, memoizer.Memoize(i + "x", () => i));

            Assert.AreEqual(42, memoizer.Memoize("42", () => 42));
            Assert.AreEqual(42, memoizer.Memoize("42", () => { throw new InvalidOperationException("Should not be called"); }));

            Assert.AreEqual(31, memoizer.Memoize("31", () => 31));
            Assert.AreEqual(31, memoizer.Memoize("31", () => { throw new InvalidOperationException("Should not be called"); }));

            Assert.AreEqual(-1, memoizer.Memoize(null, () => -1));

            Assert.AreEqual(42, memoizer.Memoize("42", () => { throw new InvalidOperationException("Should not be called"); }));
            Assert.AreEqual(-1, memoizer.Memoize(null, () => { throw new InvalidOperationException("Should not be called"); }));
            Assert.AreEqual(31, memoizer.Memoize("31", () => { throw new InvalidOperationException("Should not be called"); }));
        }

        [Test]
        [Row(100)]
        public void HybridTableOptimizationHandsOffAllValuesToDictionary(int count)
        {
            KeyedMemoizer<int?, int> memoizer = new KeyedMemoizer<int?, int>();
            for (int i = 0; i < count; i++)
                Assert.AreEqual(i, memoizer.Memoize(i, () => i));

            for (int i = 0; i < count; i++)
                Assert.AreEqual(i, memoizer.Memoize(i, () => { throw new InvalidOperationException("Should not happen."); }));
        }
    }
}