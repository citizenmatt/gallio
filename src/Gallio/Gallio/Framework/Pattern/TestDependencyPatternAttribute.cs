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
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Model.Tree;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A dependency pattern attribute creates a dependency on the tests defined
    /// by some other code element.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public abstract class TestDependencyPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            //TODO: Review: Issue 762: Shouldn't the base method be invoked here?
            //base.Process(scope, codeElement);
            Validate(scope, codeElement);

            ICodeElementInfo resolvedDependency = GetDependency(scope, codeElement);
            scope.TestModelBuilder.AddDeferredAction(codeElement, int.MaxValue, delegate
            {
                bool success = false;
                foreach (PatternTest dependentTest in scope.Evaluator.GetDeclaredTests(resolvedDependency))
                {
                    scope.TestBuilder.AddDependency(dependentTest);
                    success = true;
                }

                if (! success)
                    scope.TestModelBuilder.AddAnnotation(new Annotation(AnnotationType.Warning, codeElement, "Was unable to resolve a test dependency."));
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
        /// Gets the code element that declares the tests on which this test should depend.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="codeElement">The code element.</param>
        /// <returns>The code element representing the dependency.</returns>
        protected abstract ICodeElementInfo GetDependency(IPatternScope scope, ICodeElementInfo codeElement);
    }
}
