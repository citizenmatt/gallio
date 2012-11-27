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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test decorator pattern attribute applies decorations to a test defined by an
    /// assembly, type, or method.
    /// </summary>
    /// <seealso cref="TestTypePatternAttribute"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public abstract class TestDecoratorPatternAttribute : DecoratorPatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            //TODO: Review: Issue 762: Shouldn't the base method be invoked here?
            //base.Process(scope, codeElement);
            Validate(scope, codeElement);

            scope.TestBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                DecorateTest(scope, codeElement);
            });
        }

        /// <summary>
        /// Verifies that the attribute is being used correctly.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="codeElement">The code element.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if the attribute is being used incorrectly.</exception>
        protected virtual void Validate(IPatternScope scope, ICodeElementInfo codeElement)
        {
            if (!scope.IsTestDeclaration)
                ThrowUsageErrorException("This attribute can only be used on a test.");
        }

        /// <summary>
        /// Applies decorations to a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A typical use of this method is to augment the test with additional metadata
        /// or to add additional behaviors to the test.
        /// </para>
        /// </remarks>
        /// <param name="scope">The scope.</param>
        /// <param name="codeElement">The code element.</param>
        protected virtual void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
        }
    }
}