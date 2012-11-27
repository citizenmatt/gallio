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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using System.Collections.Generic;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a custom type equality comparer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That attribute must be used on a static method which takes 2 parameters of the same type, and return a <see cref="bool"/> value.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class MyEqualityComparers
    /// {
    ///     [EqualityComparer]
    ///     public static bool Equals(Foo x, Foo y)
    ///     {
    ///         return /* Insert comparison logic here... */
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="ComparerAttribute"/>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class EqualityComparerAttribute : AbstractComparerAttribute
    {
        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (method.ReturnType.Resolve(true) != typeof(bool))
                ThrowUsageErrorException(String.Format("Expected the custom equality conversion method '{0}' to return '{1}', but found '{2}'.", method.Name, typeof(bool), method.ReturnType));

            if (method.Parameters.Count != 2)
                ThrowUsageErrorException(String.Format("Expected the custom equality conversion method '{0}' to take 2 parameters, but found {1}.", method.Name, method.Parameters.Count));

            if (!method.Parameters[0].ValueType.Equals(method.Parameters[1].ValueType))
                ThrowUsageErrorException(String.Format("Expected the custom equality conversion method '{0}' to take 2 parameters of the same type, but found '{1}' and '{2}'.", method.Name, method.Parameters[0], method.Parameters[1]));
        }

        /// <inheritdoc />
        protected override void Register(Type type, Func<object, object, object> operation)
        {
            ExtensionPoints.CustomEqualityComparers.Register(type, (x, y) => (bool)operation(x, y));
        }

        /// <inheritdoc />
        protected override void Unregister(Type type)
        {
            ExtensionPoints.CustomEqualityComparers.Unregister(type);
        }
    }
}
