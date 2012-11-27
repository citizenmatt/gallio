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
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// An abstract base class for MbUnit attributes that contribute values to data sources
    /// along with metadata such a description or expected exception type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You may define a new data source by creating a subclass of this attribute
    /// and overriding <see cref="DataPatternAttribute.PopulateDataSource" />.
    /// </para>
    /// </remarks>
    /// <seealso cref="DataPatternAttribute"/> for more information about data binding attributes in general.
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public abstract class DataAttribute : DataPatternAttribute
    {
        /// <summary>
        /// Gets or sets a description of the values provided by the data source.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of exception that should be thrown when the
        /// values provided by the data source are consumed by test.
        /// </summary>
        public Type ExpectedException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message of the exception that should be thrown when the
        /// values provided by the data source are consumed by test.
        /// May be a substring of the actual exception message.
        /// </summary>
        public string ExpectedExceptionMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of inner exception that should be set in an
        /// exception thrown when the values provided by the data source are consumed
        /// by test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value is ignored if the <see cref="ExpectedException"/> property is not set.
        /// </para>
        /// </remarks>
        public Type ExpectedInnerException
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the metadata for the data source.
        /// </summary>
        /// <returns>The metadata keys and values.</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            if (Description != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.Description, Description);
            if (ExpectedException != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.ExpectedException, ExpectedException.FullName);
            if (ExpectedExceptionMessage != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.ExpectedExceptionMessage, ExpectedExceptionMessage);
            if (ExpectedInnerException != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.ExpectedInnerException, ExpectedInnerException.FullName);
        }
    }
}
