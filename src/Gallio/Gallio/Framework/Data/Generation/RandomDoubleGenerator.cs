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

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generator of random <see cref="double"/> values within a given range.
    /// </summary>
    public class RandomDoubleGenerator : RandomRangeGenerator<double>
    {
        /// <summary>
        /// Constructs a generator of random <see cref="double"/> numbers.
        /// </summary>
        public RandomDoubleGenerator()
        {
        }

        /// <inheritdoc/>
        protected override IEnumerable<double> GetSequence()
        {
            CheckProperty(Minimum.Value, "Minimum");
            CheckProperty(Maximum.Value, "Maximum");
            int i = 0;

            while (i < Count.Value)
            {
                var value = GetNextRandomValue();

                if (DoFilter(value))
                {
                    yield return value;
                    i++;
                }
            }
        }

        private double GetNextRandomValue()
        {
            return Minimum.Value + InnerGenerator.NextDouble() * (Maximum.Value - Minimum.Value);
        }
    }
}
