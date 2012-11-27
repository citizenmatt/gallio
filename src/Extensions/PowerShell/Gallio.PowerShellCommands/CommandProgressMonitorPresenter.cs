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
using System.Management.Automation;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    internal class CommandProgressMonitorPresenter : BaseProgressMonitorPresenter
    {
        private readonly BaseCommand cmdlet;

        public CommandProgressMonitorPresenter(BaseCommand cmdlet)
        {
            if (cmdlet == null)
                throw new ArgumentNullException("cmdlet");

            this.cmdlet = cmdlet;
        }

        protected override void Initialize()
        {
            ProgressMonitor.TaskFinished += HandleTaskFinished;
            ProgressMonitor.Changed += HandleChanged;

            cmdlet.StopRequested += HandleStopRequested;

            if (cmdlet.Stopping)
                ProgressMonitor.Cancel();
        }

        private void HandleTaskFinished(object sender, EventArgs e)
        {
            cmdlet.StopRequested -= HandleStopRequested;
        }

        private void HandleChanged(object sender, EventArgs e)
        {
            if (ProgressMonitor.TaskName.Length == 0)
                return;

            int percentComplete = -1;
            string status = ProgressMonitor.LeafSubTaskName;

            if (!double.IsNaN(ProgressMonitor.RemainingWorkUnits))
            {
                percentComplete = (int)Math.Ceiling((ProgressMonitor.TotalWorkUnits - ProgressMonitor.RemainingWorkUnits) * 100 / ProgressMonitor.TotalWorkUnits);
                status = String.Format("{0,3}% complete.  {1}", percentComplete, status);
            }
            else if (status.Length == 0)
            {
                status = @" ";
            }

            var progressRecord = new ProgressRecord(0, ProgressMonitor.TaskName, status);
            progressRecord.RecordType = ProgressMonitor.IsRunning ? ProgressRecordType.Processing : ProgressRecordType.Completed;
            progressRecord.CurrentOperation = ProgressMonitor.Leaf.Status;
            progressRecord.PercentComplete = percentComplete;
            cmdlet.PostMessage(() => cmdlet.WriteProgress(progressRecord));
        }

        private void HandleStopRequested(object sender, EventArgs e)
        {
            ProgressMonitor.Cancel();
        }
    }
}
