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
using System.Collections;
using Gallio.Framework.Data.Generation;

namespace MbUnit.Framework
{
    /// <summary>
    /// Helper methods to quickly combine and generate random or constrained values for data-driven tests.
    /// </summary>
    public static partial class DataGenerators
    {
        /// <summary>
        /// Generates random values.
        /// </summary>
        public static class Random
        {
            /// <summary>
            /// Returns the an enumeration of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Numbers(int count, decimal minimum, decimal maximum)
            {
                var generator = new RandomDecimalGenerator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                };

                foreach (decimal value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns the an enumeration of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<double> Numbers(int count, double minimum, double maximum)
            {
                var generator = new RandomDoubleGenerator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                };

                foreach (double value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns the an enumeration of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<int> Numbers(int count, int minimum, int maximum)
            {
                var generator = new RandomInt32Generator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                };

                foreach (int value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns the an enumeration of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <param name="seed">A seed value to initialize the random number generator.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<decimal> Numbers(int count, decimal minimum, decimal maximum, int seed)
            {
                var generator = new RandomDecimalGenerator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                    Seed = seed,
                };

                foreach (decimal value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns the an enumeration of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <param name="seed">A seed value to initialize the random number generator.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<double> Numbers(int count, double minimum, double maximum, int seed)
            {
                var generator = new RandomDoubleGenerator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                    Seed = seed,
                };

                foreach (double value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns the an enumeration of random numbers.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="minimum">The lower bound of the range.</param>
            /// <param name="maximum">The upper bound of the range.</param>
            /// <param name="seed">A seed value to initialize the random number generator.</param>
            /// <returns>An enumeration of random number values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<int> Numbers(int count, int minimum, int maximum, int seed)
            {
                var generator = new RandomInt32Generator
                {
                    Count = count,
                    Minimum = minimum,
                    Maximum = maximum,
                    Seed = seed,
                };

                foreach (int value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns an enumeration of random strings based on a regular expression filter.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="regularExpressionPattern">The regular expression filter.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Strings(int count, string regularExpressionPattern)
            {
                var generator = new RandomRegexLiteStringGenerator
                {
                    Count = count,
                    RegularExpressionPattern = regularExpressionPattern,
                };

                foreach (string value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns an enumeration of random strings from a pre-existing stock of values.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="stock">A stock of preset values.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Strings(int count, RandomStringStock stock)
            {
                var generator = new RandomStockStringGenerator
                {
                    Count = count,
                    Values = RandomStringStockInfo.FromStock(stock).GetItems(),
                };

                foreach (string value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns an enumeration of random strings based on a regular expression filter.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="regularExpressionPattern">The regular expression filter.</param>
            /// <param name="seed">A seed value to initialize the random number generator.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Strings(int count, string regularExpressionPattern, int seed)
            {
                var generator = new RandomRegexLiteStringGenerator
                {
                    Count = count,
                    RegularExpressionPattern = regularExpressionPattern,
                    Seed = seed,
                };

                foreach (string value in generator.Run())
                    yield return value;
            }

            /// <summary>
            /// Returns an enumeration of random strings from a pre-existing stock of values.
            /// </summary>
            /// <param name="count">The number of strings to generate.</param>
            /// <param name="stock">A stock of preset values.</param>
            /// <param name="seed">A seed value to initialize the random number generator.</param>
            /// <returns>An enumeration of random string values.</returns>
            /// <exception cref="GenerationException">Thrown if the specified parameters are inconsistent or invalid.</exception>
            public static IEnumerable<string> Strings(int count, RandomStringStock stock, int seed)
            {
                var generator = new RandomStockStringGenerator
                {
                    Count = count,
                    Values = RandomStringStockInfo.FromStock(stock).GetItems(),
                    Seed = seed,
                };

                foreach (string value in generator.Run())
                    yield return value;
            }
        }
    }
}
