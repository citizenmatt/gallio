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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gallio.Properties;

namespace Gallio.Common.Collections
{
    /// <summary>
    /// Utility functions for manipulating collections.
    /// </summary>
    public static class CollectionUtils
    {
        /// <summary>
        /// Converts all elements of the input collection and returns the collected results as an array
        /// of the same size.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="TOutput">The output type.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="converter">The conversion function to apply to each element.</param>
        /// <returns>The output array.</returns>
        public static TOutput[] ConvertAllToArray<TInput, TOutput>(ICollection input,
            Converter<TInput, TOutput> converter)
        {
            TOutput[] array = new TOutput[input.Count];

            int i = 0;
            foreach (object value in input)
                array[i++] = converter((TInput)value);

            return array;
        }

        /// <summary>
        /// Copies all of the elements of the input collection to an array.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The input collection.</param>
        /// <returns>The output array.</returns>
        public static T[] ToArray<T>(ICollection collection)
        {
            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Returns the first element of the input enumeration for which the specified
        /// predicate returns true.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The input enumeration.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The first matching value or the default for the type if not found.</returns>
        public static T Find<T>(IEnumerable enumeration, Predicate<T> predicate)
        {
            foreach (T value in enumeration)
                if (predicate(value))
                    return value;

            return default(T);
        }        

        /// <summary>
        /// Returns an array of the specified length, filled with the specifed constant value.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="value">The constant value.</param>
        /// <param name="length">The length of the array.</param>
        /// <returns>The resulting array.</returns>
        public static T[] ConstantArray<T>(T value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", Resources.ExceptionDescription_ZeroOrGreater);

            var array = new T[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = value;
            }

            return array;
        }
    }
}