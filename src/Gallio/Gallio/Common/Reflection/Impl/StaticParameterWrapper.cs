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
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> parameter wrapper.
    /// </summary>
    public sealed class StaticParameterWrapper : StaticCodeElementWrapper, IParameterInfo
    {
        private Memoizer<ParameterAttributes> parameterAttributesMemoizer = new Memoizer<ParameterAttributes>();
        private Memoizer<string> nameMemoizer = new Memoizer<string>();
        private Memoizer<int> positionMemoizer = new Memoizer<int>();
        private Memoizer<ITypeInfo> valueTypeMemoizer = new Memoizer<ITypeInfo>();

        private readonly StaticMemberWrapper member;

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy.</param>
        /// <param name="handle">The underlying reflection object.</param>
        /// <param name="member">The member to which the parameter belongs.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>
        /// or <paramref name="member"/> is null.</exception>
        public StaticParameterWrapper(StaticReflectionPolicy policy, object handle, StaticMemberWrapper member)
            : base(policy, handle)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            this.member = member;
        }

        /// <summary>
        /// Gets the member to which the parameter belongs.
        /// </summary>
        public StaticMemberWrapper Member
        {
            get { return member; }
        }
        IMemberInfo IParameterInfo.Member
        {
            get { return member; }
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Parameter; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return nameMemoizer.Memoize(() => ReflectionPolicy.GetParameterName(this)); }
        }

        /// <inheritdoc />
        public override CodeReference CodeReference
        {
            get
            {
                CodeReference reference = member.CodeReference;
                return new CodeReference(reference.AssemblyName, reference.NamespaceName, reference.TypeName, reference.MemberName, Name);
            }
        }

        /// <inheritdoc />
        public ParameterAttributes ParameterAttributes
        {
            get
            {
                return parameterAttributesMemoizer.Memoize(delegate
                {
                    return ReflectionPolicy.GetParameterAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public bool IsIn
        {
            get { return (ParameterAttributes & ParameterAttributes.In) != 0; }
        }

        /// <inheritdoc />
        public bool IsOptional
        {
            get { return (ParameterAttributes & ParameterAttributes.Optional) != 0; }
        }

        /// <inheritdoc />
        public bool IsOut
        {
            get { return (ParameterAttributes & ParameterAttributes.Out) != 0; }
        }

        /// <inheritdoc />
        public ParameterInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveParameter(this, throwOnError);
        }

        /// <inheritdoc />
        public ITypeInfo ValueType
        {
            get { return valueTypeMemoizer.Memoize(() => member.Substitution.Apply(ReflectionPolicy.GetParameterType(this))); }
        }

        /// <inheritdoc />
        public int Position
        {
            get { return positionMemoizer.Memoize(() => ReflectionPolicy.GetParameterPosition(this)); }
        }

        /// <inheritdoc />
        public bool Equals(ISlotInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public bool Equals(IParameterInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            StaticParameterWrapper other = obj as StaticParameterWrapper;
            return EqualsByHandle(other) && member.Equals(other.member);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ member.GetHashCode();
        }

        /// <excludedoc />
        protected override IEnumerable<StaticAttributeWrapper> GetCustomAttributes()
        {
            return ReflectionPolicy.GetParameterCustomAttributes(this);
        }

        /// <excludedoc />
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            if (IsIn)
                yield return new InAttribute();
            if (IsOut)
                yield return new OutAttribute();
            if (IsOptional)
                yield return new OptionalAttribute();

            // TODO: Handle MarshalAs.
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(ValueType));
            sig.Append(' ');
            sig.Append(Name);

            return sig.ToString();
        }
    }
}
