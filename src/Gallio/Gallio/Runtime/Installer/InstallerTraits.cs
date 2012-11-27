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
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.Installer
{
    /// <summary>
    /// Traits for <see cref="IInstaller"/> components.
    /// </summary>
    public class InstallerTraits : Traits
    {
        /// <summary>
        /// Gets or sets whether an installer requires privilege elevation (admin access)
        /// to run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On Windows Vista and more recent operating systems, this will result in the
        /// appearance of a UAC Shield icon.
        /// </para>
        /// </remarks>
        public bool RequiresElevation { get; set; }
    }
}
