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
using System.Linq;
using System.Text;
using Gallio.Common.Xml;
using Gallio.Common.Xml.Diffing;
using Gallio.Tests;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Xml.Diffing
{
    public abstract class DiffableTestBase
    {
        protected void AssertDiff(DiffSet actual, params Diff[] expected)
        {
            Assert.AreEqual(expected.Length == 0, actual.IsEmpty);
            Assert.AreElementsEqualIgnoringOrder(expected, actual,
                new StructuralEqualityComparer<Diff>
                {
                    x => x.Type,
                    x => x.Path.ToString(),
                });
        }
    }
}
