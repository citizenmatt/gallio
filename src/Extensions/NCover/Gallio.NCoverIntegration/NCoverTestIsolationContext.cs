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
using System.IO;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Model.Isolation;
using Gallio.NCoverIntegration.Tools;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.NCoverIntegration
{
    /// <summary>
    /// An NCover test isolation context.
    /// </summary>
    public class NCoverTestIsolationContext : HostedTestIsolationContext
    {
        private readonly NCoverVersion version;
        private Batch batch;

        /// <summary>
        /// Creates an NCover test isolation context.
        /// </summary>
        /// <param name="testIsolationOptions">The test isolation options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="runtime">The runtime.</param>
        /// <param name="version">The NCover version.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testIsolationOptions" />,
        /// <paramref name="logger"/> or <paramref name="runtime"/> is null.</exception>
        public NCoverTestIsolationContext(TestIsolationOptions testIsolationOptions, ILogger logger,
            IRuntime runtime, NCoverVersion version)
            : base(new NCoverHostFactory(runtime, version), testIsolationOptions, logger)
        {
            this.version = version;
        }

        /// <inheritdoc />
        protected override object RunIsolatedTaskInHost<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
        {
            NCoverTool tool = NCoverTool.GetInstance(version, hostSetup.ProcessorArchitecture);
            if (!tool.IsInstalled())
                throw new TestIsolationException(string.Format("{0} does not appear to be installed.", tool.Name));

            string ncoverArguments, ncoverCoverageFile;
            NCoverHost.GetNCoverProperties(hostSetup, out ncoverArguments, out ncoverCoverageFile);

            if (File.Exists(ncoverCoverageFile))
                File.Delete(ncoverCoverageFile);

            if (batch != null)
            {
                hostSetup = hostSetup.Copy();

                string tempCoverageFile = batch.Enlist(ncoverCoverageFile);
                NCoverHost.SetNCoverCoverageFile(hostSetup, tempCoverageFile);
            }

            return base.RunIsolatedTaskInHost<TIsolatedTask>(hostSetup, statusReporter, args);
        }

        /// <inheritdoc />
        protected override IDisposable BeginBatchImpl(StatusReporter statusReporter)
        {
            batch = new Batch(version, statusReporter, Logger);
            return batch;
        }

        private sealed class Batch : IDisposable
        {
            private readonly NCoverVersion version;
            private readonly StatusReporter statusReporter;
            private readonly MultiMap<string, string> coverageFiles;
            private readonly ILogger logger;

            public Batch(NCoverVersion version, StatusReporter statusReporter, ILogger logger)
            {
                this.version = version;
                this.statusReporter = statusReporter;
                this.logger = logger;

                coverageFiles = new MultiMap<string, string>();
            }

            public void Dispose()
            {
                statusReporter("Merging NCover coverage files.");

                foreach (var pair in coverageFiles)
                    Merge(pair.Value, pair.Key);

                statusReporter("");
            }

            public string Enlist(string coverageFile)
            {
                string temporaryFile = SpecialPathPolicy.For("NCover").CreateTempFileWithUniqueName().FullName;
                coverageFiles.Add(coverageFile, temporaryFile);
                return temporaryFile;
            }

            private void Merge(IList<string> sources, string destination)
            {
                NCoverTool tool = NCoverTool.GetInstance(version, ProcessorArchitecture.None);
                tool.Merge(sources, destination, logger);
            }
        }
    }
}
