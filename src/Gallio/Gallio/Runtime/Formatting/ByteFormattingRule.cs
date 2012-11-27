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
using System.Text;
using Gallio.Common.Text;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// A formatting rule for <see cref="byte" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Formats values as two digit hex values like "0xa5".
    /// </para>
    /// </remarks>
    public sealed class ByteFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (type == typeof(byte))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            var value = (byte)obj;
            return new StringBuilder(4, 4)
                .Append("0x")
                .Append(StringUtils.ToHexDigit(value >> 4))
                .Append(StringUtils.ToHexDigit(value))
                .ToString();
        }
    }
}