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

namespace Gallio.Common.Collections
{
    /// <summary>
    /// Utility functions for manipulating generic collections.
    /// </summary>
    public static class GenericCollectionUtils
    {
        /// <summary>
        /// Converts each element of the input collection and stores the result in place.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="list">The list to mutate.</param>
        /// <param name="converter">The conversion function to apply to each element.</param>
        public static void ConvertInPlace<T>(IList<T> list, Converter<T, T> converter)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = converter(list[i]);
        }

        /// <summary>
        /// Converts each element of the input collection and stores the result in the
        /// output list using the same index.  The output list must be at least as
        /// large as the input list.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="TOutput">The output type.</typeparam>
        /// <param name="input">The input list.</param>
        /// <param name="output">The output list.</param>
        /// <param name="converter">The conversion function to apply to each element.</param>
        public static void ConvertAndCopyAll<TInput, TOutput>(ICollection<TInput> input, IList<TOutput> output,
            Converter<TInput, TOutput> converter)
        {
            int i = 0;
            foreach (TInput value in input)
                output[i++] = converter(value);
        }

        /// <summary>
        /// Converts each element of the input collection and adds the result to the
        /// output collection succession in the same order.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="TOutput">The output type.</typeparam>
        /// <param name="input">The input list.</param>
        /// <param name="output">The output list.</param>
        /// <param name="converter">The conversion function to apply to each element.</param>
        public static void ConvertAndAddAll<TInput, TOutput>(ICollection<TInput> input, ICollection<TOutput> output,
            Converter<TInput, TOutput> converter)
        {
            foreach (TInput value in input)
                output.Add(converter(value));
        }

        /// <summary>
        /// Converts each element of the input collection and returns the collected results as an array
        /// of the same size.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="TOutput">The output type.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="converter">The conversion function to apply to each element.</param>
        /// <returns>The output array.</returns>
        public static TOutput[] ConvertAllToArray<TInput, TOutput>(ICollection<TInput> input,
            Converter<TInput, TOutput> converter)
        {
            TOutput[] output = new TOutput[input.Count];
            ConvertAndCopyAll(input, output, converter);
            return output;
        }

        /// <summary>
        /// Copies all of the elements of the input enumerable to an array.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumerable">The input enumerable.</param>
        /// <returns>The output array.</returns>
        public static T[] ToArray<T>(IEnumerable<T> enumerable)
        {
            return new List<T>(enumerable).ToArray();
        }

        /// <summary>
        /// Copies all of the elements of the input collection to an array.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="collection">The input collection.</param>
        /// <returns>The output array.</returns>
        public static T[] ToArray<T>(ICollection<T> collection)
        {
            T[] output = new T[collection.Count];

            int i = 0;
            foreach (T value in collection)
                output[i++] = value;

            return output;
        }

        /// <summary>
        /// Returns the first element of the input enumeration for which the specified
        /// predicate returns true.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The input enumeration.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The first matching value or the default for the type if not found.</returns>
        public static T Find<T>(IEnumerable<T> enumeration, Predicate<T> predicate)
        {
            foreach (T value in enumeration)
                if (predicate(value))
                    return value;

            return default(T);
        }

        /// <summary>
        /// Returns true if the input enumeration contains an element for which the predicate returns true.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The input enumeration.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>True if a matching element exists.</returns>
        public static bool Exists<T>(IEnumerable<T> enumeration, Predicate<T> predicate)
        {
            foreach (T value in enumeration)
                if (predicate(value))
                    return true;

            return false;
        }

        /// <summary>
        /// Performs an action for each element in an enumeration.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The input enumeration.</param>
        /// <param name="action">The action to perform.</param>
        public static void ForEach<T>(IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T value in enumeration)
                action(value);
        }

        /// <summary>
        /// Adds all elements of the input enumeration to the output collection.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="input">The input enumeration.</param>
        /// <param name="output">The output collection.</param>
        public static void AddAll<T>(IEnumerable<T> input, ICollection<T> output)
        {
            foreach (T value in input)
                output.Add(value);
        }

        /// <summary>
        /// Adds elements of the input enumeration to the output collection, 
        /// if not already present.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="input">The input enumeration.</param>
        /// <param name="output">The output collection.</param>
        public static void AddAllIfNotAlreadyPresent<T>(IEnumerable<T> input, ICollection<T> output)
        {
            foreach (T value in input)
                if (!output.Contains(value))
                    output.Add(value);
        }

        /// <summary>
        /// Returns true if the elements of both lists are equal.
        /// </summary>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection.</param>
        /// <returns>True if the elements are equal.</returns>
        public static bool ElementsEqual<T>(IList<T> a, IList<T> b)
        {
            return ElementsEqual(a, b, (x, y) => Equals(x, y));
        }

        /// <summary>
        /// Returns true if the elements of both lists are equal.
        /// </summary>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection.</param>
        /// <param name="comparer">The comparison strategy to use.</param>
        /// <returns>True if the elements are equal.</returns>
        public static bool ElementsEqual<T>(IList<T> a, IList<T> b, EqualityComparison<T> comparer)
        {
            int count = a.Count;
            if (count != b.Count)
                return false;

            for (int i = 0; i < count; i++)
            {
                if (!comparer(a[i], b[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if both dictionaries have equal key/value pairs.
        /// </summary>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection.</param>
        /// <returns>True if the elements are equal.</returns>
        public static bool KeyValuePairsEqual<TKey, TValue>(IDictionary<TKey, TValue> a, IDictionary<TKey, TValue> b)
        {
            if (a.Count != b.Count)
                return false;

            foreach (KeyValuePair<TKey, TValue> entry in a)
            {
                TValue value;
                if (!b.TryGetValue(entry.Key, out value) || !Equals(entry.Value, value))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the elements of both lists are equal but possibly appear in a different order.
        /// Handles elements that appear multiple times and ensures that they appear the same
        /// number of times in each list.
        /// </summary>
        /// <param name="a">The first collection.</param>
        /// <param name="b">The second collection.</param>
        /// <returns>True if the elements are equal.</returns>
        public static bool ElementsEqualOrderIndependent<T>(IList<T> a, IList<T> b)
        {
            int count = a.Count;
            if (count != b.Count)
                return false;

            var ca = new Dictionary<T, int>();
            var cb = new Dictionary<T, int>();

            foreach (T ea in a)
                IncrementCounter(ea, ca);
            foreach (T eb in b)
                IncrementCounter(eb, cb);

            return KeyValuePairsEqual(ca, cb);
        }

        private static void IncrementCounter<T>(T elem, Dictionary<T, int> table)
        {
            int counter;
            table.TryGetValue(elem, out counter);
            counter += 1;
            table[elem] = counter;
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TInput">The type of the elements of the input sequence.</typeparam>
        /// <typeparam name="TOutput">The type of the elements of the output sequence.</typeparam>
        /// <param name="enumeration">The input sequence.</param>
        /// <param name="filter"></param>
        /// <returns>The output sequence</returns>
        /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
        public static IEnumerable<TOutput> Select<TInput, TOutput>(IEnumerable<TInput> enumeration, Func<TInput, TOutput> filter)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration");
            if (filter == null)
                throw new ArgumentNullException("filter");

            foreach (TInput value in enumeration)
                yield return filter(value);
        }
    }
}