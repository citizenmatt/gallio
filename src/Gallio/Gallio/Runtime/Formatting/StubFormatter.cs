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
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// A formatter that is used when the runtime is not initialized.
    /// </summary>
    public class StubFormatter : RuleBasedFormatter
    {
        /// <summary>
        /// Creates a stub formatter using only a few built-in formatting rules.
        /// </summary>
        public StubFormatter()
            : base(new DefaultExtensionPoints(), GetBuiltInRules())
        {
        }

        private static IFormattingRule[] GetBuiltInRules()
        {
            var rules = new List<IFormattingRule>();

            foreach (Type type in typeof(StubFormatter).Assembly.GetExportedTypes())
            {
                if (typeof(IFormattingRule).IsAssignableFrom(type)
                    && type.GetConstructor(EmptyArray<Type>.Instance) != null)
                {
                    rules.Add((IFormattingRule)Activator.CreateInstance(type));
                }
            }

            return rules.ToArray();
        }
    }
}
