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
using Gallio.Common.Diagnostics;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// A null implementation of <see cref="ILogger" /> that does nothing.
    /// </summary>
    public sealed class NullLogger : BaseLogger
    {
        /// <summary>
        /// Gets a singleton instance of the null logger.
        /// </summary>
        public static readonly NullLogger Instance = new NullLogger();

        /// <excludedoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
        }
    }
}
