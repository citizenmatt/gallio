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
using System.Text;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.Debugging
{
    /// <summary>
    /// Provides control over a debugger.
    /// </summary>
    public interface IDebugger
    {
        /// <summary>
        /// Returns true if the debugger is attached to a process.
        /// </summary>
        /// <param name="process">The process to which the debugger should be attached.</param>
        /// <returns>True if the debugger is already attached.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is null.</exception>
        bool IsAttachedToProcess(Process process);

        /// <summary>
        /// Attaches the debugger to a process.
        /// </summary>
        /// <param name="process">The process to which the debugger should be attached.</param>
        /// <returns>A result code to indicate whether the debugger was attached.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is null.</exception>
        AttachDebuggerResult AttachToProcess(Process process);

        /// <summary>
        /// Detaches the debugger from a process.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does nothing if the process was not attached.
        /// </para>
        /// </remarks>
        /// <param name="process">The process from which the debugger should be detached.</param>
        /// <returns>A result code to indicate whether the debugger was detached.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="process"/> is null.</exception>
        DetachDebuggerResult DetachFromProcess(Process process);

        /// <summary>
        /// Launches a process and attaches the debugger.
        /// </summary>
        /// <param name="processStartInfo">The process start information.</param>
        /// <returns>The process that was started, or null if the process could not be launched with the debugger.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="processStartInfo"/> is null.</exception>
        Process LaunchProcess(ProcessStartInfo processStartInfo);
    }
}
