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
using System.Collections;
using Gallio.Common.Text.RegularExpression;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generator of random <see cref="String"/> objects based on a regular expression filter mask.
    /// </summary>
    public abstract class RandomStringGenerator : RandomGenerator<string>
    {
        /// <summary>
        /// Constructs a generator of random <see cref="String"/> objects.
        /// </summary>
        protected RandomStringGenerator()
        {
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetSequence()
        {
            int i = 0;

            while (i < Count.Value)
            {
                var value = GetNextString();

                if (DoFilter(value))
                {
                    yield return value;
                    i++;
                }
            }
        }

        /// <summary>
        /// Generates the next random string.
        /// </summary>
        /// <returns>A random string.</returns>
        protected abstract string GetNextString();
    }
}
