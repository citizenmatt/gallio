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
using Gallio.Model;

namespace Gallio.Common.Text.RegularExpression
{
    /// <summary>
    /// This exception type is used to signal an error while parsing
    /// a simplified regular expression with the RegexLite parser.
    /// </summary>
    [Serializable]
    public class RegexLiteException : Exception
    {
        /// <summary>
        /// Creates a exception.
        /// </summary>
        public RegexLiteException()
        {
        }

        /// <summary>
        /// Creates a exception with the specified message..
        /// </summary>
        /// <param name="message">The message, or null if none.</param>
        public RegexLiteException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a exception with the specified message and the inner exception.
        /// </summary>
        /// <param name="message">The message, or null if none.</param>
        /// <param name="innerException">The inner exception, or null if none.</param>
        public RegexLiteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RegexLiteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
