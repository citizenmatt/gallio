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

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// A <see cref="Type" /> reflection wrapper for generic parameters.
    /// The parameter is presented as if it were a slot the accepted
    /// a value of type <see cref="Type" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This wrapper enables reflection-based algorithms to be used against
    /// code that may or may not be loaded into the current AppDomain.
    /// For example, the target of the wrapper could be an in-memory
    /// code model representation.
    /// </para>
    /// </remarks>
    public interface IGenericParameterInfo : ITypeInfo, ISlotInfo, IEquatable<IGenericParameterInfo>
    {
        /// <summary>
        /// Gets the generic parameter attributes.
        /// </summary>
        GenericParameterAttributes GenericParameterAttributes { get; }

        /// <summary>
        /// Gets the declaring generic method of a generic method parameter
        /// or null for a generic type parameter.
        /// </summary>
        /// <seealso cref="IMemberInfo.DeclaringType"/>
        IMethodInfo DeclaringMethod { get; }

        /// <summary>
        /// Gets the constraints of the generic parameter.
        /// </summary>
        IList<ITypeInfo> Constraints { get; }
    }
}
