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
using Gallio.Common.Reflection;

using XunitTypeInfo = Xunit.Sdk.ITypeInfo;
using XunitMethodInfo = Xunit.Sdk.IMethodInfo;
using XunitAttributeInfo = Xunit.Sdk.IAttributeInfo;

namespace Gallio.XunitAdapter.Model
{
    /// <summary>
    /// An adapter for converting <see cref="ITypeInfo" /> into <see cref="XunitTypeInfo" />.
    /// </summary>
    internal class XunitTypeInfoAdapter : XunitTypeInfo
    {
        private readonly ITypeInfo target;

        public XunitTypeInfoAdapter(ITypeInfo target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        public ITypeInfo Target
        {
            get { return target; }
        }

        public XunitMethodInfo GetMethod(string methodName)
        {
            IMethodInfo method = target.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Instance | BindingFlags.Static);
            return method != null ? new XunitMethodInfoAdapter(method) : null;
        }

        public IEnumerable<XunitMethodInfo> GetMethods()
        {
            foreach (IMethodInfo method in target.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                yield return new XunitMethodInfoAdapter(method);
        }

        public IEnumerable<XunitAttributeInfo> GetCustomAttributes(Type attributeType)
        {
            foreach (IAttributeInfo attribute in target.GetAttributeInfos(Reflector.Wrap(attributeType), true))
                yield return new XunitAttributeInfoAdapter(attribute);
        }

        public bool HasAttribute(Type attributeType)
        {
            return AttributeUtils.HasAttribute(target, attributeType, true);
        }

        public bool HasInterface(Type interfaceType)
        {
            foreach (ITypeInfo @interface in target.Interfaces)
            {
                if (@interface.FullName == interfaceType.FullName)
                    return true;
            }

            return false;
        }

        public bool IsAbstract
        {
            get { return target.IsAbstract; }
        }

        public bool IsSealed
        {
            get { return target.IsSealed; }
        }

        public Type Type
        {
            get { return target.Resolve(false); }
        }

        public override string ToString()
        {
            return target.FullName;
        }
    }
}
