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

using System.IO;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Reflection;
using JetBrains.ProjectModel;
using JetBrains.UI;
using JetBrains.Util;
using MbUnit.Framework;
using Action = Gallio.Common.Action;

#if RESHARPER_31
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Shell.Test;
#else
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.Application.Test;
#if RESHARPER_40 || RESHARPER_41
using Resources;
#endif
#endif

namespace Gallio.ReSharperRunner.Tests
{
    /// <summary>
    /// This class helps to configure a ReSharper test shell with a limited
    /// subset of ReSharper assemblies that we require for intergration
    /// testing purposes.
    /// </summary>
    [AssemblyFixture]
    public static class ReSharperTestHarness
    {
        private static bool isTestSolutionLoaded;

        [FixtureSetUp]
        public static void SetUp()
        {
            GallioTestShellHandler.Initialize();
        }

        [FixtureTearDown]
        public static void TearDown()
        {
            if (isTestSolutionLoaded)
            {
                GallioTestShellHandler.RunWithWriteLock(delegate
                {
                    SolutionManager.Instance.CloseSolution(SolutionManager.Instance.CurrentSolution);
                });
                isTestSolutionLoaded = false;
            }

            GallioTestShellHandler.ShutDown();
        }

        public static void LoadTestSolutionIfNeeded()
        {
            if (isTestSolutionLoaded)
                return;

            FileSystemPath testSolutionPath = new FileSystemPath(
                Path.Combine(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GallioTestShellHandler.TestAssembly)),
                    @"..\..\TestSolution" + GallioTestShellHandler.VersionSuffix + ".sln"));

            GallioTestShellHandler.RunWithWriteLock(delegate
            {
#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
                SolutionManager.Instance.OpenSolution(testSolutionPath, new SimpleTaskExecutor());
#else
                SolutionManager.Instance.OpenSolution(testSolutionPath, SimpleTaskExecutor.Instance);
#endif
            });

            isTestSolutionLoaded = true;
        }
    }
}
