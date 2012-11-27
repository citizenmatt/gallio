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
using System.Drawing;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;

namespace Gallio.Model
{
    /// <summary>
    /// Describes traits of an <see cref="ITestFramework"/> component.
    /// </summary>
    public class TestFrameworkTraits : Traits
    {
        private readonly string name;
        private AssemblySignature[] frameworkAssemblies;
        private string[] fileTypes;

        /// <summary>
        /// Creates test framework traits.
        /// </summary>
        /// <param name="name">The framework's display name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public TestFrameworkTraits(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        /// <summary>
        /// Gets the framework's display name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets the framework's icon, or null if none.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// The version of the test framework.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the list of framework assembly signatures that are recognized and supported
        /// by this framework component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The framework manager consults this list to determine whether the framework component
        /// should participate in test exploration.  If none of the named framework assemblies are found
        /// among the test assembly references then the framework component will not be instantiated
        /// during test exploration in order to improve performance.
        /// </para>
        /// <para>
        /// Refer to <see cref="AssemblySignature.Parse" /> for a description of the format of an assembly signature.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public AssemblySignature[] FrameworkAssemblies
        {
            get { return frameworkAssemblies ?? EmptyArray<AssemblySignature>.Instance; }
            set
            {
                if (value == null || Array.IndexOf(value, null) >= 0)
                    throw new ArgumentNullException("value");
                frameworkAssemblies = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of file types ids recognized by the framework as test files.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The framework manager consults this list to determine whether the framework component
        /// should participate in test exploration.  If none of the file types match the files
        /// specified by the test source then the framework component will not be instantiated
        /// during test exploration in order to improve performance.
        /// </para>
        /// <para>
        /// For .Net-based test frameworks, the list of file types should probably include "Assembly"
        /// or one of its subtypes.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public string[] FileTypes
        {
            get { return fileTypes ?? EmptyArray<string>.Instance; }
            set
            {
                if (value == null || Array.IndexOf(value, null) >= 0)
                    throw new ArgumentNullException("value");
                fileTypes = value;
            }
        }

        /// <summary>
        /// Returns true if at least one of the assembly references is in <see cref="FrameworkAssemblies"/>.
        /// </summary>
        /// <param name="assemblyReferences">The assembly references.</param>
        /// <returns>True if the framework is compatible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyReferences"/> is null.</exception>
        public bool IsFrameworkCompatibleWithAssemblyReferences(IEnumerable<AssemblyName> assemblyReferences)
        {
            if (assemblyReferences == null)
                throw new ArgumentNullException("assemblyReferences");

            foreach (AssemblyName referencedAssemblyName in assemblyReferences)
            {
                foreach (AssemblySignature assemblySignature in FrameworkAssemblies)
                {
                    if (assemblySignature.IsMatch(referencedAssemblyName))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the file type is the same or a subtype of one of those specified by <see cref="FileTypes"/>.
        /// </summary>
        /// <param name="fileType">The file type.</param>
        /// <param name="fileTypeManager">The file type manager used to resolve file type ids to file types.</param>
        /// <returns>True if the framework is compatible.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileType"/> or <paramref name="fileTypeManager"/> is null.</exception>
        public bool IsFrameworkCompatibleWithFileType(FileType fileType, IFileTypeManager fileTypeManager)
        {
            if (fileType == null)
                throw new ArgumentNullException("fileType");
            if (fileTypeManager == null)
                throw new ArgumentNullException("fileTypeManager");

            foreach (string fileTypeId in FileTypes)
            {
                FileType supportedFileType = fileTypeManager.GetFileTypeById(fileTypeId);
                if (fileType.IsSameOrSubtypeOf(supportedFileType))
                    return true;
            }

            return false;
        }
    }
}
