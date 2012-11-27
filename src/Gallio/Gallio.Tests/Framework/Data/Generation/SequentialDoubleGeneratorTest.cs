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
using System.Linq;
using System.Text;
using Gallio.Common;
using Gallio.Framework.Data.Generation;
using MbUnit.Framework;
using Gallio.Framework;
using System.Collections;

namespace Gallio.Tests.Framework.Data.Generation
{
    [TestFixture]
    [TestsOn(typeof(SequentialDoubleGenerator))]
    public class SequentialDoubleGeneratorTest
    {
        private readonly EqualityComparison<double> approximative = (x, y) => Math.Abs(x - y) < 0.01;

        [Test]
        [Row(0, 0, 0, new double[] { })]
        [Row(0, 0, 1, new[] { 0 })]
        [Row(1, 1, 1, new[] { 1 })]
        [Row(1, 1, 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, -1, 10, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 2.26, 6, new[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_step_count_sequence_ok(double start, double step, int count, double[] expectedOutput)
        {
            var generator = new SequentialDoubleGenerator
            {
                Start = start,
                Step = step,
                Count = count
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, approximative);
        }

        [Test]
        [Row(0, 0, 0, new double[] { })]
        [Row(0, 0, 1, new[] { 0 })]
        [Row(1, 1, 1, new[] { 1 })]
        [Row(1, 10, 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, 1, 10, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 7.8, 6, new[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_stop_count_sequence_ok(double start, double stop, int count, double[] expectedOutput)
        {
            var generator = new SequentialDoubleGenerator
            {
                Start = start,
                End = stop,
                Count = count
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, approximative);
        }

        [Test]
        [Row(0, 0, 1, new[] { 0 })]
        [Row(1, 1, 1, new[] { 1 })]
        [Row(1, 10, 1, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, 1, -1, new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 7.8, 2.26, new[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_stop_step_sequence_ok(double start, double stop, double step, double[] expectedOutput)
        {
            var generator = new SequentialDoubleGenerator
            {
                Start = start,
                End = stop,
                Step = step
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, approximative);
        }

        private IEnumerable<object[]> GetInvalidProperyValues()
        {
            yield return new object[] { null, null, null, null };
            yield return new object[] { 1, 10, 1, 10 };
            yield return new object[] { 1, 10, -1, null };
            yield return new object[] { 1, 10, 0, null };
            yield return new object[] { 1, 10, null, -1 };
            yield return new object[] { Double.MaxValue, 10, null, 10 };
            yield return new object[] { Double.MinValue, 10, null, 10 };
            yield return new object[] { Double.PositiveInfinity, 10, null, 10 };
            yield return new object[] { Double.NegativeInfinity, 10, null, 10 };
            yield return new object[] { Double.NaN, 10, null, 10 };
            yield return new object[] { 1, Double.MaxValue, null, 10 };
            yield return new object[] { 1, Double.MinValue, null, 10 };
            yield return new object[] { 1, Double.PositiveInfinity, null, 10 };
            yield return new object[] { 1, Double.NegativeInfinity, null, 10 };
            yield return new object[] { 1, Double.NaN, null, 10 };
            yield return new object[] { 1, 10, Double.MaxValue, null };
            yield return new object[] { 1, 10, Double.MinValue, null };
            yield return new object[] { 1, 10, Double.PositiveInfinity, null };
            yield return new object[] { 1, 10, Double.NegativeInfinity, null };
            yield return new object[] { 1, 10, Double.NaN, null };
            yield return new object[] { 1, 10, null, Int32.MinValue };
            yield return new object[] { 1, 10, null, Int32.MaxValue };
        }

        [Test, Factory("GetInvalidProperyValues")]
        public void Generate_sequence_with_invalid_property_settings_should_throw_exception(double? start, double? stop, double? step, int? count)
        {
            var generator = new SequentialDoubleGenerator
            {
                Start = start,
                End = stop,
                Step = step,
                Count = count
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<double>().ToArray());
        }

        [Test]
        public void Generate_filtered_sequence()
        {
            var generator = new SequentialDoubleGenerator
            {
                Start = 0,
                End = 100,
                Step = 1,
                Filter = IsPrime
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(new double[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 }, actualOutput);
        }

        public static bool IsPrime(double number)
        {
            // Simple implementation of the "Sieve of Eratosthenes" algorithm, 
            // that checks for the primality of a number 
            // (http://en.wikipedia.org/wiki/Sieve_of_Eratosthenes)
            int n = (int)Convert.ChangeType(number, typeof(int));
            var array = new BitArray(n + 1, true);

            for (int i = 2; i < n + 1; i++)
            {
                if (array[i])
                {
                    for (int j = i * 2; j < n + 1; j += i)
                    {
                        array[j] = false;
                    }

                    if (array[i] && (n == i))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
