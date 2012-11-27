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
using Gallio.Framework.Assertions;
using Gallio.Runtime.Formatting;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// A diff item representing one single difference between two XML fragments.
    /// </summary>
    public sealed class Diff
    {
        private readonly DiffType diffType;
        private readonly IXmlPathStrict path;
        private readonly DiffTargets targets;

        /// <summary>
        /// Gets a type of the diff.
        /// </summary>
        public DiffType Type
        {
            get
            {
                return diffType;
            }
        }

        /// <summary>
        /// Gets the path of the difference.
        /// </summary>
        public IXmlPathStrict Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Indicates which XML fragment is targeted by the diff.
        /// </summary>
        public DiffTargets Targets
        {
            get
            {
                return targets;
            }
        }

        /// <summary>
        /// Constructs a diff item.
        /// </summary>
        /// <param name="diffType">The type of the diff.</param>
        /// <param name="path">The path of the difference.</param>
        /// <param name="targets">Indicates which XML fragment is targeted by the diff.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="diffType"/> or <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="diffType"/> or <paramref name="path"/> is empty.</exception>
        public Diff(DiffType diffType, IXmlPathStrict path, DiffTargets targets)
        {
            if (diffType == null)
                throw new ArgumentNullException("diffType");
            if (path == null)
                throw new ArgumentNullException("path");

            this.diffType = diffType;
            this.path = path;
            this.targets = targets;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("{0} at '{1}'.", diffType.Description, path.ToString());
        }

        /// <summary>
        /// Returns the diff as an assertion failure.
        /// </summary>
        /// <param name="expected">The expected fragment used to format the diff.</param>
        /// <param name="actual">The actual fragment used to format the diff.</param>
        /// <returns>The resulting assertion failure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public AssertionFailure ToAssertionFailure(NodeFragment expected, NodeFragment actual)
        {
            bool showActual = ((targets & DiffTargets.Actual) != 0);
            bool showExpected = ((targets & DiffTargets.Expected) != 0);
            var builder = new AssertionFailureBuilder(diffType.Description, new NullFormatter());
            const XmlPathRenderingOptions options = XmlPathRenderingOptions.UseIndentation;

            if (showActual && showExpected)
            {
                var actualFormatted = XmlPathRenderer.Run(path, actual, options);
                var expectedFormatted = XmlPathRenderer.Run(path, expected, options);
                builder.AddRawExpectedAndActualValuesWithDiffs(expectedFormatted, actualFormatted);
            }
            else if (showActual)
            {
                var actualFormatted = XmlPathRenderer.Run(path, actual, options);
                builder.AddRawActualValue(actualFormatted);
            }
            else if (showExpected)
            {
                var expectedFormatted = XmlPathRenderer.Run(path, expected, options);
                builder.AddRawExpectedValue(expectedFormatted);
            }

            return builder.ToAssertionFailure();
        }

        private class NullFormatter : IFormatter
        {
            public string Format(object obj)
            {
                return (string) obj;
            }
        }
    }
}
