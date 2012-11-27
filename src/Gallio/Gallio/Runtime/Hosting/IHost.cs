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

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// A host represents an environment that may be used to perform various
    /// services in isolation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For example, a host might provide the ability to run code in an isolated
    /// <see cref="AppDomain" /> of the current process, or it might run code
    /// in an isolated process, or connect to an existing remote process.
    /// </para>
    /// </remarks>
    public interface IHost : IDisposable
    {
        /// <summary>
        /// An event that is raised when the host is disconnected.
        /// If the host has already been disconnected, the event is fired immediately.
        /// </summary>
        /// <seealso cref="IsConnected"/>
        /// <exception cref="ObjectDisposedException">Thrown if the host has been disposed.</exception>
        event EventHandler Disconnected;

        /// <summary>
        /// Returns true if the host is local to the creating AppDomain, false if it
        /// must be accessed across a remote channel.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// A local host might not support all configuration options.
        /// </para>
        /// </remarks>
        bool IsLocal { get; }

        /// <summary>
        /// Returns true if the host is connected.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A host may become disconnected non-deterministically due to a failure in the
        /// host's communication channel or some other remote eventuality.
        /// </para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">Thrown if the host has been disposed.</exception>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a host service that can be used to perform operations within the host's environment.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the host has been disconnected.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the host has been disposed.</exception>
        IHostService GetHostService();

        /// <summary>
        /// Gets a deep copy of the host setup information.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the host has been disposed.</exception>
        HostSetup GetHostSetup();

        /// <summary>
        /// Disconnects the host.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the host has been disposed.</exception>
        void Disconnect();
    }
}
