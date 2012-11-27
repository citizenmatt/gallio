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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies a method that is to be invoked to tear down the state of a fixture
    /// after all of its tests have been executed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The attribute may be applied to multiple methods within a fixture, however
    /// the order in which they are processed is undefined.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class and must not have any parameters.  The method may be static.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class FixtureTearDownAttribute : ContributionMethodPatternAttribute
    {
        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (method.Parameters.Count != 0)
                ThrowUsageErrorException("A fixture tear-down method must not have any parameters.");
        }

        /// <inheritdoc />
        protected override void DecorateContainingScope(IPatternScope containingScope, IMethodInfo method)
        {
            containingScope.TestBuilder.TestInstanceActions.TearDownTestInstanceChain.After(state =>
                state.InvokeFixtureMethod(method, EmptyArray<KeyValuePair<ISlotInfo, object>>.Instance));
        }
    }
}
