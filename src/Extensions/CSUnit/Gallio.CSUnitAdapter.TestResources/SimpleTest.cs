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

using csUnit;

namespace Gallio.CSUnitAdapter.TestResources
{
    /// <summary>
    /// A simple test fixture.
    /// </summary>
    [TestFixture]
    public class SimpleTest
    {
        /// <summary>
        /// A passing test.
        /// </summary>
        [Test]
        public void Pass()
        {
            Assert.True(1 == 1.0);
        }

        /// <summary>
        /// A failing test.
        /// </summary>
        [Test]
        public void Fail()
        {
            Assert.Fail("Boom");
        }
   }
}
