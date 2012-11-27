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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Gallio.Common.Collections;
using Gallio.Framework.Assertions;
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using MbUnit.Framework;
using System.Runtime.CompilerServices;
using Action=Gallio.Common.Action;

namespace Gallio.Tests.Common.Reflection
{
    /// <summary>
    /// Assertions for testing the reflection wrappers.
    /// </summary>
    public class WrapperAssert
    {
        private const BindingFlags All = BindingFlags.NonPublic | AllPublic;
        private const BindingFlags AllPublic = BindingFlags.Public
            | BindingFlags.Instance | BindingFlags.Static
            | BindingFlags.FlattenHierarchy;

        private TypeAttributes TypeAttributesMask
        {
            get
            {
                return TypeAttributes.ClassSemanticsMask | TypeAttributes.VisibilityMask
                    | TypeAttributes.Abstract | TypeAttributes.Sealed
                    | (supportsSpecialName ? TypeAttributes.SpecialName : 0);
            }
        }

        private const GenericParameterAttributes GenericParameterAttributesMask =
            GenericParameterAttributes.SpecialConstraintMask | GenericParameterAttributes.VarianceMask;

        private MethodAttributes MethodAttributesMask
        {
            get
            {
                return MethodAttributes.MemberAccessMask | MethodAttributes.VtableLayoutMask
                    | MethodAttributes.Abstract | MethodAttributes.Final
                    | MethodAttributes.PrivateScope
                    | MethodAttributes.Static | MethodAttributes.Virtual
                    | (supportsSpecialName ? MethodAttributes.SpecialName : 0);
            }
        }

        private const ParameterAttributes ParameterAttributesMask =
            ParameterAttributes.HasDefault | ParameterAttributes.Optional
            | ParameterAttributes.In | ParameterAttributes.Out | ParameterAttributes.Retval;

        private FieldAttributes FieldAttributesMask
        {
            get
            {
                return FieldAttributes.FieldAccessMask | FieldAttributes.HasDefault | FieldAttributes.InitOnly
                    | FieldAttributes.Literal | FieldAttributes.Static
                    | (supportsSpecialName ? FieldAttributes.SpecialName : 0);
            }
        }

        private PropertyAttributes PropertyAttributesMask
        {
            get
            {
                return PropertyAttributes.HasDefault
                    | (supportsSpecialName ? PropertyAttributes.SpecialName : 0);
            }
        }

        private EventAttributes EventAttributesMask
        {
            get
            {
                return supportsSpecialName ? EventAttributes.SpecialName : 0;
            }
        }

        private readonly MultiMap<object, object> equivalenceCache;
        private bool supportsSpecialFeatures = true;
        private bool supportsSpecialName = true;
        private bool supportsCallingConventions = true;
        private bool supportsReturnAttributes = true;
        private bool supportsGenericParameterAttributes = true;
        private bool supportsEventFields = true;
        private bool supportsFinalizers = true;
        private bool supportsStaticConstructors = true;
        private bool supportsFullAssemblyName = true;
        private bool supportsNestedGenericTypes = true;

        public WrapperAssert()
        {
            equivalenceCache = new MultiMap<object, object>();
        }

        /// <summary>
        /// Specifies whether the reflection API supports all of the weird quirky runtime
        /// tricks the CLR plays with reflection like creating new methods out of
        /// thin air.
        /// </summary>
        public bool SupportsSpecialFeatures
        {
            set { supportsSpecialFeatures = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports the SpecialName property for members.
        /// </summary>
        public bool SupportsSpecialName
        {
            set { supportsSpecialName = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports the CallingConventions property for methods.
        /// </summary>
        public bool SupportsCallingConventions
        {
            set { supportsCallingConventions = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports attributes on method return values.
        /// </summary>
        public bool SupportsReturnAttributes
        {
            set { supportsReturnAttributes = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports attributes on generic parameters.
        /// </summary>
        public bool SupportsGenericParameterAttributes
        {
            set { supportsGenericParameterAttributes = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports representing events as fields.
        /// </summary>
        public bool SupportsEventFields
        {
            set { supportsEventFields = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports finalizer methods.
        /// </summary>
        public bool SupportsFinalizers
        {
            set { supportsFinalizers = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports static constructors.
        /// </summary>
        public bool SupportsStaticConstructors
        {
            set { supportsStaticConstructors = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports full assembly name information.
        /// </summary>
        public bool SupportsFullAssemblyName
        {
            set { supportsFullAssemblyName = value; }
        }

        /// <summary>
        /// Specifies whether the reflection API supports nested generic types.
        /// This is to workaround R# 3.1 specific issues involving a generic type
        /// that contains another generic type nested within.
        /// </summary>
        public bool SupportsNestedGenericTypes
        {
            set { supportsNestedGenericTypes = value; }
        }

        public void AreEquivalent(string namespaceName, INamespaceInfo info)
        {
            if (namespaceName == null)
            {
                Assert.IsNull(info);
                return;
            }

            MemoizeEquivalence(namespaceName, info, delegate
            {
                Assert.AreEqual(CodeElementKind.Namespace, info.Kind);

                Assert.AreEqual(CodeReference.CreateFromNamespace(namespaceName), info.CodeReference);
                Assert.AreEqual(namespaceName, info.Name);

                Assert.IsFalse(info.GetAttributeInfos(null, false).GetEnumerator().MoveNext());
                Assert.IsFalse(info.GetAttributes(null, false).GetEnumerator().MoveNext());
                Assert.IsFalse(info.HasAttribute(null, false));

                Assert.AreEqual(CodeLocation.Unknown, info.GetCodeLocation());
                Assert.IsNull(info.GetXmlDocumentation());
            });
        }

        public void AreEquivalent(Assembly target, IAssemblyInfo info, bool recursive)
        {
            MemoizeEquivalence(target, info, delegate
            {
                Assert.AreEqual(CodeElementKind.Assembly, info.Kind);

                string targetDisplayName = target.GetName().Name;
                Assert.AreEqual(targetDisplayName, info.Name);

                if (info.Name != info.FullName)
                {
                    Assert.AreEqual(target.FullName,
                        info.ToString());
                    Assert.AreEqual(target.FullName,
                        info.FullName);
                    Assert.AreEqual(target.GetName().FullName,
                        info.GetName().FullName);
                }
                else
                {
                    // We might not always be able to get the full name in the reflection wrapper.
                    // A partial name will suffice.

                    Assert.AreEqual(targetDisplayName, info.ToString());
                    Assert.AreEqual(targetDisplayName, info.FullName);
                    Assert.AreEqual(targetDisplayName, info.GetName().Name);
                }

                AreEqualUpToAssemblyDisplayName(CodeReference.CreateFromAssembly(target), info.CodeReference);
                Assert.AreEqual(Path.GetFileName(AssemblyUtils.GetAssemblyLocalPath(target)), Path.GetFileName(info.Path), StringComparison.InvariantCultureIgnoreCase);

                CodeLocation infoLocation = info.GetCodeLocation();
                Assert.AreEqual(Path.GetFileName(AssemblyUtils.GetAssemblyLocalPath(target)), Path.GetFileName(infoLocation.Path), StringComparison.InvariantCultureIgnoreCase);
                Assert.AreEqual(0, infoLocation.Line);
                Assert.AreEqual(0, infoLocation.Column);

                Assert.IsNull(info.GetXmlDocumentation());

                Assert.AreElementsEqualIgnoringOrder(
                    target.GetReferencedAssemblies(),
                    info.GetReferencedAssemblies(),
                    delegate(AssemblyName expected, AssemblyName actual) {
                        return expected.FullName == actual.FullName;
                    });

                IList<ITypeInfo> privateTypes = info.GetTypes();
                IList<ITypeInfo> publicTypes = info.GetExportedTypes();

                foreach (Type type in target.GetTypes())
                {
                    if (type.IsPublic)
                    {
                        if (! IsUnsupportedType(type))
                        {
                            ITypeInfo foundType = info.GetType(type.FullName);
                            AreEqualWhenResolved(type, foundType);
                            Assert.IsTrue(publicTypes.Contains(foundType), "Should appear in public types: {0}", type);
                        }
                    }
                }

                foreach (ITypeInfo type in privateTypes)
                {
                    if (type.IsPublic || type.IsNestedPublic)
                        Assert.IsTrue(publicTypes.Contains(type), "Should appear in public types: {0}", type);
                    else
                        Assert.IsFalse(publicTypes.Contains(type), "Should not appear in public types: {0}", type);

                    AreEqualWhenResolved(Type.GetType(type.AssemblyQualifiedName), type);
                }

                foreach (ITypeInfo type in publicTypes)
                {
                    Assert.IsTrue(type.IsPublic || type.IsNestedPublic);
                    Assert.IsTrue(privateTypes.Contains(type), "Should appear in private types: {0}", type);
                }

                Assert.AreEqual(target, info.Resolve(true));

                AreAttributeProvidersEquivalent(target, info);
            });

            if (recursive)
            {
                foreach (Type type in target.GetExportedTypes())
                {
                    if (!IsUnsupportedType(type))
                    {
                        AreEquivalent(type, info.GetType(type.FullName), true);
                    }
                }
            }
        }

        public void AreEquivalent(Attribute target, IAttributeInfo info, bool recursive)
        {
            AreEqualWhenResolved(target.GetType(), info.Type);
            Assert.AreEqual(target.GetType(), info.Resolve(false).GetType());

            foreach (FieldInfo field in target.GetType().GetFields())
                Assert.AreEqual(field.GetValue(target), info.GetFieldValue(field.Name).Resolve(false));

            foreach (PropertyInfo property in target.GetType().GetProperties())
                if (property.CanRead && property.CanWrite)
                    Assert.AreEqual(property.GetValue(target, null), info.GetPropertyValue(property.Name).Resolve(false));

            if (recursive)
            {
                AreEquivalent(target.GetType(), info.Type, true);
            }
        }

        public void AreEquivalent(ConstructorInfo target, IConstructorInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Constructor, info.Kind);

            AreFunctionsEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));
        }

        public void AreEquivalent(MethodInfo target, IMethodInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Method, info.Kind);

            AreFunctionsEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.ContainsGenericParameters, info.ContainsGenericParameters);
            Assert.AreEqual(target.IsGenericMethod, info.IsGenericMethod);
            Assert.AreEqual(target.IsGenericMethodDefinition, info.IsGenericMethodDefinition);
            AreElementsEqualWhenResolved(target.GetGenericArguments(), info.GenericArguments);
            AreEqualWhenResolved(target.IsGenericMethod ? target.GetGenericMethodDefinition() : null, info.GenericMethodDefinition);

            AreEqualWhenResolved(target.ReturnType, info.ReturnType);
            AreEqualWhenResolved(target.ReturnParameter, info.ReturnParameter);

            if (recursive)
            {
                AreEquivalent(target.GetGenericArguments(), info.GenericArguments, false);
                AreEquivalent(target.ReturnType, info.ReturnType, false);
            }
        }

        public void AreEquivalent(EventInfo target, IEventInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Event, info.Kind);

            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.Attributes & EventAttributesMask, info.EventAttributes & EventAttributesMask);
            AreEqualWhenResolved(target.GetAddMethod(true), info.AddMethod);
            AreEqualWhenResolved(target.GetRemoveMethod(true), info.RemoveMethod);
            AreEqualWhenResolved(target.GetRaiseMethod(true), info.RaiseMethod);
            AreEqualWhenResolved(target.EventHandlerType, info.EventHandlerType);

            if (recursive)
            {
                AreEquivalent(target.EventHandlerType, info.EventHandlerType, false);
            }
        }

        public void AreEquivalent(FieldInfo target, IFieldInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Field, info.Kind);

            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.FieldType, info.ValueType);
            Assert.AreEqual(0, info.Position);

            Assert.AreEqual(target.Attributes & FieldAttributesMask, info.FieldAttributes & FieldAttributesMask);

            Assert.AreEqual(target.IsLiteral, info.IsLiteral);
            Assert.AreEqual(target.IsInitOnly, info.IsInitOnly);
            Assert.AreEqual(target.IsStatic, info.IsStatic);

            Assert.AreEqual(target.IsAssembly, info.IsAssembly);
            Assert.AreEqual(target.IsFamily, info.IsFamily);
            Assert.AreEqual(target.IsFamilyAndAssembly, info.IsFamilyAndAssembly);
            Assert.AreEqual(target.IsFamilyOrAssembly, info.IsFamilyOrAssembly);
            Assert.AreEqual(target.IsPrivate, info.IsPrivate);
            Assert.AreEqual(target.IsPublic, info.IsPublic);

            if (recursive)
            {
                AreEquivalent(target.FieldType, info.ValueType, false);
            }
        }

        public void AreEquivalent(PropertyInfo target, IPropertyInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Property, info.Kind);

            AreMembersEquivalent(target, info, recursive);
            AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(target.PropertyType, info.ValueType);
            Assert.AreEqual(0, info.Position);

            Assert.AreEqual(target.Attributes & PropertyAttributesMask, info.PropertyAttributes & PropertyAttributesMask);

            Assert.AreEqual(target.CanRead, info.CanRead);
            Assert.AreEqual(target.CanWrite, info.CanWrite);

            AreEqualWhenResolved(target.GetGetMethod(true), info.GetMethod);
            AreEqualWhenResolved(target.GetSetMethod(true), info.SetMethod);
            AreElementsEqualWhenResolved(target.GetIndexParameters(), info.IndexParameters);

            if (recursive)
            {
                AreEquivalent(target.PropertyType, info.ValueType, false);
                AreEquivalent(target.GetIndexParameters(), info.IndexParameters, true);
            }
        }

        public void AreEquivalent(Type target, ITypeInfo info, bool recursive)
        {
            if (target.IsGenericParameter)
            {
                AreEquivalent(target, (IGenericParameterInfo)info, recursive);
                return;
            }

            Assert.AreEqual(CodeElementKind.Type, info.Kind);

            AreTypesEquivalent(target, info, recursive);
            Assert.AreEqual(target, info.Resolve(true));

            if (recursive)
            {
                if (target.HasElementType)
                    AreEquivalent(target.GetElementType(), info.ElementType, false);

                if (target.BaseType != null)
                    AreEquivalent(target.BaseType, info.BaseType, false);

                AreEquivalent(target.GetGenericArguments(), info.GenericArguments, false);
            }
        }

        public void AreEquivalent(Type target, IGenericParameterInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.GenericParameter, info.Kind);

            AreTypesEquivalent(target, info, recursive);
            Assert.AreEqual(target, info.Resolve(true));

            AreEqualWhenResolved(typeof(Type), info.ValueType);
            Assert.IsTrue(info.ContainsGenericParameters);
            Assert.AreEqual(target.GenericParameterPosition, info.Position);
            Assert.AreEqual(target.GenericParameterAttributes & GenericParameterAttributesMask, info.GenericParameterAttributes & GenericParameterAttributesMask);

            Assert.IsNull(info.FullName);
            Assert.IsNull(info.AssemblyQualifiedName);
        }

        public void AreEquivalent(ParameterInfo target, IParameterInfo info, bool recursive)
        {
            Assert.AreEqual(CodeElementKind.Parameter, info.Kind);

            AreEqual(target, info.Resolve(true));

            AreEqualUpToAssemblyDisplayName(CodeReference.CreateFromParameter(target), info.CodeReference);
            Assert.AreEqual(target.Name, info.Name);
            Assert.IsNull(info.GetXmlDocumentation());

            AreEqualWhenResolved(target.ParameterType, info.ValueType);
            Assert.AreEqual(target.Position, info.Position);

            Assert.AreEqual(target.Attributes & ParameterAttributesMask, info.ParameterAttributes & ParameterAttributesMask);
            AreEqualWhenResolved(target.Member, info.Member);

            AreAttributeProvidersEquivalent(target, info);

            if (recursive)
            {
                AreEquivalent(target.ParameterType, info.ValueType, false);
            }
        }

        private void AreFunctionsEquivalent(MethodBase target, IFunctionInfo info, bool recursive)
        {
            AreMembersEquivalent(target, info, recursive);

            MemoizeEquivalence(target, info, delegate
            {
                AreEqual(target, info.Resolve(true));

                Assert.AreEqual(target.IsAbstract, info.IsAbstract);
                Assert.AreEqual(target.IsFinal, info.IsFinal);
                Assert.AreEqual(target.IsStatic, info.IsStatic);
                Assert.AreEqual(target.IsVirtual, info.IsVirtual);

                Assert.AreEqual(target.IsAssembly, info.IsAssembly);
                Assert.AreEqual(target.IsFamily, info.IsFamily);
                Assert.AreEqual(target.IsFamilyAndAssembly, info.IsFamilyAndAssembly);
                Assert.AreEqual(target.IsFamilyOrAssembly, info.IsFamilyOrAssembly);
                Assert.AreEqual(target.IsPrivate, info.IsPrivate);
                Assert.AreEqual(target.IsPublic, info.IsPublic);

                Assert.AreEqual(target.IsHideBySig, info.IsHideBySig);

                Assert.AreEqual(target.Attributes & MethodAttributesMask, info.MethodAttributes & MethodAttributesMask);
                if (supportsCallingConventions)
                    Assert.AreEqual(target.CallingConvention, info.CallingConvention);
                AreElementsEqualWhenResolved(target.GetParameters(), info.Parameters);

                // The source location may not be exactly the same with some wrappers.
                // What's important is that it is in the same file at least when it is available at all.
                CodeLocation targetLocation = DebugSymbolUtils.GetSourceLocation(target);
                if (targetLocation == CodeLocation.Unknown)
                    targetLocation = DebugSymbolUtils.GetSourceLocation(target.DeclaringType);
                CodeLocation infoLocation = info.GetCodeLocation();

                if (targetLocation != CodeLocation.Unknown && infoLocation != CodeLocation.Unknown)
                    Assert.AreEqual(targetLocation.Path, infoLocation.Path, StringComparison.InvariantCultureIgnoreCase);
            });

            if (recursive)
            {
                AreEquivalent(target.GetParameters(), info.Parameters, true);
            }
        }

        private void AreTypesEquivalent(Type target, ITypeInfo info, bool recursive)
        {
            AreMembersEquivalent(target, info, recursive);

            MemoizeEquivalence(target, info, delegate
            {
                Assert.AreEqual(target, info.Resolve(true));

                Assert.AreEqual(target.Attributes & TypeAttributesMask, info.TypeAttributes & TypeAttributesMask, target.ToString());
                Assert.AreEqual(target.Assembly, info.Assembly.Resolve(true), target.ToString());
                Assert.AreEqual(target.Namespace ?? "", info.Namespace.Name, target.ToString());
                Assert.AreEqual(target.Namespace ?? "", info.NamespaceName, target.ToString());
                AreEqualWhenResolved(target.BaseType, info.BaseType);

                if (supportsFullAssemblyName)
                    Assert.AreEqual(target.AssemblyQualifiedName,
                        info.AssemblyQualifiedName, target.ToString());
                else
                    Assert.StartsWith(target.AssemblyQualifiedName,
                        info.AssemblyQualifiedName, target.ToString());

                Assert.AreEqual(target.FullName,
                    info.FullName, target.ToString());
                AreEqualWhenResolved(target.HasElementType ? target.GetElementType() : null, info.ElementType);

                Assert.AreEqual(target.IsAbstract, info.IsAbstract);
                Assert.AreEqual(target.IsSealed, info.IsSealed);
                Assert.AreEqual(target.IsClass, info.IsClass);
                Assert.AreEqual(target.IsEnum, info.IsEnum);
                Assert.AreEqual(target.IsInterface, info.IsInterface);
                Assert.AreEqual(target.IsValueType, info.IsValueType);

                Assert.AreEqual(target.IsNested, info.IsNested);
                Assert.AreEqual(target.IsNestedAssembly, info.IsNestedAssembly);
                Assert.AreEqual(target.IsNestedFamANDAssem, info.IsNestedFamilyAndAssembly);
                Assert.AreEqual(target.IsNestedFamily, info.IsNestedFamily);
                Assert.AreEqual(target.IsNestedFamORAssem, info.IsNestedFamilyOrAssembly);
                Assert.AreEqual(target.IsNestedPrivate, info.IsNestedPrivate);
                Assert.AreEqual(target.IsNestedPublic, info.IsNestedPublic);
                Assert.AreEqual(target.IsPublic, info.IsPublic);
                Assert.AreEqual(target.IsNotPublic, info.IsNotPublic);

                Assert.AreEqual(target.IsArray, info.IsArray);
                Assert.AreEqual(target.IsPointer, info.IsPointer);
                Assert.AreEqual(target.IsByRef, info.IsByRef);
                Assert.AreEqual(target.IsGenericParameter, info.IsGenericParameter);
                Assert.AreEqual(Type.GetTypeCode(target), info.TypeCode);

                Assert.AreEqual(target.ContainsGenericParameters, info.ContainsGenericParameters);
                Assert.AreEqual(target.IsGenericType, info.IsGenericType);
                Assert.AreEqual(target.IsGenericTypeDefinition, info.IsGenericTypeDefinition);
                AreElementsEqualWhenResolved(target.GetGenericArguments(), info.GenericArguments);
                AreEqualWhenResolved(target.IsGenericType ? target.GetGenericTypeDefinition() : null, info.GenericTypeDefinition);

                if (target.IsArray)
                    Assert.AreEqual(target.GetArrayRank(), info.ArrayRank);
                else
                    Assert.Throws<InvalidOperationException>(delegate { GC.KeepAlive(info.ArrayRank); });

                if (info.IsArray || info.IsByRef || info.IsPointer || info.IsGenericParameter)
                    return; // Stop here for special types.

                AreElementsEqualWhenResolved(target.GetInterfaces(), info.Interfaces);
                BindingFlags constructorFlags = supportsStaticConstructors ? All : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                AreElementsEqualWhenResolved(target.GetConstructors(constructorFlags), info.GetConstructors(constructorFlags));
                AreElementsEqualWhenResolved(target.GetEvents(All), info.GetEvents(All));
                AreElementsEqualWhenResolved(target.GetFields(All), info.GetFields(All));
                AreElementsEqualWhenResolved(target.GetMethods(All), info.GetMethods(All));
                AreElementsEqualWhenResolved(target.GetProperties(All), info.GetProperties(All));

                foreach (MethodInfo method in target.GetMethods(AllPublic))
                {
                    if (! supportsSpecialFeatures && IsSpecialMember(method))
                        continue;

                    try
                    {
                        MethodInfo methodByName = target.GetMethod(method.Name, AllPublic);
                        Assert.IsNotNull(methodByName);

                        // In the case of hidden methods, GetMethod might return a different method than we
                        // expected because the method was hidden but not overridden.  In this case, we need
                        // to perform the comparison on the basis of the named method we obtained rather
                        // than the one we were originally looking for.
                        Assert.DoesNotThrow(delegate
                        {
                            AreEqualWhenResolved(methodByName, info.GetMethod(methodByName.Name, AllPublic));
                        });
                    }
                    catch (AmbiguousMatchException)
                    {
                        // WORKAROUND: The .Net framework contains a BUG associated with hidden generic methods.
                        // As it compares the method signatures, of the methods, it does not take into
                        // account the fact that the hidden method's parameter types may not compare
                        // for equality with those of the hiding method.
                        //
                        // For example:
                        //   public class A { public void Method<T>(T t); }
                        //   public class B : A { new public void Method<T>(T t); }
                        //
                        // The runtime will not realize that B's Method hides A's because it tries to compare
                        // parameter types to ascertain it.  The generic parameter T of the hiding method will not
                        // compare for equality with that of the hidden method.
                        //
                        // Consequently .Net will throw a spurious AmbiguousMatchException.
                        //
                        // Simulating this bug is more trouble than it's worth, so we will ignore it.
                        if (method.IsGenericMethod)
                            continue;

                        Assert.Throws<AmbiguousMatchException>(delegate
                        {
                            Assert.IsNotNull(info.GetMethod(method.Name, AllPublic));
                        });
                    }
                }

                foreach (PropertyInfo property in target.GetProperties(AllPublic))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(property))
                        continue;

                    try
                    {
                        PropertyInfo propertyByName = target.GetProperty(property.Name, AllPublic);
                        Assert.IsNotNull(propertyByName);

                        Assert.DoesNotThrow(delegate
                        {
                            AreEqualWhenResolved(propertyByName, info.GetProperty(propertyByName.Name, AllPublic));
                        });
                    }
                    catch (AmbiguousMatchException)
                    {
                        Assert.Throws<AmbiguousMatchException>(delegate
                        {
                            Assert.IsNotNull(info.GetProperty(property.Name, AllPublic));
                        });
                    }
                }

                foreach (FieldInfo field in target.GetFields(AllPublic))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(field))
                        continue;

                    try
                    {
                        FieldInfo fieldByName = target.GetField(field.Name, AllPublic);
                        Assert.IsNotNull(fieldByName);

                        Assert.DoesNotThrow(delegate
                        {
                            AreEqualWhenResolved(fieldByName, info.GetField(fieldByName.Name, AllPublic));
                        });
                    }
                    catch (AmbiguousMatchException)
                    {
                        Assert.Throws<AmbiguousMatchException>(delegate
                        {
                            Assert.IsNotNull(info.GetField(field.Name, AllPublic));
                        });
                    }
                }

                foreach (EventInfo @event in target.GetEvents(AllPublic))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(@event))
                        continue;

                    try
                    {
                        EventInfo eventByName = target.GetEvent(@event.Name, AllPublic);
                        Assert.IsNotNull(eventByName);

                        Assert.DoesNotThrow(delegate
                        {
                            AreEqualWhenResolved(eventByName, info.GetEvent(eventByName.Name, AllPublic));
                        });
                    }
                    catch (AmbiguousMatchException)
                    {
                        Assert.Throws<AmbiguousMatchException>(delegate
                        {
                            Assert.IsNotNull(info.GetEvent(@event.Name, AllPublic));
                        });
                    }
                }

                foreach (Type nestedType in target.GetNestedTypes(AllPublic))
                {
                    try
                    {
                        Type nestedTypeByName = target.GetNestedType(nestedType.Name, AllPublic);
                        Assert.IsNotNull(nestedTypeByName);

                        Assert.DoesNotThrow(delegate
                        {
                            AreEqualWhenResolved(nestedTypeByName, info.GetNestedType(nestedTypeByName.Name, AllPublic));
                        });
                    }
                    catch (AmbiguousMatchException)
                    {
                        Assert.Throws<AmbiguousMatchException>(delegate
                        {
                            Assert.IsNotNull(info.GetNestedType(nestedType.Name, AllPublic));
                        });
                    }
                }
            });

            if (recursive)
            {
                if (target.IsGenericType)
                {
                    Type[] genericArguments = target.GetGenericArguments();
                    IList<ITypeInfo> genericArgumentInfos = info.GenericArguments;
                    for (int i = 0; i < genericArguments.Length; i++)
                        AreEquivalent(genericArguments[i], genericArgumentInfos[i], false);
                }

                foreach (FieldInfo field in target.GetFields(All))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(field))
                        continue;

                    IFieldInfo fieldInfo = GetMemberThatIsEqualWhenResolved(field, info.GetFields(All));
                    AreEquivalent(field, fieldInfo, true);
                }

                foreach (EventInfo @event in target.GetEvents(All))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(@event))
                        continue;

                    IEventInfo eventInfo = GetMemberThatIsEqualWhenResolved(@event, info.GetEvents(All));
                    AreEquivalent(@event, eventInfo, true);
                }

                foreach (PropertyInfo property in target.GetProperties(All))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(property))
                        continue;

                    IPropertyInfo propertyInfo = GetMemberThatIsEqualWhenResolved(property, info.GetProperties(All));
                    AreEquivalent(property, propertyInfo, true);
                }

                foreach (ConstructorInfo constructor in target.GetConstructors(All))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(constructor))
                        continue;

                    IConstructorInfo constructorInfo = GetMemberThatIsEqualWhenResolved(constructor, info.GetConstructors(All));
                    AreEquivalent(constructor, constructorInfo, true);
                }

                foreach (MethodInfo method in target.GetMethods(All))
                {
                    if (!supportsSpecialFeatures && IsSpecialMember(method))
                        continue;

                    IMethodInfo methodInfo = GetMemberThatIsEqualWhenResolved(method, info.GetMethods(All));
                    AreEquivalent(method, methodInfo, true);
                }

                foreach (Type nestedType in target.GetNestedTypes(All))
                {
                    ITypeInfo nestedTypeInfo = GetMemberThatIsEqualWhenResolved(nestedType, info.GetNestedTypes(All));
                    AreEquivalent(nestedType, nestedTypeInfo, true);
                }
            }
        }

        private void AreMembersEquivalent(MemberInfo target, IMemberInfo info, bool recursive)
        {
            AreEqual(target, info.Resolve(true));

            Assert.AreEqual(target.ToString(), info.ToString());

            Assert.AreEqual(target.Name, info.Name);
            AreEqualUpToAssemblyDisplayName(CodeReference.CreateFromMember(target), info.CodeReference);
            AreEqualWhenResolved(target.DeclaringType, info.DeclaringType);
            AreEqualWhenResolved(target.ReflectedType, info.ReflectedType);

            string xmlDocumentation = info.GetXmlDocumentation();
            if (xmlDocumentation != null)
                Assert.AreEqual(XmlDocumentationUtils.GetXmlDocumentation(target), xmlDocumentation);

            AreAttributeProvidersEquivalent(target, info);
        }

        private void AreEquivalent(ParameterInfo[] parameters, IList<IParameterInfo> parameterInfos, bool recursive)
        {
            for (int i = 0; i < parameters.Length; i++)
                AreEquivalent(parameters[i], parameterInfos[i], recursive);
        }

        private void AreEquivalent(Type[] types, IList<ITypeInfo> typeInfos, bool recursive)
        {
            for (int i = 0; i < types.Length; i++)
                AreEquivalent(types[i], typeInfos[i], recursive);
        }

        private static T GetMemberThatIsEqualWhenResolved<T>(MemberInfo member, IEnumerable<T> memberInfos)
            where T : IMemberInfo
        {
            foreach (T memberInfo in memberInfos)
            {
                MemberInfo resolvedMember = memberInfo.Resolve(true);
                if (member.DeclaringType == resolvedMember.DeclaringType
                    && member.MetadataToken == resolvedMember.MetadataToken)
                    return memberInfo;
            }

            throw new AssertionException(String.Format("Did not find matching member: {0}", member));
        }

        private void AreEqual(MemberInfo expected, MemberInfo actual)
        {
            Assert.AreEqual(expected.DeclaringType, actual.DeclaringType);
            Assert.AreEqual(expected.MetadataToken, actual.MetadataToken);
        }

        private void AreEqual(ParameterInfo expected, ParameterInfo actual)
        {
            Assert.AreEqual(expected.Member.DeclaringType, actual.Member.DeclaringType);
            Assert.AreEqual(expected.MetadataToken, actual.MetadataToken);
        }

        private void AreEqualWhenResolved(Type expected, ITypeInfo wrapper)
        {
            if (expected != null)
            {
                Assert.AreEqual(expected, wrapper != null ? wrapper.Resolve(true) : null);
            }
            else
                Assert.IsNull(wrapper);
        }

        private void AreEqualWhenResolved<TMember, TWrapper>(TMember expected, TWrapper wrapper)
            where TMember : MemberInfo
            where TWrapper : IMemberInfo
        {
            if (expected != null)
                AreEqual(expected, wrapper != null ? wrapper.Resolve(true) : null);
            else
                Assert.IsNull(wrapper);
        }

        private void AreEqualWhenResolved(ParameterInfo expected, IParameterInfo wrapper)
        {
            if (expected != null)
                AreEqual(expected, wrapper != null ? wrapper.Resolve(true) : null);
            else
                Assert.IsNull(wrapper);
        }

        private void AreElementsEqualWhenResolved<TWrapper>(IEnumerable<Type> expectedTypes, IEnumerable<TWrapper> actualTypes)
            where TWrapper : ITypeInfo
        {
            Dictionary<string, Type> keyedExpectedTypes = new Dictionary<string, Type>();
            foreach (Type expectedType in expectedTypes)
                keyedExpectedTypes.Add(expectedType.FullName ?? expectedType.Name, expectedType);

            Dictionary<string, ITypeInfo> keyedActualTypes = new Dictionary<string, ITypeInfo>();
            foreach (ITypeInfo actualType in actualTypes)
                keyedActualTypes.Add(actualType.FullName ?? actualType.Name, actualType);

            Assert.Over.KeyedPairs(keyedExpectedTypes, keyedActualTypes, AreEqualWhenResolved);
        }

        private void AreElementsEqualWhenResolved<TMember, TWrapper>(IEnumerable<TMember> expectedMembers, IEnumerable<TWrapper> actualMembers)
            where TMember : MemberInfo
            where TWrapper : IMemberInfo
        {
            Dictionary<string, TMember> keyedExpectedMembers = new Dictionary<string, TMember>();
            foreach (TMember expectedMember in expectedMembers)
            {
                if (!supportsSpecialFeatures && IsSpecialMember(expectedMember))
                    continue;

                string key = expectedMember.DeclaringType + " -> " + expectedMember;
                keyedExpectedMembers.Add(key, expectedMember);
            }

            Dictionary<string, TWrapper> keyedActualMembers = new Dictionary<string, TWrapper>();
            foreach (TWrapper actualMember in actualMembers)
            {
                if (!supportsSpecialFeatures && IsSpecialMember(actualMember.Resolve(false)))
                    continue;

                string key = actualMember.DeclaringType + " -> " + actualMember;

                if (keyedActualMembers.ContainsKey(key))
                    Assert.Fail("Found duplicate member: {0}", key);
                keyedActualMembers.Add(key, actualMember);
            }

            Assert.Over.KeyedPairs(keyedExpectedMembers, keyedActualMembers, AreEqualWhenResolved);
        }

        private void AreElementsEqualWhenResolved(IEnumerable<ParameterInfo> expected, IEnumerable<IParameterInfo> actual)
        {
            Assert.Over.Pairs(expected, actual, AreEqualWhenResolved);
        }

        private void AreAttributeProvidersEquivalent<TTarget, TWrapper>(TTarget expectedTarget, TWrapper actualWrapper)
            where TTarget : ICustomAttributeProvider
            where TWrapper : ICodeElementInfo
        {
            // There are some weird cases with attributes of object.
            // For example, the runtime reutrns ComVisibleAttributer and ClassInterfaceAttribute
            // when the "inherit" flag is false, but not if the flag is true!
            if (! supportsSpecialFeatures && expectedTarget is Type && expectedTarget.Equals(typeof(object)))
                return;

            if (!supportsReturnAttributes)
            {
                MethodInfo targetMethod = expectedTarget as MethodInfo;
                if (targetMethod != null && MightHaveOrInheritReturnParameter(targetMethod))
                    return;
            }

            if (!supportsGenericParameterAttributes)
            {
                Type targetType = expectedTarget as Type;
                if (targetType != null && targetType.IsGenericParameter)
                    return;
            }

            IList<object> nonInheritedAttribs;
            IList<object> inheritedAttribs;
            try
            {
                nonInheritedAttribs = expectedTarget.GetCustomAttributes(false);
                inheritedAttribs = expectedTarget.GetCustomAttributes(true);
            }
            catch (ArgumentException)
            {
                // The sample assembly might have some attributes with deliberately invalid arguments.
                // If we trip over then, we can just ignore them.  It would be better to check that
                // the same exception were thrown but that's a little tedious and in any case we
                // have pretty good guarantees that it will given everything else works.
                return;
            }

            ITypeInfo attributeType = Reflector.Wrap(typeof(Attribute));

            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributes(null, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributes(null, true));
            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributes(attributeType, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributes(attributeType, true));

            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributeInfos(null, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributeInfos(null, true));
            AreAttributesOfSameTypesExcludingSpecialAttributes(nonInheritedAttribs,
                actualWrapper.GetAttributeInfos(attributeType, false));
            AreAttributesOfSameTypesExcludingSpecialAttributes(inheritedAttribs,
                actualWrapper.GetAttributeInfos(attributeType, true));

            foreach (object nonInheritedAttrib in nonInheritedAttribs)
            {
                if (supportsSpecialFeatures || !IsSpecialAttribute(nonInheritedAttrib))
                    Assert.IsTrue(AttributeUtils.HasAttribute(actualWrapper, nonInheritedAttrib.GetType(), false),
                        "Should contain non-inherited attributes of same type.");
            }

            foreach (object inheritedAttrib in inheritedAttribs)
            {
                if (supportsSpecialFeatures || !IsSpecialAttribute(inheritedAttrib))
                    Assert.IsTrue(AttributeUtils.HasAttribute(actualWrapper, inheritedAttrib.GetType(), true),
                        "Should contain inherited attributes of same type.");
            }
        }

        private void AreAttributesOfSameTypesExcludingSpecialAttributes(IEnumerable<object> expected,
            IEnumerable<object> actual)
        {
            Dictionary<Type, int> expectedCounts = new Dictionary<Type, int>();
            Dictionary<Type, int> actualCounts = new Dictionary<Type, int>();

            foreach (object expectedAttrib in expected)
                if (supportsSpecialFeatures || !IsSpecialAttribute(expectedAttrib))
                    IncrementCount(expectedCounts, expectedAttrib.GetType());
            foreach (object actualAttrib in actual)
                if (supportsSpecialFeatures || !IsSpecialAttribute(actualAttrib))
                    IncrementCount(actualCounts, actualAttrib.GetType());

            Assert.Over.KeyedPairs(expectedCounts, actualCounts, delegate(Type type, int expectedCount, int actualCount)
            {
                Assert.AreEqual(expectedCount, actualCount, "Number of {0} attributes should be equal.", type);
            });
        }

        private void AreAttributesOfSameTypesExcludingSpecialAttributes(IEnumerable<object> expected,
            IEnumerable<IAttributeInfo> actual)
        {
            List<object> actualResolvedObjects = new List<object>();
            foreach (IAttributeInfo attrib in actual)
                actualResolvedObjects.Add(attrib.Resolve(false));

            AreAttributesOfSameTypesExcludingSpecialAttributes(expected, actualResolvedObjects);
        }

        private static void IncrementCount(IDictionary<Type, int> counts, Type type)
        {
            int count;
            counts.TryGetValue(type, out count);
            counts[type] = count + 1;
        }

        public bool IsUnsupportedType(Type type)
        {
            if (!supportsNestedGenericTypes && IsNestedGenericType(type))
                return true;
            return false;
        }

        private bool IsNestedGenericType(Type type)
        {
            int genericTypeCount = 0;
            while (type != null)
            {
                if (type.IsGenericType)
                    genericTypeCount += 1;
                type = type.DeclaringType;
            }

            return genericTypeCount > 1;
        }

        private bool IsSpecialAttribute(object attrib)
        {
            return attrib is ComVisibleAttribute
                || attrib is ClassInterfaceAttribute
                || attrib is DefaultMemberAttribute
                || attrib is SerializableAttribute
                || attrib is DebuggableAttribute
                || attrib is CompilationRelaxationsAttribute
                || attrib is RuntimeCompatibilityAttribute
                || attrib is AssemblyCultureAttribute
                || attrib is AssemblyVersionAttribute
                || attrib is SecurityAttribute
                || attrib is StructLayoutAttribute
                || attrib is MethodImplAttribute;
        }

        private bool IsSpecialMember(FieldInfo field)
        {
            if (! supportsEventFields && field.FieldType.IsSubclassOf(typeof(Delegate))) // imprecise hack
                return true;

            return IsSpecialMember(field, field.IsPublic);
        }

        private bool IsSpecialMember(PropertyInfo property)
        {
            return IsSpecialMember(property,
                property.GetGetMethod() != null
                || property.GetSetMethod() != null);
        }

        private bool IsSpecialMember(EventInfo @event)
        {
            return IsSpecialMember(@event,
                @event.GetAddMethod() != null
                || @event.GetRemoveMethod() != null
                || @event.GetRaiseMethod() != null);
        }

        private bool IsSpecialMember(MethodBase function)
        {
            if (!supportsCallingConventions && (function.CallingConvention & CallingConventions.VarArgs) != 0)
                return true;
            if (!supportsFinalizers && function.Name == "Finalize")
                return true;
            if (function.IsConstructor && function.DeclaringType.IsSubclassOf(typeof(Delegate)))
                return true;

            return IsSpecialMember(function, function.IsPublic);
        }

        private bool IsSpecialMember(MemberInfo member)
        {
            if (member is FieldInfo)
                return IsSpecialMember((FieldInfo)member);
            if (member is PropertyInfo)
                return IsSpecialMember((PropertyInfo)member);
            if (member is EventInfo)
                return IsSpecialMember((EventInfo)member);
            if (member is MethodBase)
                return IsSpecialMember((MethodBase)member);
            throw new NotSupportedException("Unsupported member.");
        }

        private bool IsSpecialMember(MemberInfo member, bool isPublic)
        {
            if (! isPublic && member.DeclaringType.Namespace.StartsWith("System"))
                return true;

            return member.Name.Contains("<");
        }

        private static bool MightHaveOrInheritReturnParameter(MethodInfo method)
        {
            if (HasReturnParameter(method))
                return true;

            // Check possibly overridden methods.
            for (Type baseType = method.DeclaringType.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                foreach (MethodInfo inheritedMethod in baseType.GetMethods(All | BindingFlags.DeclaredOnly))
                {
                    if (method.Name == inheritedMethod.Name && HasReturnParameter(inheritedMethod))
                        return true;
                }
            }

            return false;
        }

        private static bool HasReturnParameter(MethodInfo method)
        {
            return method.ReturnParameter.GetCustomAttributes(true).Length != 0;
        }

        private static void AreEqualUpToAssemblyDisplayName(CodeReference expected, CodeReference actual)
        {
            Assert.AreEqual(
                new CodeReference(new AssemblyName(expected.AssemblyName).Name,
                    expected.NamespaceName, expected.TypeName, expected.MemberName, expected.ParameterName),
                new CodeReference(new AssemblyName(actual.AssemblyName).Name,
                    actual.NamespaceName, actual.TypeName, actual.MemberName, actual.ParameterName));
        }

        /// <summary>
        /// We memoize results from comparing for equivalence of certain members to help
        /// the exhaustive recursive reflection tests run in a more reasonable period of time.
        /// This implementation assumes that the Equals methods are implemented correctly.
        /// If you're uncertain of this, you should disable memoization.
        /// </summary>
        private void MemoizeEquivalence(object target, object wrapper, Action action)
        {
            if (!equivalenceCache.Contains(target, wrapper))
            {
                action();
                equivalenceCache.Add(target, wrapper);
            }
        }
    }
}
