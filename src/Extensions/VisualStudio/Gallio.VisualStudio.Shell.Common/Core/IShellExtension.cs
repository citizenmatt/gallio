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
using Gallio.Runtime.Extensibility;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Extends the <see cref="IShell" /> with additional contributions that are
    /// initialized at the same time as the shell itself.
    /// </summary>
    [Traits(typeof(ShellExtensionTraits))]
    public interface IShellExtension
    {
        /// <summary>
        /// Initializes the shell extension.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shuts down the shell extension.
        /// </summary>
        void Shutdown();
    }
}
