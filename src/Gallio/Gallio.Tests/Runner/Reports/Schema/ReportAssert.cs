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
using Gallio.Common.Markup;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests.Model;
using MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports.Schema
{
    /// <summary>
    /// Provides assertions for common Gallio core types.
    /// </summary>
    public static class ReportAssert
    {
        public static void AreEqual(Report expected, Report actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual(expected.TestPackage, actual.TestPackage);
            ModelAssert.AreEqual(expected.TestModel, actual.TestModel);
            AreEqual(expected.TestPackageRun, actual.TestPackageRun);
        }

        public static void AreEqual(TestPackageData expected, TestPackageData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreElementsEqual(expected.Files, actual.Files);
            Assert.AreElementsEqual(expected.HintDirectories, actual.HintDirectories);

            // TODO: Compare HostSetup objects.
        }

        public static void AreEqual(TestPackageRun expected, TestPackageRun actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
            AreEqual(expected.Statistics, actual.Statistics);

            Assert.Over.Pairs(expected.AllTestStepRuns, actual.AllTestStepRuns, AreEqual);
        }

        public static void AreEqual(Statistics expected, Statistics actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.AssertCount, actual.AssertCount);
            Assert.AreEqual(expected.Duration, actual.Duration);
            Assert.AreEqual(expected.FailedCount, actual.FailedCount);
            Assert.AreEqual(expected.InconclusiveCount, actual.InconclusiveCount);
            Assert.AreEqual(expected.PassedCount, actual.PassedCount);
            Assert.AreEqual(expected.SkippedCount, actual.SkippedCount);
            Assert.AreEqual(expected.RunCount, actual.RunCount);
            Assert.AreEqual(expected.TestCount, actual.TestCount);
        }

        public static void AreEqual(TestStepRun expected, TestStepRun actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            ModelAssert.AreEqual(expected.Step, actual.Step);
            Assert.AreEqual(expected.StartTime, actual.StartTime);
            Assert.AreEqual(expected.EndTime, actual.EndTime);
            ModelAssert.AreEqual(expected.Result, actual.Result);
            AreEqual(expected.TestLog, actual.TestLog);

            Assert.Over.Pairs(expected.Children, actual.Children, AreEqual);
        }

        public static void AreEqual(StructuredDocument expected, StructuredDocument actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.Over.Pairs(expected.Attachments, actual.Attachments, AreEqual);
            Assert.Over.Pairs(expected.Streams, actual.Streams, AreEqual);
        }

        public static void AreEqual(StructuredStream expected, StructuredStream actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ToString(), actual.ToString());

            // FIXME: not precise
        }

        public static void AreEqual(AttachmentData expected, AttachmentData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ContentType, actual.ContentType);
            Assert.AreEqual(expected.Type, actual.Type);
            Assert.AreEqual(expected.ContentPath, actual.ContentPath);
            Assert.AreEqual(expected.SerializedContents, actual.SerializedContents);
        }
    }
}