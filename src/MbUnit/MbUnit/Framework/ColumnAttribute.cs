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
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides a column of literal values as a data source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is a compact equivalent to providing a sequence of values
    /// using <see cref="RowAttribute" /> with 1 element in each.  It is particularly
    /// useful for specifying combinatorial tests.
    /// </para>
    /// <para>
    /// By default, the column provided by the column data source is unnamed.
    /// Use <see cref="HeaderAttribute" /> to provide an explicit name for the column.
    /// </para>
    /// </remarks>
    /// <example><![CDATA[
    /// [Test]
    /// public void MyTest([Column(1, 2, 3)] int x, [Column("a", "b")] string y, [Column(0.1, 0.2)] double z)
    /// {
    ///     // This test will run 3 * 2 * 2 = 12 times with all combinations of
    ///     // the values specified in the column for each parameter.  This test
    ///     // is combinatorial because the values are assigned to each parameter
    ///     // separately.
    /// }
    /// ]]></example>
    /// <seealso cref="RowAttribute"/>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public class ColumnAttribute : DataAttribute
    {
        private readonly object[] values;

        /// <summary>
        /// Adds a column of literal values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// There exist two ambiguities in the use of this attribute that
        /// result from how the C# compiler handles functions that accept
        /// a variable number of arguments.
        /// </para>
        /// <para>
        /// Case 1: If there is only 1 argument and its value is <c>null</c>, the
        /// compiler will pass a <c>null</c> array reference to the attribute constructor.
        /// Since the value array cannot be null, the attribute will assume that
        /// you meant to create an array consiting of a single <c>null</c>.
        /// </para>
        /// <code><![CDATA[
        /// // Example of case #1.
        /// // The attribute will assume that you intended to pass in a single null value.
        /// [Test]
        /// [Column(null)]
        /// public void Test(object value)
        /// {
        ///     Assert.IsNull(value);
        /// }
        /// ]]></code>
        /// <para>
        /// Case 2: If there is only 1 argument and its value is an object array, the
        /// compiler will pass the array itself as the argument values.  Unfortunately,
        /// the attribute constructor cannot distinguish this case from the usual case
        /// when multiple arguments are present.  So you need to disambiguate this case
        /// explicitly.
        /// </para>
        /// <code><![CDATA[
        /// // Example of case #2.
        /// // The attribute will treat both of the following declarations equivalently
        /// // contrary to what we probably intend.
        /// [Column(new object[] { 1, 2, 3 })]
        /// [Column(1, 2, 3)]
        /// ]]></code>
        /// <para>
        /// To fix this case, you must provide explicit disambiguation.
        /// (We cannot do it automatically based on the number of method parameters provided
        /// because the row attribute can be applied to other elements besides methods and
        /// its contents might be consumed in other ways.)
        /// </para>
        /// <code><![CDATA[
        /// // Example of case #2, disambiguated to define a column that contains only an array of values.
        /// [Test]
        /// [Column(new object[] { new object[] { 1, 2, 3 })]
        /// public void Test(object[] values)
        /// {
        ///     ArrayAssert.AreElementsEqual(new object[] { 1, 2, 3 }, values);
        /// }
        /// ]]></code>
        /// </remarks>
        /// <param name="values">The array of values in the column.</param>
        [CLSCompliant(false)]
        public ColumnAttribute(params object[] values)
        {
            this.values = values ?? new object[] { null };
        }

        /// <summary>
        /// Gets the array of values in the column.
        /// </summary>
        public object[] Values
        {
            get { return values; }
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            dataSource.AddDataSet(new ValueSequenceDataSet(values, GetMetadata(), false));
        }
    }
}