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
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(TestLog))]
    public class TestLogTest
    {
        [Test]
        public void LogContainsStaticVersionsOfLogWriterDeclaredMethods()
        {
            AssertContainsStaticVersionsOfDeclaredMethods(typeof(MarkupDocumentWriter),
                "get_Item", "Close", "get_IsClosed");
        }

        [Test]
        public void LogContainsStaticVersionsOfLogStreamWriterDeclaredMethods()
        {
            AssertContainsStaticVersionsOfDeclaredMethods(typeof(MarkupStreamWriter),
                "get_StreamName", "get_Encoding", "get_NewLine", "set_NewLine", "Flush", "Close", "get_Container");
        }

        private void AssertContainsStaticVersionsOfDeclaredMethods(Type sourceType, params string[] excludedMethodNames)
        {
            foreach (MethodInfo sourceMethod in sourceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (Array.IndexOf(excludedMethodNames, sourceMethod.Name) >= 0)
                    continue;

                Type[] parameterTypes = GenericCollectionUtils.ConvertAllToArray<ParameterInfo, Type>(sourceMethod.GetParameters(), delegate(ParameterInfo parameter)
                {
                    return parameter.ParameterType;
                });

                MethodInfo targetMethod = typeof(TestLog).GetMethod(sourceMethod.Name, BindingFlags.Static | BindingFlags.Public,
                    null, parameterTypes, null);

                Assert.IsNotNull(targetMethod, "Log is missing a static method '{0}({1})' corresponding to those defined by type {2}",
                    sourceMethod.Name,
                    string.Join(", ", Array.ConvertAll<Type, string>(parameterTypes, delegate(Type type) { return type.Name; })),
                    sourceType.FullName);

                Assert.AreEqual(sourceMethod.ReturnType, targetMethod.ReturnType);

                TestLog.WriteLine("Found method '{0}'", sourceMethod.Name);
            }
        }

        [Test]
        public void MethodName()
        {
            TestLog.WriteLine("Hello");
        }
    }
}
