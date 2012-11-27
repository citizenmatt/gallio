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

using Gallio.Common;

namespace Gallio.UI.ProgressMonitoring
{
    ///<summary>
    /// A task manager for UI applications.
    ///</summary>
    public interface ITaskManager
    {
        ///<summary>
        /// Run a task as a background action (uses ThreadPool). 
        /// No progress information will be displayed.
        ///</summary>
        ///<param name="action">The action to perform.</param>
        void BackgroundTask(Action action);
        
        ///<summary>
        /// Empty the queue of tasks.
        ///</summary>
        void ClearQueue();
        /// <inheritdoc />
        void ClearQueue(string queueId);

        ///<summary>
        /// Add a task to the queue. If nothing is in the queue or 
        /// running, then the task will be executed.
        ///</summary>
        ///<param name="command">The command to queue.</param>
        void QueueTask(ICommand command);
        /// <inheritdoc />
        void QueueTask(string queueId, ICommand command);
    }
}