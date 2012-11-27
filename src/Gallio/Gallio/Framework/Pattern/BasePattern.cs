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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Model;
using System.Collections.Generic;
using Gallio.Common.Collections;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Abstract base implementation of <see cref="IPattern" /> with do-nothing
    /// implementations.
    /// </summary>
    [SystemInternal]
    public abstract class BasePattern : IPattern
    {
        /// <inheritdoc />
        public virtual bool IsPrimary
        {
            get { return false; }
        }

        /// <inheritdoc />
        public virtual IList<TestPart> GetTestParts(IPatternEvaluator evaluator, ICodeElementInfo codeElement)
        {
            return EmptyArray<TestPart>.Instance;
        }

        /// <inheritdoc />
        public virtual void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
        }

        /// <inheritdoc />
        public virtual void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
        }
    }
}