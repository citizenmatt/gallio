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

namespace Gallio.Runner
{
    /// <summary>
    /// The type of exception thrown when the test runner or one of its
    /// supportive components like a test domain fails in an unrecoverable manner.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It can happen that the test results will be lost or incomplete.
    /// </para>
    /// </remarks>
    [Serializable]
    public class RunnerException : Exception
    {
        /// <summary>
        /// Creates an exception.
        /// </summary>
        public RunnerException()
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        public RunnerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public RunnerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RunnerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
