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

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// Provides helpers for enumerating attributes taking into account
    /// the attribute inheritance structure.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </remarks>
    public static class ReflectorAttributeUtils
    {
        /// <summary>
        /// Creates an attribute instance from an <see cref="IAttributeInfo" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may be used by <see cref="IAttributeInfo.Resolve"/> to construct
        /// an attribute instance from its raw description.  Client code should
        /// call <see cref="IAttributeInfo.Resolve" /> instead of using this method
        /// directly.
        /// </para>
        /// </remarks>
        /// <param name="attribute">The attribute description.</param>
        /// <param name="throwOnError">If true, throws an exception if the target could
        /// not be resolved, otherwise the result may include unresolved types, enums or arrays.</param>
        /// <returns>The attribute instance.</returns>
        /// <exception cref="ReflectionResolveException">Thrown if the attribute could not be resolved.</exception>
        public static object CreateAttribute(IAttributeInfo attribute, bool throwOnError)
        {
            try
            {
                var constructor = attribute.Constructor.Resolve(true);
                
                var instance = constructor.Invoke(Array.ConvertAll(attribute.InitializedArgumentValues, 
                    constantValue => constantValue.Resolve(throwOnError)));

                foreach (var initializer in attribute.InitializedFieldValues)
                    initializer.Key.Resolve(true).SetValue(instance, initializer.Value.Resolve(throwOnError));

                foreach (var initializer in attribute.InitializedPropertyValues)
                    initializer.Key.Resolve(true).SetValue(instance, initializer.Value.Resolve(throwOnError), null);

                return instance;
            }
            catch (TargetException ex)
            {
                throw new ReflectionResolveException(attribute, ex);
            }
            catch (TargetParameterCountException ex)
            {
                throw new ReflectionResolveException(attribute, ex);
            }
            catch (TargetInvocationException ex)
            {
                throw new ReflectionResolveException(attribute, ex.InnerException ?? ex);
            }
            catch (MemberAccessException ex)
            {
                throw new ReflectionResolveException(attribute, ex);
            }
            catch (ArgumentException ex)
            {
                throw new ReflectionResolveException(attribute, ex);
            }
        }

        /// <summary>
        /// Returns true if the field is assignable as a named attribute parameter.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>True if the field is assignable.</returns>
        public static bool IsAttributeField(IFieldInfo field)
        {
            return !field.IsLiteral && !field.IsInitOnly && !field.IsStatic;
        }

        /// <summary>
        /// Returns true if the property is assignable as a named attribute parameter.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>True if the property is assignable.</returns>
        public static bool IsAttributeProperty(IPropertyInfo property)
        {
            IMethodInfo setMethod = property.SetMethod;
            return setMethod != null && setMethod.IsPublic && ! setMethod.IsAbstract && ! setMethod.IsStatic;
        }
    }
}
