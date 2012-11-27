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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Formatting;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a custom type formatter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That attribute must be used on a static method taking one single parameter of the source type, and returning a string describing the object.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class MyFormatters
    /// {
    ///     [Formatter]
    ///     public static string FormatColor(Color color)
    ///     {
    ///         return String.Format("Color: R={0}, G={1}, B{2}, H={3}, L={4}, S={5}", 
    ///             color.R, color.G, color.B, color.GetHue(), color.GetLuminance(), color.GetSaturation());
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class FormatterAttribute : ExtensionPointPatternAttribute
    {
        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (method.ReturnType.Resolve(true) != typeof(string))
                ThrowUsageErrorException(String.Format("Expected the custom formatting method '{0}' to return a value of type '{1}', but found '{2}'.", method.Name, typeof(string), method.ReturnType));

            if (method.Parameters.Count != 1)
                ThrowUsageErrorException(String.Format("Expected the custom formatting method '{0}' to take only one parameter, but found {1}.", method.Name, method.Parameters.Count));
        }

        /// <inheritdoc />
        protected override void DecorateContainingScope(IPatternScope containingScope, IMethodInfo methodInfo)
        {
            Type formattableType = methodInfo.Parameters[0].Resolve(true).ParameterType;
            var extensionPoints = (IExtensionPoints)RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.ExtensionPoints");
            CustomTestEnvironment.SetUpThreadChain.Before(() => extensionPoints.CustomFormatters.Register(formattableType, x => (string)methodInfo.Resolve(true).Invoke(this, new[] { x })));
            CustomTestEnvironment.TeardownThreadChain.After(() => extensionPoints.CustomFormatters.Unregister(formattableType));
        }
    }
}
