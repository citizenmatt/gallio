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
using System.Runtime.Serialization;

namespace Gallio.Common.Validation
{
    /// <summary>
    /// An exception that expresses a data validation error.
    /// </summary>
    [Serializable]
    public class ValidationException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public ValidationException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}