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
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Common.Diagnostics;
using Action=Gallio.Common.Action;
using System.Reflection;
using System.IO;

namespace MbUnit.Tests.Framework
{
    public abstract class BaseAssertTest
    {
        [SystemInternal]
        public static AssertionFailure[] Capture(Action action)
        {
            AssertionFailure[] failures = AssertionHelper.Eval(action);

            if (failures.Length != 0)
            {
                using (TestLog.BeginSection("Captured Assertion Failures"))
                {
                    foreach (AssertionFailure failure in failures)
                        failure.WriteTo(TestLog.Default);
                }
            }

            return failures;
        }

        public sealed class NonGenericCompare : IComparable
        {
            public int CompareTo(object obj)
            {
                return 1;
            }
        }

        public sealed class TestComparer : StringComparer
        {
            public bool EqualsReturn;
            public int CompareReturn;

            public TestComparer()
            {
                EqualsReturn = false;
                CompareReturn = 0;
            }

            public override int Compare(string x, string y)
            {
                return CompareReturn;
            }

            public override bool Equals(string x, string y)
            {
                return EqualsReturn;
            }

            public override int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        protected string GetTextResource(string resourceName)
        {
            using (var textReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)))
            {
                return textReader.ReadToEnd();
            }
        }
    }
}
