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
using Gallio.Common.Reflection;
using Gallio.Common.Reflection.Impl;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

#if RESHARPER_31
using JetBrains.ReSharper.Editor;
using ReSharperDocumentRange = JetBrains.ReSharper.Editor.DocumentRange;
#else
using JetBrains.DocumentModel;
using ConstantValue = Gallio.Common.Reflection.ConstantValue;
using ReSharperDocumentRange = JetBrains.DocumentModel.DocumentRange;
#endif

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Base class for ReSharper reflection policies.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Provides additional support for getting the <see cref="IProject" /> and <see cref="IDeclaredElement" />
    /// associated with a code element, if available.
    /// </para>
    /// </remarks>
    public abstract class ReSharperReflectionPolicy : StaticReflectionPolicy
    {
        /// <summary>
        /// Gets the declared element resolver associated with the code element, or null if none.
        /// </summary>
        public static IDeclaredElementResolver GetDeclaredElementResolver(ICodeElementInfo codeElement)
        {
            StaticWrapper element = codeElement as StaticWrapper;
            if (element == null)
                return null;

            ReSharperReflectionPolicy policy = element.ReflectionPolicy as ReSharperReflectionPolicy;
            return policy != null ? policy.GetDeclaredElementResolver(element) : null;
        }

        /// <summary>
        /// Gets the project to which a code element belongs, or null if none.
        /// </summary>
        public static IProject GetProject(ICodeElementInfo codeElement)
        {
            StaticWrapper element = codeElement as StaticWrapper;
            if (element == null)
                return null;

            ReSharperReflectionPolicy policy = element.ReflectionPolicy as ReSharperReflectionPolicy;
            return policy != null ? policy.GetProject(element) : null;
        }

        /// <summary>
        /// Gets the declared element resolver associated with the code element, or null if none.
        /// </summary>
        protected abstract IDeclaredElementResolver GetDeclaredElementResolver(StaticWrapper element);

        /// <summary>
        /// Gets the project to which a code element belongs, or null if none.
        /// </summary>
        protected abstract IProject GetProject(StaticWrapper element);

        /// <summary>
        /// Converts constant values from a foreign form to a native one.
        /// </summary>
        /// <param name="value">The foreign constant value.</param>
        /// <param name="typeConstructor">A function for resolving types.</param>
        /// <returns>The native constant value.</returns>
        protected static ConstantValue ConvertConstantValue<TType>(object value, Common.Func<TType, ITypeInfo> typeConstructor)
            where TType : class
        {
            if (value == null)
                return new ConstantValue(Reflector.Wrap(typeof(object)), null);

            if (value != null)
            {
                TType type = value as TType;
                if (type != null)
                    return new ConstantValue(Reflector.Wrap(typeof(Type)), typeConstructor(type));

                Type valueType = value.GetType();
                if (valueType.IsArray)
                {
                    Array valueArray = (Array) value;
                    int length = valueArray.Length;
                    ConstantValue[] array = new ConstantValue[length];

                    for (int i = 0; i < length; i++)
                        array[i] = ConvertConstantValue(valueArray.GetValue(i), typeConstructor);

                    return new ConstantValue(Reflector.Wrap(MapConstantArrayElementType<TType>(valueType).MakeArrayType()), array);
                }
            }

            return new ConstantValue(Reflector.Wrap(value.GetType()), value);
        }

        /// <summary>
        /// Gets the location associated with a declared element.
        /// </summary>
        /// <param name="declaredElement">The declared element.</param>
        /// <returns>The resulting code location.</returns>
        protected static CodeLocation GetDeclaredElementSourceLocation(IDeclaredElement declaredElement)
        {
            IList<IDeclaration> decl = declaredElement.GetDeclarations();
            if (decl.Count == 0)
                return CodeLocation.Unknown;

            ReSharperDocumentRange range = decl[0].GetDocumentRange();
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
            bool isValid = range.IsValid;
#else
            bool isValid = range.IsValid();
#endif
            if (isValid)
                return CodeLocation.Unknown;

#if RESHARPER_60_OR_NEWER
            var projectFile = decl[0].GetSourceFile().ToProjectFile();
#else
			var projectFile = decl[0].GetProjectFile();
#endif
            if (projectFile == null)
                return CodeLocation.Unknown;

            string filename = projectFile.Location.FullPath;
            DocumentCoords start = range.Document.GetCoordsByOffset(range.TextRange.StartOffset);

#if ! RESHARPER_50_OR_NEWER
            return new CodeLocation(filename, start.Line, start.Column);
#else
            return new CodeLocation(filename, (int) start.Line.Plus1(), (int) start.Column.Plus1());
#endif

        }

        private static Type MapConstantArrayElementType<TType>(Type arrayType)
        {
            Type elementType = arrayType.GetElementType();
            if (elementType.IsArray)
                return MapConstantArrayElementType<TType>(elementType).MakeArrayType();

            if (typeof(TType).IsAssignableFrom(elementType))
                return typeof(Type);

            return elementType;
        }
    }
}
