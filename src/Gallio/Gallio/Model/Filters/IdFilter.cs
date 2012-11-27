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
using Gallio.Model;

namespace Gallio.Model.Filters
{
    /// <summary>
    /// A filter that matches objects whose <see cref="ITestDescriptor.Id" />
    /// matches the specified id filter.
    /// </summary>
    [Serializable]
    public class IdFilter<T> : PropertyFilter<T> where T : ITestDescriptor
    {
        /// <summary>
        /// Creates an identity filter.
        /// </summary>
        /// <param name="idFilter">A filter for the id.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="idFilter"/> is null.</exception>
        public IdFilter(Filter<string> idFilter)
            : base(idFilter)
        {
        }

        /// <inheritdoc />
        public override string Key
        {
            get { return @"Id"; }
        }

        /// <inheritdoc />
        public override bool IsMatch(T value)
        {
            return ValueFilter.IsMatch(value.Id);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return @"Id(" + ValueFilter + @")";
        }
    }
}