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
using Gallio.Runtime.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Conversions
{
    [TestFixture]
    [TestsOn(typeof(ArrayToArrayConversionRule))]
    public class ArrayToArrayConversionRuleTest : BaseConversionRuleTest<ArrayToArrayConversionRule>
    {
        [Test]
        public void TransitiveConversion()
        {
            int[] sourceValue = new int[] { 1, 2, 3 };
            string[] targetValue = (string[])Converter.Convert(sourceValue, typeof(string[]));

            Assert.AreElementsEqual(new string[] { "1", "2", "3" }, targetValue);
        }

        [Test]
        public void UnsupportedConversion()
        {
            Assert.IsFalse(Converter.CanConvert(typeof(int[]), typeof(int)));
            Assert.IsFalse(Converter.CanConvert(typeof(int), typeof(int[])));
        }
    }
}
