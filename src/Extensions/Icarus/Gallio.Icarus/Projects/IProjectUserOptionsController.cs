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

using System.Collections.Generic;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Projects
{
    public interface IProjectUserOptionsController
    {
        IEnumerable<string> CollapsedNodes { get; }
        string TreeViewCategory { get; }
        void SetCollapsedNodes(IEnumerable<string> collapsedNodes);
        void LoadUserOptions(string projectLocation);
        void SaveUserOptions(string projectName, IProgressMonitor subProgressMonitor);
    }
}
