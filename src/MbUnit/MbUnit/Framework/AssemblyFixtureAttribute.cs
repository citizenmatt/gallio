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
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that a class contains assembly-level setup and teardown methods.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The assembly fixture attribute is applied to a class that contains setup and
    /// teardown methods that are to be applied at the assembly level.  Conceptually,
    /// the <see cref="AssemblyFixtureAttribute" /> adds new behavior to an assembly-level
    /// test fixture that contains all of the test fixtures within the assembly.
    /// </para>
    /// <para>
    /// The following attributes are typically used within an assembly fixture:
    /// <list type="bullet">
    /// <item><see cref="FixtureSetUpAttribute" />: Performs setup activities before any
    /// test fixtures within the assembly are executed.</item>
    /// <item><see cref="FixtureTearDownAttribute" />: Performs teardown activities after all
    /// test fixtures within the assembly are executed.</item>
    /// <item><see cref="SetUpAttribute" />: Performs setup activities before each
    /// test fixture within the assembly is executed.</item>
    /// <item><see cref="TearDownAttribute" />: Performs teardown activities after each
    /// test fixture within the assembly is executed.</item>
    /// </list>
    /// </para>
    /// <para>
    /// It is also possible to use other attributes as with an ordinary <see cref="TestFixtureAttribute" />.
    /// An assembly fixture also supports data binding.  When data binding is used on an assembly
    /// fixture, it will cause all test fixtures within the assembly to run once for each combination
    /// of data values used.
    /// </para>
    /// <para>
    /// The class must have a public default constructor. The class may not be static.
    /// </para>
    /// <para>
    /// There must only be at most one class with an <see cref="AssemblyFixtureAttribute" />
    /// within any given assembly.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class AssemblyFixtureAttribute : TestTypePatternAttribute
    {
        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            //TODO: Review: Issue 762: Shouldn't the base method be invoked here?
            //base.Consume(containingScope, codeElement, skipChildren);
            var type = codeElement as ITypeInfo;
            Validate(containingScope, type);

            ITestBuilder assemblyTest = containingScope.TestBuilder;
            InitializeTest(containingScope, type);
            SetTestSemantics(assemblyTest, type);
        }

        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, ITypeInfo type)
        {
            base.Validate(containingScope, type);

            if (type.IsNested)
                ThrowUsageErrorException("This attribute can only be used on a non-nested class.");
        }

        /// <inheritdoc />
        protected override void SetTestSemantics(ITestBuilder testBuilder, ITypeInfo type)
        {
            testBuilder.TestActions.TestInstanceActions.BeforeTestInstanceChain.After(
                delegate(PatternTestInstanceState testInstanceState)
                {
                    if (testInstanceState.FixtureType != null)
                        ThrowUsageErrorException("There appears to already be a fixture defined for the assembly.");
                });

            base.SetTestSemantics(testBuilder, type);
        }
    }
}
