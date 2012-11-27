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
using System.Transactions;
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;
using System.Linq;
using Gallio.Common.Markup;
using Gallio.Common;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(DataGenerators))]
    public class DataGeneratorsTest_Join
    {
        [Test]
        public void CombinatorialJoin()
        {
            var actual = DataGenerators.Join(new[] { "A", "B", "C", "D" }, new[] { 1, 2, 3 }, JoinStrategy.Combinatorial);

            Assert.AreElementsEqualIgnoringOrder(new[] 
            { 
                new Pair<string, int>("A", 1),
                new Pair<string, int>("A", 2),
                new Pair<string, int>("A", 3),
                new Pair<string, int>("B", 1),
                new Pair<string, int>("B", 2),
                new Pair<string, int>("B", 3),
                new Pair<string, int>("C", 1),
                new Pair<string, int>("C", 2),
                new Pair<string, int>("C", 3),
                new Pair<string, int>("D", 1),
                new Pair<string, int>("D", 2),
                new Pair<string, int>("D", 3)
            }, actual);
        }

        [Test]
        public void SequentialJoin()
        {
            var actual = DataGenerators.Join(new[] { "A", "B", "C", "D" }, new[] { 1, 2, 3, 4 }, JoinStrategy.Sequential);

            Assert.AreElementsEqualIgnoringOrder(new[] 
            { 
                new Pair<string, int>("A", 1),
                new Pair<string, int>("B", 2),
                new Pair<string, int>("C", 3),
                new Pair<string, int>("D", 4),
            }, actual);
        }

        [Test]
        public void PairwiseJoin()
        {
            var actual = DataGenerators.Join(new[] { "A", "B" }, new[] { 1, 2 }, new[] { 5.0, 9.0 }, JoinStrategy.Pairwise);

            Assert.AreElementsEqualIgnoringOrder(new[] 
            { 
                new Triple<string, int, double>("A", 1, 9.0),
                new Triple<string, int, double>("B", 2, 9.0),
                new Triple<string, int, double>("A", 2, 5.0),
                new Triple<string, int, double>("B", 1, 5.0),
            }, actual);
        }

        [Test]
        public void CombinatorialJoin_with_high_rank()
        {
            var w = new[] { "A", "B" };
            var x = new[] { 1, 2, 3 };
            var y = new[] { 5.0, 9.0 };
            var z = new[] { 'U', 'V' };
            var actual = DataGenerators.Join(w, x, y, z);

            Assert.AreElementsEqualIgnoringOrder(new[] 
            { 
                new object[] { "A", 1, 5.0, 'U' },
                new object[] { "A", 1, 5.0, 'V' },
                new object[] { "A", 1, 9.0, 'U' },
                new object[] { "A", 1, 9.0, 'V' },
                new object[] { "A", 2, 5.0, 'U' },
                new object[] { "A", 2, 5.0, 'V' },
                new object[] { "A", 2, 9.0, 'U' },
                new object[] { "A", 2, 9.0, 'V' },
                new object[] { "A", 3, 5.0, 'U' },
                new object[] { "A", 3, 5.0, 'V' },
                new object[] { "A", 3, 9.0, 'U' },
                new object[] { "A", 3, 9.0, 'V' },
                new object[] { "B", 1, 5.0, 'U' },
                new object[] { "B", 1, 5.0, 'V' },
                new object[] { "B", 1, 9.0, 'U' },
                new object[] { "B", 1, 9.0, 'V' },
                new object[] { "B", 2, 5.0, 'U' },
                new object[] { "B", 2, 5.0, 'V' },
                new object[] { "B", 2, 9.0, 'U' },
                new object[] { "B", 2, 9.0, 'V' },
                new object[] { "B", 3, 5.0, 'U' },
                new object[] { "B", 3, 5.0, 'V' },
                new object[] { "B", 3, 9.0, 'U' },
                new object[] { "B", 3, 9.0, 'V' },
           }, actual, 
           (u, v) => u.Length == v.Length 
               && u.Length == 4 
               && (string)u[0] == (string)v[0]
               && (int)u[1] == (int)v[1]
               && (double)u[2] == (double)v[2]
               && (char)u[3] == (char)v[3]);
        }
    }
}
