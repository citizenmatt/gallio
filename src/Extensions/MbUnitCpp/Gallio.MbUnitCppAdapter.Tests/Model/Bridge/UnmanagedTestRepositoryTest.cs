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
// WITHOUT WC:\Projects\Gallio\v3\src\Extensions\MbUnitCpp\Gallio.MbUnitCppAdapter.Tests\Model\Bridge\UnmanagedTestRepositoryTest.csARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Framework;
using MbUnit.Framework;
using System.Runtime.InteropServices;

namespace Gallio.MbUnitCppAdapter.Tests.Model.Bridge
{
    [TestFixture]
    [TestsOn(typeof(UnmanagedTestRepository))]
    public class UnmanagedTestRepositoryTest
    {
        private readonly string resources = Helper.GetTestResources();

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_fullName_should_throw_exception()
        {
            new UnmanagedTestRepository(null);
        }

        [Test, Explicit]
        public void GetTests()
        {
            using (var repository = new UnmanagedTestRepository(resources))
            {
                Assert.IsTrue(repository.IsValid);
                TestInfoData[] items = repository.GetTests().ToArray();
                Assert.IsNotEmpty(items);

                using (TestLog.BeginSection(String.Format("Found {0} test item(s):", items.Length)))
                {
                    foreach (TestInfoData item in items)
                    {
                        DiagnosticLog.WriteLine("{0}: Fixture={1}, Test={2}, Row={3}, Kind={4}", 
                            item.Name, item.Native.Position.pTestFixture, item.Native.Position.pTest, 
                            item.Native.Position.pRow, item.Kind);
                    }
                }
            }
        }
    }
}
