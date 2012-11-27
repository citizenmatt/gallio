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
using Gallio.Runner.Reports.Schema;
using Gallio.VisualStudio.Shell.Core;
using Gallio.VisualStudio.Shell.UI.ToolWindows;

namespace Gallio.VisualStudio.Tip.UI
{
    /// <summary>
    /// UI component which displays results details about a Gallio test.
    /// </summary>
    public partial class TestResultWindow : ToolWindow
    {
        private readonly GallioTestResult testResult;
        private readonly IList<TestStepRun> testStepRuns;

        /// <summary>
        /// Default constructor.  (Designer only)
        /// </summary>
        public TestResultWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="testResult">The test result to be displayed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testResult"/> is null.</exception>
        public TestResultWindow(GallioTestResult testResult)
        {
            if (testResult == null)
                throw new ArgumentNullException("testResult");

            this.testResult = testResult;

            testStepRuns = GallioTestResultFactory.GetTestStepRuns(testResult);

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Caption = String.Format("{0} [{1}]", testResult.TestName, Properties.Resources.Results);
            InitializeContent();
        }

        private void InitializeContent()
        {
            InitializeStatusHeader();
            InitializeRunViewer();
        }

        private void InitializeStatusHeader()
        {
            if (testResult.HasPassed)
            {
                pictureBoxStatus.Image = Properties.Resources.Passed;
                labelStatus.Text = Properties.Resources.TestHasPassed;
            }
            else if (testStepRuns.Count == 0)
            {
                pictureBoxStatus.Image = Properties.Resources.Pending;
                labelStatus.Text = Properties.Resources.TestNotRunYet;
            }
            else
            {
                pictureBoxStatus.Image = Properties.Resources.Failed;
                labelStatus.Text = Properties.Resources.TestHasFailed;
            }
        }

        private void InitializeRunViewer()
        {
            if (testStepRuns.Count != 0)
                runViewer.Show(testStepRuns);
        }

        private void pictureBoxGallioLogo_Click(object sender, EventArgs e)
        {
            ShellAccessor.Instance.DTE.ItemOperations.Navigate(
                Properties.Resources.GallioWebSiteUrl, 
                EnvDTE.vsNavigateOptions.vsNavigateOptionsNewWindow);
        }
    }
}
