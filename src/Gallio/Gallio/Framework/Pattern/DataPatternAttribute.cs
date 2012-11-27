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
using Gallio.Framework.Data;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The data pattern attribute applies a data source to a fixture or test
    /// parameter declaratively.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It can be attached to a fixture class, a public property
    /// or field of a fixture, a test method or a test method parameter.  When attached
    /// to a property or field of a fixture, implies that the property or field is
    /// a fixture parameter (so the <see cref="TestParameterPatternAttribute" />
    /// may be omitted).
    /// </para>
    /// <para>
    /// The order in which items contributed by a data pattern attribute are
    /// use can be controlled via the <see cref="DecoratorPatternAttribute.Order" />
    /// property.  The contents of data sets with lower order indices are processed
    /// before those with higher indices.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example ensures that the rows are processed in exactly the order they appear.
    /// <code><![CDATA[
    /// [TestFixture]
    /// public class Fixture
    /// {
    ///     [Test]
    ///     [Row(1, "a"), Order=1)]
    ///     [Row(2, "b"), Order=2)]
    ///     [Row(3, "c"), Order=3)]
    ///     public void Test(int x, string y) 
    ///     { 
    ///         // Code logic here...
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple=true, Inherited=true)]
    public abstract class DataPatternAttribute : DecoratorPatternAttribute
    {
        private string sourceName = "";

        /// <summary>
        /// Gets or sets the name of the data source to create so that the values produced
        /// by this attribute can be referred to elsewhere.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Multiple data attributes may use the same data source name to produce a compound
        /// data source consisting of all of their values combined.
        /// </para>
        /// <para>
        /// If no name is given to the data source (or it is an empty string), the data source
        /// is considered anonymous.  An anonymous data source is only visible
        /// within the scope of the code element with which the data source declaration
        /// is associated.  By default, test parameters are bound to
        /// the anonymous data source of their enclosing scope.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                sourceName = value;
            }
        }

        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            //TODO: Review: Issue 762: Shouldn't the base method be invoked here?
            //base.Process(scope, codeElement);
            Validate(scope, codeElement);

            scope.TestComponentBuilder.AddDeferredAction(codeElement, Order, delegate
            {
                PopulateDataSource(scope, scope.TestDataContextBuilder.DefineDataSource(sourceName), codeElement);
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
        }

        /// <summary>
        /// Populates the data source with the contributions of this attribute.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="codeElement">The code element.</param>
        protected virtual void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
        }
    }
}