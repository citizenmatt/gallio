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

namespace Gallio.Runtime.Loader
{
    /// <summary>
    /// Provides support for resolving and loading assemblies using hint paths and custom resolvers.
    /// </summary>
    public interface IAssemblyLoader : IDisposable
    {
        /// <summary>
        /// Adds an assembly load hint directory to search when standard assembly
        /// resolution fails.
        /// </summary>
        /// <param name="hintDirectory">The hint directory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hintDirectory"/> is null.</exception>
        void AddHintDirectory(string hintDirectory);

        /// <summary>
        /// Adds a custom assembly resolver to use when standard assembly resolution fails.
        /// </summary>
        /// <param name="assemblyResolver">The assembly resolver.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyResolver"/> is null.</exception>
        void AddAssemblyResolver(IAssemblyResolver assemblyResolver);

        /// <summary>
        /// Loads an assembly from the specified file.
        /// </summary>
        /// <param name="assemblyFile">The assembly file.</param>
        /// <returns>The loaded assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="assemblyFile"/> is null.</exception>
        /// <exception cref="Exception">Thrown if the assembly could not be loaded.</exception>
        Assembly LoadAssemblyFrom(string assemblyFile);
    }
}