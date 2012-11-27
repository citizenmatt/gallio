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

namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// Represents a task to be executed with a progress monitor.
    /// </summary>
    /// <param name="progressMonitor">The progress monitor, never null.</param>
    public delegate void TaskWithProgress(IProgressMonitor progressMonitor);

    /// <summary>
    /// Represents a task to be executed with a progress monitor.
    /// </summary>
    /// <param name="progressMonitor">The progress monitor, never null.</param>
    /// <typeparam name="T">The type of the result returned by the task.</typeparam>
    public delegate T TaskWithProgress<T>(IProgressMonitor progressMonitor);
}