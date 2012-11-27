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
using Gallio.Common.Text;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// The base class for commands that can be sent to the AutoCAD process.
    /// </summary>
    public abstract class AcadCommand
    {
        /// <summary>
        /// Initializes a new <see cref="AcadCommand"/> object.
        /// </summary>
        /// <param name="globalName">The global name for the command.</param>
        protected AcadCommand(string globalName)
        {
            if (String.IsNullOrEmpty(globalName))
                throw new ArgumentException("Must not be null or empty.", "globalName");
            GlobalName = globalName;
        }

        /// <summary>
        /// Gets the command's arguments.
        /// </summary>
        /// <param name="application">The <c>AcadApplication</c> COM object.</param>
        public IEnumerable<string> GetArguments(object application)
        {
            if (application == null)
                throw new ArgumentNullException("application");

            var arguments = GetArgumentsImpl(application);
            if (arguments == null)
                throw new InvalidOperationException("Unable to get arguments.");

            return arguments;
        }

        /// <summary>
        /// Gets the command's arguments.
        /// </summary>
        /// <param name="application">The <c>AcadApplication</c> COM object.</param>
        /// <remarks>
        /// This method must not return null.
        /// </remarks>
        protected abstract IEnumerable<string> GetArgumentsImpl(object application);

        /// <summary>
        /// Gets the global name for this command.
        /// </summary>
        public string GlobalName
        { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return GlobalName;
        }

        /// <summary>
        /// Creates a string representing the command as a AutoLISP expression.
        /// </summary>
        /// <param name="application">The <c>AcadApplication</c> COM object.</param>
        /// <returns>An AutoLISP expression.</returns>
        public string ToLispExpression(object application)
        {
            var builder = new StringBuilder();

            builder.Append("(command ");
            builder.Append(StringUtils.ToStringLiteral("_" + GlobalName));

            var args = GetArguments(application);
            if (args != null)
            {
                foreach (var arg in args)
                    builder.Append(" " + StringUtils.ToStringLiteral(arg ?? string.Empty));
            }

            builder.Append(")\n");

            return builder.ToString();
        }
    }
}
