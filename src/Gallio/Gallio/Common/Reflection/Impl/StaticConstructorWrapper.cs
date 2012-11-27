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
using Gallio.Common.Collections;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> constructor wrapper.
    /// </summary>
    public sealed class StaticConstructorWrapper : StaticFunctionWrapper, IConstructorInfo
    {
        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <param name="declaringType">The declaring type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null.</exception>
        public StaticConstructorWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType, declaringType)
        {
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Constructor; }
        }

        /// <inheritdoc />
        public bool Equals(IConstructorInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public ConstructorInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveConstructor(this, throwOnError);
        }

        /// <excludedoc />
        protected override MethodBase ResolveMethodBase(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(Reflector.Wrap(typeof(void)), EmptyArray<ITypeInfo>.Instance);
        }

        /// <excludedoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            // TODO: Handle code access security.
            return EmptyArray<Attribute>.Instance;
        }
    }
}
