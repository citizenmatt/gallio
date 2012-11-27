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
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Xml;

namespace Gallio.Runner.Reports.Schema
{
    /// <summary>
    /// Summarizes the execution of a test package for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class TestPackageRun
    {
        private Statistics statistics;

        /// <summary>
        /// Creates an empty package run.
        /// </summary>
        public TestPackageRun()
        {
        }

        /// <summary>
        /// Gets or sets the time when the package run started.
        /// </summary>
        [XmlAttribute("startTime")]
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time when the package run ended.
        /// </summary>
        [XmlAttribute("endTime")]
        public DateTime EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the root test step run, or null if the root test has not run.
        /// </summary>
        [XmlElement("testStepRun", IsNullable = false)]
        public TestStepRun RootTestStepRun
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the statistics for the package run.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlElement("statistics", Namespace = SchemaConstants.XmlNamespace, IsNullable = false)]
        public Statistics Statistics
        {
            get
            {
                if (statistics == null)
                    statistics = new Statistics();
                return statistics;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                statistics = value;
            }
        }

        /// <summary>
        /// Recursively enumerates all test step runs including the root test step run.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<TestStepRun> AllTestStepRuns
        {
            get
            {
                if (RootTestStepRun == null)
                    return EmptyArray<TestStepRun>.Instance;

                return TreeUtils.GetPreOrderTraversal(RootTestStepRun, GetChildren);
            }
        }

        private static IEnumerable<TestStepRun> GetChildren(TestStepRun node)
        {
            return node.Children;
        }

        /// <summary>
        /// Merges the specified package run into the current one.
        /// </summary>
        /// <param name="other">The other package run to merge.</param>
        public void MergeWith(TestPackageRun other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            if ((StartTime == DateTime.MinValue) || (StartTime == DateTime.MaxValue) || (other.StartTime < StartTime))
                StartTime = other.StartTime;

            if ((EndTime == DateTime.MinValue) || (EndTime == DateTime.MaxValue) || (other.EndTime > EndTime))
                EndTime = other.EndTime;

            if (RootTestStepRun == null)
            {
                RootTestStepRun = other.RootTestStepRun;
            }
            else
            {
                foreach (var child in other.RootTestStepRun.Children)
                {
                    RootTestStepRun.Children.Add(child);
                }
            }

            Statistics.MergeWith(other.Statistics);
        }
    }
}