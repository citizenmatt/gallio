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
using Gallio.Framework.Data;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test builder applies contributions to a test under construction.
    /// </summary>
    public interface ITestParameterBuilder : ITestComponentBuilder
    {
        /// <summary>
        /// Gets the test data context builder for the test parameter.
        /// </summary>
        ITestDataContextBuilder TestDataContextBuilder { get; }

        /// <summary>
        /// Gets the set of actions that describe the behavior of the test parameter.
        /// </summary>
        PatternTestParameterActions TestParameterActions { get; }

        /// <summary>
        /// Gets or sets the data binder for the parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is a <see cref="ScalarDataBinder" /> bound to the anonymous
        /// data source using a <see cref="DataBinding"/> whose path is the name of this parameter and whose
        /// index is the implicit index computed by the pararameter's data context.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        IDataBinder Binder { get; set; }

        /// <summary>
        /// Gets the underlying test parameter.
        /// </summary>
        /// <returns>The underlying test parameter.</returns>
        PatternTestParameter ToTestParameter();
    }
}
