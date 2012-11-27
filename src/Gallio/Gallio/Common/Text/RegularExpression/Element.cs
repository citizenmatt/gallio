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
using System.Text;

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// Abstract base class for a regular expression composite tree leaf.
    /// </summary>
    internal abstract class Element : IElement
    {
        private readonly Quantifier quantifier;

        /// <summary>
        /// Constructs an inner element of a regular expression composite tree.
        /// </summary>
        /// <param name="quantifier">A quantifier specifying how many times the element is repeated.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="quantifier"/> is null.</exception>
        protected Element(Quantifier quantifier)
        {
            if (quantifier == null)
                throw new ArgumentNullException("quantifier");

            this.quantifier = quantifier;
        }

        /// <inheritdoc />
        public string GetRandomString(Random random)
        {
            var output = new StringBuilder();
            int repeat = quantifier.GetRandomRepeat(random);

            for (int i = 0; i < repeat; i++)
            {
                output.Append(GetRandomStringImpl(random));
            }

            return output.ToString();
        }

        /// <summary>
        /// Returns the random string that matches the regular expression element.
        /// </summary>
        /// <param name="random">A random number generator.</param>
        /// <returns>A random string.</returns>
        protected abstract string GetRandomStringImpl(Random random);
    }
}
