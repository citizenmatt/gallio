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
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using Gallio.ReSharperRunner.Provider;
using JetBrains.Metadata.Access;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using System.IO;
#if RESHARPER_31
using JetBrains.Shell;
using JetBrains.Util;
#else
using JetBrains.Application;
#if RESHARPER_61_OR_NEWER
using JetBrains.ReSharper.Psi.Impl.Caches2;
#endif
using AssemblyReference=JetBrains.Metadata.Access.AssemblyReference;
#endif
#if RESHARPER_50_OR_NEWER
using JetBrains.Metadata.Utils;
#endif
#if RESHARPER_60_OR_NEWER
using JetBrains.Util;
using ConstantValue = Gallio.Common.Reflection.ConstantValue;

#endif

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Wraps ReSharper metadata objects using the reflection adapter interfaces.
    /// </summary>
    public class MetadataReflectionPolicy : ReSharperReflectionPolicy
    {
        private readonly IProject contextProject;
        private readonly MetadataLoader metadataLoader;
#if RESHARPER_61_OR_NEWER
		private readonly CacheManagerEx cacheManager;
#endif

        private KeyedMemoizer<IMetadataAssembly, StaticAssemblyWrapper> assemblyMemoizer = new KeyedMemoizer<IMetadataAssembly, StaticAssemblyWrapper>();
        private KeyedMemoizer<IMetadataType, StaticTypeWrapper> typeMemoizer = new KeyedMemoizer<IMetadataType, StaticTypeWrapper>();
        private KeyedMemoizer<IMetadataTypeInfo, StaticDeclaredTypeWrapper> typeWithoutSubstitutionMemoizer = new KeyedMemoizer<IMetadataTypeInfo, StaticDeclaredTypeWrapper>();
        private KeyedMemoizer<IMetadataClassType, StaticDeclaredTypeWrapper> classMemoizer = new KeyedMemoizer<IMetadataClassType, StaticDeclaredTypeWrapper>();

#if !RESHARPER_61_OR_NEWER
        /// <summary>
        /// Creates a reflector with the specified project as its context.
        /// The context project is used to resolve metadata items to declared elements.
        /// </summary>
        /// <param name="assembly">The assembly provide context for the loader.</param>
        /// <param name="contextProject">The context project, or null if none.</param>
        public MetadataReflectionPolicy(IMetadataAssembly assembly, IProject contextProject)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            metadataLoader = GetMetadataLoaderHack(assembly);
            this.contextProject = contextProject;
        }
#else
		/// <summary>
		/// Creates a reflector with the specified project as its context.
		/// The context project is used to resolve metadata items to declared elements.
		/// </summary>
		/// <param name="assembly">The assembly provide context for the loader.</param>
		/// <param name="contextProject">The context project, or null if none.</param>
		/// <param name="cacheManager">Cache manager.</param>
		public MetadataReflectionPolicy(IMetadataAssembly assembly, IProject contextProject, CacheManagerEx cacheManager)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			metadataLoader = GetMetadataLoaderHack(assembly);
			this.contextProject = contextProject;
			this.cacheManager = cacheManager;
		}
#endif
        /// <summary>
        /// Gets the context project, or null if none.
        /// </summary>
        public IProject ContextProject
        {
            get { return contextProject; }
        }

        #region Wrapping
        /// <summary>
        /// Obtains a reflection wrapper for an assembly.
        /// </summary>
        /// <param name="target">The assembly, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticAssemblyWrapper Wrap(IMetadataAssembly target)
        {
            return target != null
                ? assemblyMemoizer.Memoize(target, () => new StaticAssemblyWrapper(this, target))
                : null;
        }

        /// <summary>
        /// Obtains a reflection wrapper for a property.
        /// </summary>
        /// <param name="target">The property, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticPropertyWrapper Wrap(IMetadataProperty target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.DeclaringType);
            return new StaticPropertyWrapper(this, target, declaringType, declaringType);
        }

        /// <summary>
        /// Obtains a reflection wrapper for a field.
        /// </summary>
        /// <param name="target">The field, or null if none.</param>
        /// <returns>The reflection wrapper, or null if none.</returns>
        public StaticFieldWrapper Wrap(IMetadataField target)
        {
            if (target == null)
                return null;

            StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(target.DeclaringType);
            return new StaticFieldWrapper(this, target, declaringType, declaringType);
        }
        #endregion

        #region Assemblies
        protected override IAssemblyInfo LoadAssemblyImpl(AssemblyName assemblyName)
        {
            return Wrap(LoadMetadataAssembly(assemblyName, true));
        }

        protected override IAssemblyInfo LoadAssemblyFromImpl(string assemblyFile)
        {
            if (metadataLoader != null)
#if RESHARPER_60_OR_NEWER
				return Wrap(metadataLoader.LoadFrom(new FileSystemPath(assemblyFile), DummyLoadReferencePredicate));
#else
                return Wrap(metadataLoader.LoadFrom(assemblyFile, DummyLoadReferencePredicate));
#endif

            throw new InvalidOperationException(String.Format("The metadata loader could not load assembly '{0}'.", assemblyFile));
        }

        protected override IEnumerable<StaticAttributeWrapper> GetAssemblyCustomAttributes(StaticAssemblyWrapper assembly)
        {
            IMetadataAssembly assemblyHandle = (IMetadataAssembly)assembly.Handle;
            return EnumerateAttributesForEntity(assemblyHandle);
        }

        protected override AssemblyName GetAssemblyName(StaticAssemblyWrapper assembly)
        {
            IMetadataAssembly assemblyHandle = (IMetadataAssembly) assembly.Handle;
#if RESHARPER_50_OR_NEWER
            return new AssemblyName(assemblyHandle.AssemblyName.FullName);
#else
            return assemblyHandle.AssemblyName;
#endif
        }

        protected override string GetAssemblyPath(StaticAssemblyWrapper assembly)
        {
            IMetadataAssembly assemblyHandle = (IMetadataAssembly)assembly.Handle;
#if RESHARPER_60_OR_NEWER
            return assemblyHandle.Location.FullPath;
#else
			return assemblyHandle.Location;
#endif
		}

        protected override IList<AssemblyName> GetAssemblyReferences(StaticAssemblyWrapper assembly)
        {
            var assemblyHandle = (IMetadataAssembly)assembly.Handle;
#if RESHARPER_60_OR_NEWER
			var referencedAssembliesNames = new List<AssemblyNameInfo>(assemblyHandle.ReferencedAssembliesNames);
			return Array.ConvertAll(referencedAssembliesNames.ToArray(), assemblyNameInfo => new AssemblyName(assemblyNameInfo.FullName));
#else
            AssemblyReference[] references = assemblyHandle.ReferencedAssembliesNames;
            return Array.ConvertAll<AssemblyReference, AssemblyName>(references, delegate(AssemblyReference reference)
            {
#if RESHARPER_50_OR_NEWER
                return new AssemblyName(reference.AssemblyName.FullName);
#else
                return reference.AssemblyName;
#endif
			});
#endif
		}

        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyExportedTypes(StaticAssemblyWrapper assembly)
        {
            IMetadataAssembly assemblyHandle = (IMetadataAssembly)assembly.Handle;

            List<StaticDeclaredTypeWrapper> types = new List<StaticDeclaredTypeWrapper>();
            foreach (IMetadataTypeInfo type in assemblyHandle.GetTypes())
            {
                if (type.IsPublic || type.IsNestedPublic)
                    types.Add(MakeDeclaredTypeWithoutSubstitution(type));
            }

            return types;
        }

        protected override IList<StaticDeclaredTypeWrapper> GetAssemblyTypes(StaticAssemblyWrapper assembly)
        {
            IMetadataAssembly assemblyHandle = (IMetadataAssembly)assembly.Handle;

            return Array.ConvertAll<IMetadataTypeInfo, StaticDeclaredTypeWrapper>(assemblyHandle.GetTypes(), MakeDeclaredTypeWithoutSubstitution);
        }

        protected override StaticDeclaredTypeWrapper GetAssemblyType(StaticAssemblyWrapper assembly, string typeName)
        {
            IMetadataAssembly assemblyHandle = (IMetadataAssembly)assembly.Handle;
            IMetadataClassType typeHandle = ((IMetadataClassType)assemblyHandle.GetTypeFromQualifiedName(typeName, false));

            // Note: ReSharper returns an unresolved type when it can't find the type by name.
            // The unresolved type can be distinguished by the fact that it does not have a declaring assembly name
            // or by the fact that its metadata token is always 0.
            if (typeHandle.Type.DeclaringAssemblyName == null)
                return null;

            return MakeDeclaredType(typeHandle);
        }

        private IMetadataAssembly LoadMetadataAssembly(AssemblyName assemblyName, bool throwOnError)
        {
            if (metadataLoader != null)
            {
#if RESHARPER_50_OR_NEWER && !RESHARPER_60_OR_NEWER
                IMetadataAssembly assembly = metadataLoader.Load(new AssemblyNameInfo(assemblyName.FullName), DummyLoadReferencePredicate);
#elif RESHARPER_60_OR_NEWER
                var assembly = metadataLoader.TryLoad(new AssemblyNameInfo(assemblyName.FullName), DummyLoadReferencePredicate);
#else
                IMetadataAssembly assembly = metadataLoader.Load(assemblyName, DummyLoadReferencePredicate);
#endif

                if (assembly == null && contextProject != null)
                {
#if RESHARPER_60_OR_NEWER
					var reference = GenericCollectionUtils.Find(contextProject.GetAssemblyReferences(), candidate => 
						candidate.ReferenceTarget.AssemblyName != null &&
						candidate.ReferenceTarget.AssemblyName.FullName == assemblyName.FullName);

					if (reference != null && reference.ReferenceTarget.HintLocation != null)
					{
						var hintLocation = reference.ReferenceTarget.HintLocation;
						if (File.Exists(hintLocation.FullPath))
							assembly = metadataLoader.LoadFrom(hintLocation, DummyLoadReferencePredicate);
					}

#else
                    IAssemblyReference reference = GenericCollectionUtils.Find(contextProject.GetAssemblyReferences(),
                        delegate(IAssemblyReference candidate)
                        {
#if RESHARPER_50_OR_NEWER
                            return candidate.AssemblyName != null
                                && candidate.AssemblyName.FullName == assemblyName.FullName;
#else
                            return candidate.AssemblyIdentity != null
                                && candidate.AssemblyIdentity.AssemblyName.FullName == assemblyName.FullName;
#endif
                        });

                    if (reference != null && reference.HintLocation != null)
                    {
                        string hintLocation = reference.HintLocation.FullPath;
                        if (File.Exists(hintLocation))
                            assembly = metadataLoader.LoadFrom(hintLocation, DummyLoadReferencePredicate);
                    }
#endif
                }

                if (assembly != null)
                    return assembly;
            }

            if (throwOnError)
                throw new InvalidOperationException(String.Format("The metadata loader could not load assembly '{0}'.", assemblyName));

            return null;
        }

#if RESHARPER_50_OR_NEWER
        private static bool DummyLoadReferencePredicate(AssemblyNameInfo ignored)
#else
        private static bool DummyLoadReferencePredicate(AssemblyName ignored)
#endif
        {
            return true;
        }
        #endregion

        #region Attributes
        protected override StaticConstructorWrapper GetAttributeConstructor(StaticAttributeWrapper attribute)
        {
            IMetadataCustomAttribute attributeHandle = (IMetadataCustomAttribute)attribute.Handle;
            IMetadataMethod usedConstructor = ResolveMetadataMethodHack(attributeHandle.UsedConstructor);

            return new StaticConstructorWrapper(this, usedConstructor,
                MakeDeclaredTypeWithoutSubstitution(usedConstructor.DeclaringType));
        }

        protected override ConstantValue[] GetAttributeConstructorArguments(StaticAttributeWrapper attribute)
        {
            IMetadataCustomAttribute attributeHandle = (IMetadataCustomAttribute)attribute.Handle;
#if RESHARPER_60_OR_NEWER
			return Array.ConvertAll<MetadataAttributeValue, ConstantValue>(attributeHandle.ConstructorArguments, ConvertConstantValue);
#else
			return Array.ConvertAll<object, ConstantValue>(attributeHandle.ConstructorArguments, ConvertConstantValue);
#endif
		}

        protected override IEnumerable<KeyValuePair<StaticFieldWrapper, ConstantValue>> GetAttributeFieldArguments(StaticAttributeWrapper attribute)
        {
            IMetadataCustomAttribute attributeHandle = (IMetadataCustomAttribute)attribute.Handle;

            IMetadataCustomAttributeFieldInitialization[] initializations = attributeHandle.InitializedFields;
            foreach (IMetadataCustomAttributeFieldInitialization initialization in initializations)
                yield return new KeyValuePair<StaticFieldWrapper, ConstantValue>(Wrap(initialization.Field), ConvertConstantValue(initialization.Value));
        }

        protected override IEnumerable<KeyValuePair<StaticPropertyWrapper, ConstantValue>> GetAttributePropertyArguments(StaticAttributeWrapper attribute)
        {
            IMetadataCustomAttribute attributeHandle = (IMetadataCustomAttribute)attribute.Handle;

            IMetadataCustomAttributePropertyInitialization[] initializations = attributeHandle.InitializedProperties;
            foreach (IMetadataCustomAttributePropertyInitialization initialization in initializations)
                yield return new KeyValuePair<StaticPropertyWrapper, ConstantValue>(Wrap(initialization.Property), ConvertConstantValue(initialization.Value));
        }

#if RESHARPER_60_OR_NEWER
		private ConstantValue ConvertConstantValue(MetadataAttributeValue value)
		{
			return ConvertConstantValue(value.Value, (IMetadataType type) => MakeType(type));
		}
#else
        private ConstantValue ConvertConstantValue(object value)
        {
            return ConvertConstantValue(value, (IMetadataType type) => MakeType(type));
        }
#endif

		private IEnumerable<StaticAttributeWrapper> EnumerateAttributesForEntity(IMetadataEntity entityHandle)
        {
            IMetadataCustomAttribute[] attribs = null;
            ReSharperExceptionDialogSuppressor.Suppress("Gallio was unable to read an attribute due to ReSharper bug RSRP-76078 which affects boxed custom attribute values of type SzArray.",
                () => attribs = entityHandle.CustomAttributes);

            foreach (IMetadataCustomAttribute attributeHandle in attribs)
                if (attributeHandle.UsedConstructor != null)
                    // Note: Can be null occasionally and R# itself will ignore it, why?
                    yield return new StaticAttributeWrapper(this, attributeHandle);
        }
        #endregion

        #region Members
        protected override IEnumerable<StaticAttributeWrapper> GetMemberCustomAttributes(StaticMemberWrapper member)
        {
            IMetadataEntity entityHandle = (IMetadataEntity) member.Handle;
            return EnumerateAttributesForEntity(entityHandle);
        }

        protected override string GetMemberName(StaticMemberWrapper member)
        {
            IMetadataEntity entityHandle = (IMetadataEntity) member.Handle;

            IMetadataTypeInfo typeHandle = entityHandle as IMetadataTypeInfo;
            if (typeHandle != null)
#if RESHARPER_60_OR_NEWER
                return new ClrTypeName(typeHandle.FullyQualifiedName).ShortName;
#else
                return new CLRTypeName(typeHandle.FullyQualifiedName).ShortName;
#endif

            IMetadataField fieldHandle = entityHandle as IMetadataField;
            if (fieldHandle != null)
                return fieldHandle.Name;

            IMetadataProperty propertyHandle = entityHandle as IMetadataProperty;
            if (propertyHandle != null)
                return propertyHandle.Name;

            IMetadataEvent eventHandle = entityHandle as IMetadataEvent;
            if (eventHandle != null)
                return eventHandle.Name;

            IMetadataMethod methodHandle = entityHandle as IMetadataMethod;
            if (methodHandle != null)
                return methodHandle.Name;

            IMetadataGenericArgument genericArgumentHandle = entityHandle as IMetadataGenericArgument;
            if (genericArgumentHandle != null)
                return genericArgumentHandle.Name;

            throw new NotSupportedException("Unsupported member type: " + entityHandle);
        }

        protected override CodeLocation GetMemberSourceLocation(StaticMemberWrapper member)
        {
            IDeclaredElementResolver resolver = GetDeclaredElementResolver((StaticWrapper)member);
            if (resolver != null)
            {
                IDeclaredElement declaredElement = resolver.ResolveDeclaredElement();
                if (declaredElement != null)
                    return GetDeclaredElementSourceLocation(declaredElement);
            }

            return CodeLocation.Unknown;
        }
        #endregion

        #region Events
        protected override EventAttributes GetEventAttributes(StaticEventWrapper @event)
        {
            return EventAttributes.None;
        }

        protected override StaticMethodWrapper GetEventAddMethod(StaticEventWrapper @event)
        {
            IMetadataEvent eventHandle = (IMetadataEvent) @event.Handle;
            return WrapAccessor(eventHandle.Adder, @event);
        }

        protected override StaticMethodWrapper GetEventRaiseMethod(StaticEventWrapper @event)
        {
            IMetadataEvent eventHandle = (IMetadataEvent)@event.Handle;
            return WrapAccessor(eventHandle.Raiser, @event);
        }

        protected override StaticMethodWrapper GetEventRemoveMethod(StaticEventWrapper @event)
        {
            IMetadataEvent eventHandle = (IMetadataEvent)@event.Handle;
            return WrapAccessor(eventHandle.Remover, @event);
        }

        protected override StaticTypeWrapper GetEventHandlerType(StaticEventWrapper @event)
        {
            IMetadataEvent eventHandle = (IMetadataEvent)@event.Handle;
            return MakeType(eventHandle.Type);
        }
        #endregion

        #region Fields
        protected override FieldAttributes GetFieldAttributes(StaticFieldWrapper field)
        {
            IMetadataField fieldHandle = (IMetadataField)field.Handle;

            FieldAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Assembly, fieldHandle.IsAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Family, fieldHandle.IsFamily);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.FamANDAssem, fieldHandle.IsFamilyAndAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.FamORAssem, fieldHandle.IsFamilyOrAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Private, fieldHandle.IsPrivate);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Public, fieldHandle.IsPublic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.SpecialName, fieldHandle.IsSpecialName);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Static, fieldHandle.IsStatic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.Literal | FieldAttributes.HasDefault, fieldHandle.IsLiteral);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, FieldAttributes.InitOnly, fieldHandle.IsInitOnly);
            return flags;
        }

        protected override StaticTypeWrapper GetFieldType(StaticFieldWrapper field)
        {
            IMetadataField fieldHandle = (IMetadataField)field.Handle;
            return MakeType(fieldHandle.Type);
        }
        #endregion

        #region Properties
        protected override PropertyAttributes GetPropertyAttributes(StaticPropertyWrapper property)
        {
            // Note: There don't seem to be any usable property attributes.
            return 0;
        }

        protected override StaticTypeWrapper GetPropertyType(StaticPropertyWrapper property)
        {
            IMetadataProperty propertyHandle = (IMetadataProperty)property.Handle;
            return MakeType(propertyHandle.Type);
        }

        protected override StaticMethodWrapper GetPropertyGetMethod(StaticPropertyWrapper property)
        {
            IMetadataProperty propertyHandle = (IMetadataProperty)property.Handle;
            return WrapAccessor(propertyHandle.Getter, property);
        }

        protected override StaticMethodWrapper GetPropertySetMethod(StaticPropertyWrapper property)
        {
            IMetadataProperty propertyHandle = (IMetadataProperty)property.Handle;
            return WrapAccessor(propertyHandle.Setter, property);
        }
        #endregion

        #region Functions
        protected override MethodAttributes GetFunctionAttributes(StaticFunctionWrapper function)
        {
            IMetadataMethod methodHandle = (IMetadataMethod)function.Handle;

            MethodAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Abstract, methodHandle.IsAbstract);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Assembly, methodHandle.IsAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Family, methodHandle.IsFamily);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.FamANDAssem, methodHandle.IsFamilyAndAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.FamORAssem, methodHandle.IsFamilyOrAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Final, methodHandle.IsFinal);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.HideBySig, methodHandle.IsHideBySig);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.NewSlot, methodHandle.IsNewSlot);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Private, methodHandle.IsPrivate);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Public, methodHandle.IsPublic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.SpecialName, methodHandle.IsSpecialName);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Static, methodHandle.IsStatic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, MethodAttributes.Virtual, methodHandle.IsVirtual);
            return flags;
        }

        protected override CallingConventions GetFunctionCallingConvention(StaticFunctionWrapper function)
        {
            IMetadataMethod methodHandle = (IMetadataMethod)function.Handle;
            
            CallingConventions flags = CallingConventions.Standard;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.VarArgs, methodHandle.IsVarArg);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, CallingConventions.HasThis, ! methodHandle.IsStatic);
            return flags;
        }

        protected override IList<StaticParameterWrapper> GetFunctionParameters(StaticFunctionWrapper function)
        {
            IMetadataMethod methodHandle = (IMetadataMethod)function.Handle;
            return Array.ConvertAll<IMetadataParameter, StaticParameterWrapper>(methodHandle.Parameters, delegate(IMetadataParameter parameter)
            {
                return new StaticParameterWrapper(this, parameter, function);
            });
        }
        #endregion

        #region Methods
        protected override StaticParameterWrapper GetMethodReturnParameter(StaticMethodWrapper method)
        {
            IMetadataMethod methodHandle = (IMetadataMethod)method.Handle;
            return new StaticParameterWrapper(this, methodHandle.ReturnValue, method);
        }

        protected override IList<StaticGenericParameterWrapper> GetMethodGenericParameters(StaticMethodWrapper method)
        {
            IMetadataMethod methodHandle = (IMetadataMethod)method.Handle;
            return Array.ConvertAll<IMetadataGenericArgument, StaticGenericParameterWrapper>(methodHandle.GenericArguments, delegate(IMetadataGenericArgument parameterHandle)
            {
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, parameterHandle, method);
            });
        }

        private static bool IsConstructor(IMetadataMethod method)
        {
            string name = method.Name;
            return name == ".ctor" || name == ".cctor";
        }
        #endregion

        #region Parameters
        protected override ParameterAttributes GetParameterAttributes(StaticParameterWrapper parameter)
        {
            IMetadataParameter parameterHandle = parameter.Handle as IMetadataParameter;
            if (parameterHandle != null)
            {
                ParameterAttributes flags = 0;
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.In, parameterHandle.IsIn);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Out, parameterHandle.IsOut);
                ReflectorFlagsUtils.AddFlagIfTrue(ref flags, ParameterAttributes.Optional, parameterHandle.IsOptional);
                return flags;
            }

            return ParameterAttributes.None;
        }

        protected override IEnumerable<StaticAttributeWrapper> GetParameterCustomAttributes(StaticParameterWrapper parameter)
        {
            IMetadataEntity entityHandle = (IMetadataEntity)parameter.Handle;
            return EnumerateAttributesForEntity(entityHandle);
        }

        protected override string GetParameterName(StaticParameterWrapper parameter)
        {
            IMetadataParameter parameterHandle = parameter.Handle as IMetadataParameter;
            if (parameterHandle != null)
                return parameterHandle.Name;

            return null;
        }

        protected override int GetParameterPosition(StaticParameterWrapper parameter)
        {
            IMetadataParameter parameterHandle = parameter.Handle as IMetadataParameter;
            if (parameterHandle != null)
                return Array.IndexOf(parameterHandle.DeclaringMethod.Parameters, parameterHandle); 

            return -1;
        }

        protected override StaticTypeWrapper GetParameterType(StaticParameterWrapper parameter)
        {
            IMetadataParameter parameterHandle = parameter.Handle as IMetadataParameter;
            if (parameterHandle != null)
                return MakeType(parameterHandle.Type);

            IMetadataReturnValue returnValueHandle = (IMetadataReturnValue)parameter.Handle;
            return MakeType(returnValueHandle.Type);
        }
        #endregion

        #region Types
        protected override TypeAttributes GetTypeAttributes(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            TypeAttributes flags = 0;
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Abstract, typeHandle.IsAbstract);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Class, typeHandle.IsClass);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Interface, typeHandle.IsInterface);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedAssembly, typeHandle.IsNestedAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamily, typeHandle.IsNestedFamily);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamANDAssem, typeHandle.IsNestedFamilyAndAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedFamORAssem, typeHandle.IsNestedFamilyOrAssembly);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedPrivate, typeHandle.IsNestedPrivate);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NestedPublic, typeHandle.IsNestedPublic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Public, typeHandle.IsPublic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.NotPublic, typeHandle.IsNotPublic);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Sealed, typeHandle.IsSealed);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.Serializable, typeHandle.IsSerializable);
            ReflectorFlagsUtils.AddFlagIfTrue(ref flags, TypeAttributes.SpecialName, typeHandle.IsSpecialName);
            return flags;
        }

        protected override IList<StaticGenericParameterWrapper> GetTypeGenericParameters(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;
            return Array.ConvertAll<IMetadataGenericArgument, StaticGenericParameterWrapper>(typeHandle.GenericParameters, delegate(IMetadataGenericArgument parameterHandle)
            {
                return StaticGenericParameterWrapper.CreateGenericTypeParameter(this, parameterHandle, type);
            });
        }

        protected override StaticAssemblyWrapper GetTypeAssembly(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;
            return Wrap(GetMetadataAssemblyHack(typeHandle));
        }

        protected override string GetTypeNamespace(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;
#if RESHARPER_60_OR_NEWER
            return new ClrTypeName(typeHandle.FullyQualifiedName).GetNamespaceName();
#else
            return new CLRTypeName(typeHandle.FullyQualifiedName).NamespaceName;
#endif
        }

        protected override StaticDeclaredTypeWrapper GetTypeBaseType(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;
            IMetadataClassType baseClassTypeHandle = typeHandle.Base;
            return baseClassTypeHandle != null ? MakeDeclaredType(baseClassTypeHandle) : null;
        }

        protected override IList<StaticDeclaredTypeWrapper> GetTypeInterfaces(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;
            return Array.ConvertAll<IMetadataClassType, StaticDeclaredTypeWrapper>(typeHandle.Interfaces, MakeDeclaredType);
        }

        protected override IEnumerable<StaticConstructorWrapper> GetTypeConstructors(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            foreach (IMetadataMethod methodHandle in typeHandle.GetMethods())
            {
                if (IsConstructor(methodHandle))
                    yield return new StaticConstructorWrapper(this, methodHandle, type);
            }
        }

        protected override IEnumerable<StaticMethodWrapper> GetTypeMethods(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            foreach (IMetadataMethod methodHandle in typeHandle.GetMethods())
            {
                if (!IsConstructor(methodHandle))
                    yield return new StaticMethodWrapper(this, methodHandle, type, reflectedType, type.Substitution);
            }
        }

        protected override IEnumerable<StaticPropertyWrapper> GetTypeProperties(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            foreach (IMetadataProperty propertyHandle in typeHandle.GetProperties())
                yield return new StaticPropertyWrapper(this, propertyHandle, type, reflectedType);
        }

        protected override IEnumerable<StaticFieldWrapper> GetTypeFields(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            foreach (IMetadataField fieldHandle in typeHandle.GetFields())
                yield return new StaticFieldWrapper(this, fieldHandle, type, reflectedType);
        }

        protected override IEnumerable<StaticEventWrapper> GetTypeEvents(StaticDeclaredTypeWrapper type,
            StaticDeclaredTypeWrapper reflectedType)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            foreach (IMetadataEvent eventHandle in typeHandle.GetEvents())
                yield return new StaticEventWrapper(this, eventHandle, type, reflectedType);
        }

        protected override IEnumerable<StaticTypeWrapper> GetTypeNestedTypes(StaticDeclaredTypeWrapper type)
        {
            IMetadataTypeInfo typeHandle = (IMetadataTypeInfo)type.Handle;

            foreach (IMetadataTypeInfo nestedTypeHandle in typeHandle.GetNestedTypes())
                yield return new StaticDeclaredTypeWrapper(this, nestedTypeHandle, type, type.Substitution);
        }

        private StaticTypeWrapper MakeType(IMetadataType typeHandle)
        {
            return typeMemoizer.Memoize(typeHandle, () =>
            {
                IMetadataClassType classTypeHandle = typeHandle as IMetadataClassType;
                if (classTypeHandle != null)
                    return MakeDeclaredType(classTypeHandle);

                IMetadataArrayType arrayTypeHandle = typeHandle as IMetadataArrayType;
                if (arrayTypeHandle != null)
                    return MakeArrayType(arrayTypeHandle);

                IMetadataPointerType pointerTypeHandle = typeHandle as IMetadataPointerType;
                if (pointerTypeHandle != null)
                    return MakePointerType(pointerTypeHandle);

                IMetadataReferenceType referenceTypeHandle = typeHandle as IMetadataReferenceType;
                if (referenceTypeHandle != null)
                    return MakeByRefType(referenceTypeHandle);

                IMetadataGenericArgumentReferenceType argumentTypeHandle =
                    typeHandle as IMetadataGenericArgumentReferenceType;
                if (argumentTypeHandle != null)
                    return MakeGenericParameter(argumentTypeHandle);

                throw new NotSupportedException("Unsupported type: " + typeHandle);
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredTypeWithoutSubstitution(IMetadataTypeInfo typeHandle)
        {
            return typeWithoutSubstitutionMemoizer.Memoize(typeHandle, () =>
            {
                return MakeDeclaredType(typeHandle, Common.Collections.EmptyArray<IMetadataType>.Instance);
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(IMetadataClassType typeHandle)
        {
            return classMemoizer.Memoize(typeHandle, () =>
            {
                return MakeDeclaredType(typeHandle.Type, typeHandle.Arguments);
            });
        }

        private StaticDeclaredTypeWrapper MakeDeclaredType(IMetadataTypeInfo typeInfoHandle, IMetadataType[] argumentTypeHandles)
        {
            typeInfoHandle = ResolveMetadataTypeInfoHack(typeInfoHandle);

            IMetadataTypeInfo declaringTypeInfoHandle = ResolveMetadataTypeInfoHack(typeInfoHandle.DeclaringType);
            StaticDeclaredTypeWrapper type;
            if (declaringTypeInfoHandle != null)
            {
                StaticDeclaredTypeWrapper declaringType = MakeDeclaredType(declaringTypeInfoHandle, Common.Collections.EmptyArray<IMetadataType>.Instance);
                type = new StaticDeclaredTypeWrapper(this, typeInfoHandle, declaringType, declaringType.Substitution);
            }
            else
            {
                type = new StaticDeclaredTypeWrapper(this, typeInfoHandle, null, StaticTypeSubstitution.Empty);
            }

            if (argumentTypeHandles.Length == 0)
                return type;

            ITypeInfo[] genericArguments = Array.ConvertAll<IMetadataType, ITypeInfo>(argumentTypeHandles, delegate(IMetadataType argumentTypeHandle)
            {
                return MakeType(argumentTypeHandle);
            }); 
            return type.MakeGenericType(genericArguments);
        }

        private StaticArrayTypeWrapper MakeArrayType(IMetadataArrayType arrayTypeHandle)
        {
            return MakeType(arrayTypeHandle.ElementType).MakeArrayType((int)arrayTypeHandle.Rank);
        }

        private StaticPointerTypeWrapper MakePointerType(IMetadataPointerType pointerTypeHandle)
        {
            return MakeType(pointerTypeHandle.Type).MakePointerType();
        }

        private StaticByRefTypeWrapper MakeByRefType(IMetadataReferenceType referenceTypeHandle)
        {
            return MakeType(referenceTypeHandle.Type).MakeByRefType();
        }

        private StaticGenericParameterWrapper MakeGenericParameter(IMetadataGenericArgumentReferenceType parameterTypeHandle)
        {
            return MakeGenericParameter(parameterTypeHandle.Argument);
        }

        private StaticGenericParameterWrapper MakeGenericParameter(IMetadataGenericArgument parameterHandle)
        {
            if (parameterHandle.TypeOwner != null)
            {
                StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(parameterHandle.TypeOwner);
                return StaticGenericParameterWrapper.CreateGenericTypeParameter(this, parameterHandle, declaringType);
            }
            else
            {
                StaticDeclaredTypeWrapper declaringType = MakeDeclaredTypeWithoutSubstitution(parameterHandle.MethodOwner.DeclaringType);
                StaticMethodWrapper declaringMethod = new StaticMethodWrapper(this, parameterHandle.MethodOwner, declaringType, declaringType, declaringType.Substitution);
                return StaticGenericParameterWrapper.CreateGenericMethodParameter(this, parameterHandle, declaringMethod);
            }
        }
        #endregion

        #region Generic Parameters
        protected override GenericParameterAttributes GetGenericParameterAttributes(StaticGenericParameterWrapper genericParameter)
        {
            IMetadataGenericArgument parameterHandle = (IMetadataGenericArgument) genericParameter.Handle;

            // Note: The values are defined in exactly the same way, it's just the type that's different.
            return (GenericParameterAttributes)parameterHandle.Attributes;
        }

        protected override int GetGenericParameterPosition(StaticGenericParameterWrapper genericParameter)
        {
            IMetadataGenericArgument parameterHandle = (IMetadataGenericArgument) genericParameter.Handle;
            return (int)parameterHandle.Index;
        }

        protected override IList<StaticTypeWrapper> GetGenericParameterConstraints(StaticGenericParameterWrapper genericParameter)
        {
            IMetadataGenericArgument parameterHandle = (IMetadataGenericArgument)genericParameter.Handle;
            return Array.ConvertAll<IMetadataType, StaticTypeWrapper>(parameterHandle.TypeConstraints, MakeType);
        }
        #endregion

        #region GetDeclaredElement and GetProject
        protected override IDeclaredElementResolver GetDeclaredElementResolver(StaticWrapper element)
        {
#if !RESHARPER_61_OR_NEWER
            return DeclaredElementResolver.CreateResolver(contextProject, element);
#else
			return DeclaredElementResolver.CreateResolver(contextProject, element, cacheManager);
#endif
		}

        protected override IProject GetProject(StaticWrapper element)
        {
            return contextProject;
        }

        private sealed class DeclaredElementResolver : IDeclaredElementResolver
        {
            private readonly IProject project;
            private readonly Common.Func<IProject, IDeclaredElement> provider;
#if RESHARPER_61_OR_NEWER
        	private static CacheManager cacheManager;
#endif

#if !RESHARPER_61_OR_NEWER
            private DeclaredElementResolver(IProject project, Common.Func<IProject, IDeclaredElement> provider)
#else
			private DeclaredElementResolver(IProject project, Common.Func<IProject, IDeclaredElement> provider, CacheManager cacheManager)
#endif
            {
                this.project = project;
                this.provider = provider;
#if RESHARPER_61_OR_NEWER
				DeclaredElementResolver.cacheManager = cacheManager;
#endif
            }

            public IDeclaredElement ResolveDeclaredElement()
            {
                if (provider == null)
                    return null;

#if !RESHARPER_60
                using (ReadLockCookie.Create())
#endif
				{
                    return provider(project);
                }
            }

#if !RESHARPER_61_OR_NEWER
            public static IDeclaredElementResolver CreateResolver(IProject project, StaticWrapper element)
            {
                return new DeclaredElementResolver(project, project != null ? GetProvider(element) : null);
			}
#else
			public static IDeclaredElementResolver CreateResolver(IProject project, StaticWrapper element, CacheManager cacheManager)
			{
				return new DeclaredElementResolver(project, project != null ? GetProvider(element) : null, cacheManager);
			}
#endif

            private static Common.Func<IProject, IDeclaredElement> ToDeclaredElementProvider<T>(Common.Func<IProject, T> provider)
                where T : IDeclaredElement
            {
                return project => provider(project);
            }

            private static Common.Func<IProject, IDeclaredElement> GetProvider(StaticWrapper element)
            {
                IMetadataTypeInfo type = element.Handle as IMetadataTypeInfo;
                if (type != null)
                    return ToDeclaredElementProvider(GetProvider(type));

                IMetadataMethod method = element.Handle as IMetadataMethod;
                if (method != null)
                    return ToDeclaredElementProvider(GetProvider(method));

                IMetadataProperty property = element.Handle as IMetadataProperty;
                if (property != null)
                    return ToDeclaredElementProvider(GetProvider(property));

                IMetadataField field = element.Handle as IMetadataField;
                if (field != null)
                    return ToDeclaredElementProvider(GetProvider(field));

                IMetadataEvent @event = element.Handle as IMetadataEvent;
                if (@event != null)
                    return ToDeclaredElementProvider(GetProvider(@event));

                IMetadataParameter parameter = element.Handle as IMetadataParameter;
                if (parameter != null)
                    return ToDeclaredElementProvider(GetProvider(parameter));

                IMetadataReturnValue returnValue = element.Handle as IMetadataReturnValue;
                if (returnValue != null)
                    return ToDeclaredElementProvider(GetProvider(returnValue));

                return project => null;
            }

            private static Common.Func<IProject, ITypeElement> GetProvider(IMetadataTypeInfo type)
            {
                string clrName = type.FullyQualifiedName;

                return project =>
                {
                    IDeclarationsCache cache = GetDeclarationsCache(project);

                    // TODO: Verify expected assembly name in case there are multiple types with
                    //       the same name distinguished only by declaring assembly.
                    return cache.GetTypeElementByCLRName(clrName);
                };
            }

            private static Common.Func<IProject, IFunction> GetProvider(IMetadataMethod metadataMethod)
            {
                Common.Func<IProject, ITypeElement> typeProvider = GetProvider(metadataMethod.DeclaringType);
                bool isStatic = metadataMethod.IsStatic;
                MethodParameterInfo[] parameters = MethodParameterInfo.CreateParameterListFromMetadata(metadataMethod.Parameters);

                if (IsConstructor(metadataMethod))
                {
                    return project =>
                    {
                        ITypeElement type = typeProvider(project);
                        if (type != null)
                        {
                            foreach (IConstructor constructor in type.Constructors)
                            {
                                if (constructor.IsStatic == isStatic
                                    && MethodParameterInfo.IsMatchingParameterList(parameters, constructor.Parameters))
                                    return constructor;
                            }
                        }

                        return null;
                    };
                }

                string name = metadataMethod.Name;
                return project =>
                {
                    ITypeElement type = typeProvider(project);
                    if (type != null)
                    {
                        foreach (IMethod method in type.Methods)
                        {
                            if (method.IsStatic == isStatic
                                && method.ShortName == name
                                && MethodParameterInfo.IsMatchingParameterList(parameters, method.Parameters))
                                return method;
                        }

                        foreach (IOperator @operator in type.Operators)
                        {
                            if (@operator.IsStatic == isStatic
                                && @operator.ShortName == name
                                && MethodParameterInfo.IsMatchingParameterList(parameters, @operator.Parameters))
                                return @operator;
                        }
                    }

                    return null;
                };
            }

            private static Common.Func<IProject, IProperty> GetProvider(IMetadataProperty metadataProperty)
            {
                Common.Func<IProject, ITypeElement> typeProvider = GetProvider(metadataProperty.DeclaringType);
                string name = metadataProperty.Name;

                return project =>
                {
                    ITypeElement type = typeProvider(project);
                    if (type != null)
                    {
                        // TODO: Handle overloaded indexer properties.
                        foreach (IProperty property in type.Properties)
                            if (property.ShortName == name)
                                return property;
                    }

                    return null;
                };
            }

            private static Common.Func<IProject, IField> GetProvider(IMetadataField metadataField)
            {
                Common.Func<IProject, ITypeElement> typeProvider = GetProvider(metadataField.DeclaringType);
                string name = metadataField.Name;

                return project =>
                {
                    ITypeElement type = typeProvider(project);

                    IClass classHandle = type as IClass;
                    if (classHandle != null)
                    {
                        foreach (IField field in classHandle.Fields)
                            if (field.ShortName == name)
                                return field;
                        foreach (IField field in classHandle.Constants)
                            if (field.ShortName == name)
                                return field;
                    }

                    IStruct structHandle = type as IStruct;
                    if (structHandle != null)
                    {
                        foreach (IField field in structHandle.Fields)
                            if (field.ShortName == name)
                                return field;
                        foreach (IField field in structHandle.Constants)
                            if (field.ShortName == name)
                                return field;
                    }

                    return null;
                };
            }

            private static Common.Func<IProject, IEvent> GetProvider(IMetadataEvent metadataEvent)
            {
                Common.Func<IProject, ITypeElement> typeProvider = GetProvider(metadataEvent.DeclaringType);
                string name = metadataEvent.Name;

                return project =>
                {
                    ITypeElement type = typeProvider(project);
                    if (type != null)
                    {
                        foreach (IEvent @event in type.Events)
                            if (@event.ShortName == name)
                                return @event;
                    }

                    return null;
                };
            }

            private static Common.Func<IProject, IParameter> GetProvider(IMetadataParameter metadataParameter)
            {
                Common.Func<IProject, IFunction> functionProvider = GetProvider(metadataParameter.DeclaringMethod);
                string name = metadataParameter.Name;

                return project =>
                {
                    IFunction function = functionProvider(project);
                    if (function != null)
                    {
                        foreach (IParameter parameter in function.Parameters)
                            if (parameter.ShortName == name)
                                return parameter;
                    }

                    return null;
                };
            }

            private static Common.Func<IProject, IParameter> GetProvider(IMetadataReturnValue metadataParameter)
            {
                // FIXME: Not sure which ReSharper code model element represents a return value.
                return project => null;
            }

            private static IDeclarationsCache GetDeclarationsCache(IProject contextProject)
            {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
                return PsiManager.GetInstance(contextProject.GetSolution()).
                    GetDeclarationsCache(DeclarationsCacheScope.ProjectScope(contextProject, true), true);
#elif RESHARPER_60_OR_NEWER
				var module = PsiModuleManager.GetInstance(contextProject.GetSolution()).GetPrimaryPsiModule(contextProject);
#if RESHARPER_60
            	var cacheManager = CacheManager.GetInstance(contextProject.GetSolution());
#endif
            	return cacheManager.GetDeclarationsCache(module, false, false);
#else
                IPsiModule module = PsiModuleManager.GetInstance(contextProject.GetSolution()).GetPsiModule(contextProject.ProjectFile);
                return PsiManager.GetInstance(contextProject.GetSolution()).
                    GetDeclarationsCache(DeclarationsScopeFactory.ModuleScope(module, true), true);
#endif
            }
        }

        private sealed class MethodParameterInfo : IEquatable<MethodParameterInfo>
        {
            public string Name { get; private set; }
            public bool IsOptional { get; private set; }
            public bool IsIn { get; private set; }
            public bool IsOut { get; private set; }
            public string TypeName { get; private set; }

            public static MethodParameterInfo[] CreateParameterListFromMetadata(IMetadataParameter[] parameters)
            {
                return GenericCollectionUtils.ConvertAllToArray<IMetadataParameter, MethodParameterInfo>(parameters, CreateFromMetadata);
            }

            public static bool IsMatchingParameterList(MethodParameterInfo[] a, IList<IParameter> b)
            {
                if (a.Length != b.Count)
                    return false;

                for (int i = 0; i < a.Length; i++)
                {
                    MethodParameterInfo aParameterInfo = a[i];
                    MethodParameterInfo bParameterInfo = MethodParameterInfo.CreateFromPsi(b[i]);
                    if (!aParameterInfo.Equals(bParameterInfo))
                        return false;
                }

                return true;
            }


            public static MethodParameterInfo CreateFromMetadata(IMetadataParameter parameter)
            {
                return new MethodParameterInfo()
                {
                    Name = parameter.Name,
                    IsOptional = parameter.IsOptional,
                    IsIn = parameter.IsIn,
                    IsOut = parameter.IsOut,
#if ! RESHARPER_50_OR_NEWER
                    TypeName = parameter.Type.PresentableName
#else
                    TypeName = parameter.Type.FullName
#endif
                };
            }

            public static MethodParameterInfo CreateFromPsi(IParameter parameter)
            {
                return new MethodParameterInfo()
                {
                    Name = parameter.ShortName,
                    IsOptional = parameter.IsOptional,
                    IsIn = parameter.Kind != ParameterKind.OUTPUT,
                    IsOut = parameter.Kind != ParameterKind.VALUE,
#if ! RESHARPER_50_OR_NEWER
                    TypeName = parameter.Type.GetPresentableName(PsiLanguageType.UNKNOWN)
#elif RESHARPER_60_OR_NEWER
					TypeName = parameter.Type.GetLongPresentableName(UnknownLanguage.Instance)
#else
                    TypeName = parameter.Type.GetLongPresentableName(PsiLanguageType.UNKNOWN)
#endif
                };
            }

            public bool Equals(MethodParameterInfo other)
            {
                return other != null
                    && Name == other.Name
                    && IsOptional == other.IsOptional
                    && IsIn == other.IsIn
                    && IsOut == other.IsOut
                    && TypeName == other.TypeName;
            }
        }
        #endregion

        #region HACKS
        private IMetadataAssembly GetMetadataAssemblyHack(IMetadataTypeInfo typeInfo)
        {
            // HACK: This type contains a reference to its assembly but it
            //       does not expose it in a useful manner.
            FieldInfo myAssemblyField = typeInfo.GetType().GetField(@"myAssembly", BindingFlags.Instance | BindingFlags.NonPublic);

            IMetadataAssembly assembly = myAssemblyField != null ? (IMetadataAssembly)myAssemblyField.GetValue(typeInfo) : null;
            if (assembly != null)
                return assembly;

#if RESHARPER_50_OR_NEWER
            AssemblyNameInfo assemblyNameInfo = typeInfo.DeclaringAssemblyName;
            AssemblyName assemblyName = assemblyNameInfo != null ? new AssemblyName(assemblyNameInfo.FullName) : null;
#else
            AssemblyName assemblyName = typeInfo.DeclaringAssemblyName;
#endif

            // Note: ReSharper can sometimes return unresolved types (which have a null declaring assembly name).
            //       We can't really do much with these except to guess the assembly if possible.
            if (assemblyName == null)
            {
                Type type = Type.GetType(typeInfo.FullyQualifiedName);
                if (type != null)
                    assemblyName = type.Assembly.GetName();
            }

            if (assemblyName != null)
            {
                assembly = LoadMetadataAssembly(assemblyName, false);
                if (assembly != null)
                    return assembly;
            }

            throw new NotSupportedException(String.Format(
                "Cannot determine the assembly to which type '{0}' belongs because it is unresolved (ReSharper did not supply the assembly name information).", typeInfo.FullyQualifiedName));
        }

        private MetadataLoader GetMetadataLoaderHack(IMetadataAssembly assembly)
        {
            // HACK: The assembly contains a reference back to its loader
            //       which is useful for loading referenced assemblies but it
            //       does not expose it.
#if RESHARPER_70
            var loaderProperty = assembly.GetType().GetProperty(@"Loader", BindingFlags.Instance | BindingFlags.Public);
#else
            PropertyInfo loaderProperty = assembly.GetType().GetProperty(@"Loader", BindingFlags.Instance | BindingFlags.NonPublic);
#endif
            return loaderProperty != null ? (MetadataLoader)loaderProperty.GetValue(assembly, null) : null;

        }

        private IMetadataTypeInfo ResolveMetadataTypeInfoHack(IMetadataTypeInfo typeInfo)
        {
            if (typeInfo == null || typeInfo.GetType().Name != "UnresolvedTypeInfo")
                return typeInfo;

            IMetadataTypeInfo resolvedTypeInfo = GetMetadataAssemblyHack(typeInfo).GetTypeInfoFromQualifiedName(typeInfo.FullyQualifiedName, false);
            if (resolvedTypeInfo != null)
                return resolvedTypeInfo;

            throw new NotSupportedException(String.Format("Could not resolve type '{0}'.", typeInfo.FullyQualifiedName));
        }

        private IMetadataMethod ResolveMetadataMethodHack(IMetadataMethod method)
        {
            if (method == null || method.GetType().Name != "UnresolvedMethod")
                return method;

            IMetadataParameter[] methodParameters = method.Parameters;
            ITypeInfo[] methodParameterTypes = MakeParameterTypes(methodParameters);
            string methodName = method.Name;

            IMetadataTypeInfo resolvedTypeInfo = ResolveMetadataTypeInfoHack(method.DeclaringType);
            foreach (IMetadataMethod resolvedMethod in resolvedTypeInfo.GetMethods())
            {
                if (methodName != resolvedMethod.Name)
                    continue;

                IMetadataParameter[] resolvedMethodParameters = resolvedMethod.Parameters;
                if (methodParameters.Length != resolvedMethodParameters.Length)
                    continue;

                ITypeInfo[] resolvedMethodParameterTypes = MakeParameterTypes(resolvedMethodParameters);
                if (GenericCollectionUtils.ElementsEqual(methodParameterTypes, resolvedMethodParameterTypes))
                    return resolvedMethod;
            }

            throw new NotSupportedException(String.Format("Could not resolve method '{0}'.", method.Name));
        }

        private ITypeInfo[] MakeParameterTypes(IMetadataParameter[] parameters)
        {
            return GenericCollectionUtils.ConvertAllToArray<IMetadataParameter, ITypeInfo>(parameters, delegate(IMetadataParameter parameter)
            {
                return MakeType(parameter.Type);
            });
        }
        #endregion

        #region Misc
        private StaticMethodWrapper WrapAccessor(IMetadataMethod accessorHandle, StaticMemberWrapper member)
        {
            return accessorHandle != null ? new StaticMethodWrapper(this, accessorHandle, member.DeclaringType, member.ReflectedType, member.Substitution) : null;
        }
        #endregion
    }
}
