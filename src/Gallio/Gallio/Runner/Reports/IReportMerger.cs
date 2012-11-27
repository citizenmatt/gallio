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
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A report merger provides services for merging several reports together.
    /// </summary>
    public interface IReportMerger
    {
        /// <summary>
        /// Merges the reports and returns the output merged report.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The merged report.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor "/> is null.</exception>
        Report Merge(IProgressMonitor progressMonitor);
    }
}
