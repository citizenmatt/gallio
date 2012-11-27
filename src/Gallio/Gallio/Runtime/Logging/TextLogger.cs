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
using System.IO;
using Gallio.Common.Diagnostics;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A logger that writes output to a <see cref="TextWriter"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To include severity information in the output, wrap this logger with a <see cref="SeverityPrefixLogger" />.
    /// </para>
    /// </remarks>
    public class TextLogger : BaseLogger
    {
        private readonly TextWriter textWriter;

        /// <summary>
        /// Creates a text logger.
        /// </summary>
        /// <param name="textWriter">The text writer to which the log output should be written.</param>
        public TextLogger(TextWriter textWriter)
        {
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            this.textWriter = textWriter;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            textWriter.WriteLine(message);
            if (exceptionData != null)
            {
                textWriter.Write("  ");
                textWriter.WriteLine(exceptionData);
            }

            textWriter.Flush();
        }
    }
}
