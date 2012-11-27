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
using System.Text;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// Diffing engine for collections of unordered elements.
    /// </summary>
    internal sealed class DiffEngineForUnorderedElements : IDiffEngine<NodeCollection>
    {
        private readonly NodeCollection expected;
        private readonly NodeCollection actual;
        private readonly IXmlPathStrict path;
        private readonly IXmlPathStrict pathExpected;
        private readonly Options options;

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="pathExpected">The path of the parent node of the expected collection.</param>
        /// <param name="options">Equality options.</param>
        public DiffEngineForUnorderedElements(NodeCollection expected, NodeCollection actual, IXmlPathStrict path, IXmlPathStrict pathExpected, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");
            if (path == null)
                throw new ArgumentNullException("path");
            if (pathExpected == null)
                throw new ArgumentNullException("pathExpected");

            this.expected = expected;
            this.actual = actual;
            this.path = path;
            this.pathExpected = pathExpected;
            this.options = options;
        }

        /// <inheritdoc />
        public DiffSet Diff()
        {
            var notified = new List<int>();
            return new DiffSetBuilder()
                .Add(FindElements(notified, true, DiffType.MissingElement))
                .Add(FindElements(notified, false, DiffType.UnexpectedElement))
                .ToDiffSet();
        }

        private DiffSet FindElements(IList<int> notified, bool invert, DiffType diffType)
        {
            var builder = new DiffSetBuilder();
            var mask = new List<int>();
            var noExactMatch = new List<int>();
            var source = invert ? expected : actual;
            var pool = invert ? actual : expected;

            // Find first exact match (= empty diff)
            for (int i = 0; i < source.Count; i++)
            {
                int j = pool.FindIndex(x => !mask.Contains(x) && source[i].Diff(pool[x], XmlPathRoot.Strict.Empty, XmlPathRoot.Strict.Empty, options).IsEmpty);

                if (j < 0)
                {
                    noExactMatch.Add(i);
                }
                else
                {
                    mask.Add(j);
                }
            }

            // Find first name-only match for the remaining items without exact match.
            foreach (int i in noExactMatch)
            {
                int j = pool.FindIndex(x => !mask.Contains(x) && source[i].AreNamesEqual(pool[x].Name, options));

                if (j < 0)
                {
                    builder.Add(new Diff(diffType, (invert ? pathExpected : path).Element(i), invert ? DiffTargets.Expected : DiffTargets.Actual));
                }
                else
                {
                    int k = invert ? j : i;
                    DiffSet diffSet = actual[k].Diff(expected[invert ? i : j], path, pathExpected, options);

                    if (!diffSet.IsEmpty && !notified.Contains(k))
                    {
                        builder.Add(diffSet);
                        notified.Add(k);
                    }

                    mask.Add(j);
                }
            }

            return builder.ToDiffSet();
        }
    }
}
