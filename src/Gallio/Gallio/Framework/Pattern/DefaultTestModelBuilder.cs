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
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.Runtime;
using Gallio.Runtime.Loader;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Default implementation of a test model builder.
    /// </summary>
    public class DefaultTestModelBuilder : BaseBuilder, ITestModelBuilder
    {
        private readonly IReflectionPolicy reflectionPolicy;
        private readonly PatternTestModel testModel;
        private readonly ITestBuilder rootTestBuilder;

        /// <summary>
        /// Creates a test model builder.
        /// </summary>
        /// <param name="reflectionPolicy">The reflection policy.</param>
        /// <param name="testModel">The underlying test model.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="reflectionPolicy"/>
        /// or <paramref name="testModel"/> is null.</exception>
        public DefaultTestModelBuilder(IReflectionPolicy reflectionPolicy, PatternTestModel testModel)
        {
            if (reflectionPolicy == null)
                throw new ArgumentNullException("reflectionPolicy");
            if (testModel == null)
                throw new ArgumentNullException("testModel");

            this.reflectionPolicy = reflectionPolicy;
            this.testModel = testModel;

            rootTestBuilder = new DefaultTestBuilder(this, testModel.RootTest);
        }

        /// <inheritdoc />
        public IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        /// <inheritdoc />
        public ITestBuilder RootTestBuilder
        {
            get { return rootTestBuilder; }
        }

        /// <inheritdoc />
        public void AddAnnotation(Annotation annotation)
        {
            if (annotation == null)
                throw new ArgumentNullException("annotation");

            testModel.AddAnnotation(annotation);
        }

        /// <inheritdoc />
        public void AddAssemblyResolver(IAssemblyResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            RuntimeAccessor.AssemblyLoader.AddAssemblyResolver(resolver);
        }

        /// <inheritdoc />
        public void PublishExceptionAsAnnotation(ICodeElementInfo codeElement, Exception ex)
        {
            if (ex is PatternUsageErrorException)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, codeElement,
                    ex.Message, ex.InnerException));
            }
            else
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, codeElement,
                    "An exception was thrown while exploring tests.", ex));
            }
        }

        /// <inheritdoc />
        public PatternTestModel ToTestModel()
        {
            return testModel;
        }

        /// <inheritdoc />
        protected sealed override ITestModelBuilder GetTestModelBuilder()
        {
            return this;
        }
    }
}
