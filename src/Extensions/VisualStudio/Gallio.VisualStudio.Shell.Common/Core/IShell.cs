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
using Gallio.Runtime.Logging;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Provides services for integration with Visual Studio.
    /// </summary>
    public interface IShell
    {
        /// <summary>
        /// Returns true if the Shell has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the Visual Studio DTE object.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        object DTE { get; }

        /// <summary>
        /// Gets the Shell package object.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        object ShellPackage { get; }

        /// <summary>
        /// Gets the Shell add-in object.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        object ShellAddIn { get; }

        /// <summary>
        /// Gets the Shell add-in handler object.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        object ShellAddInHandler { get; }

        /// <summary>
        /// Gets the logger for the Shell.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Gets the Visual Studio service provider.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        IServiceProvider VsServiceProvider { get; }

        /// <summary>
        /// Gets a Visual Studio service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        object GetVsService(Type serviceType);

        /// <summary>
        /// Gets a Visual Studio service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>The service object.</returns>
        /// <typeparam name="T">The interface type.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        T GetVsService<T>(Type serviceType);

        /// <summary>
        /// Proffers a Visual Studio service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="factory">The service factory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/>
        /// or <paramref name="factory"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the Shell has not been initialized.</exception>
        void ProfferVsService(Type serviceType, ServiceFactory factory);
    }
}
