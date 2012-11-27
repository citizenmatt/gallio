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
using NUnit.Framework;

#if NUNIT248
using NUnit.Framework.Extensions;
#endif

namespace Gallio.NUnitAdapter.TestResources
{
#if NUNIT248
    /// <summary>
    /// Verifies that NUnit addins are registered properly.
    /// </summary>
    /// <remarks>
    /// The idea behind this test is to actually try to use an NUnit addin such as
    /// the Row extension for NUnit v2.4.8.
    /// </remarks>
    [TestFixture]
    public class AddinsTest
    {
        [RowTest]
        [Row(1, 2, 3, Description="Pass")]
        [Row(2, 2, 5, Description="Fail")]
        public void RowTest(int x, int y, int z)
        {
            Assert.AreEqual(z, x + y);
        }
    }
#endif
}
