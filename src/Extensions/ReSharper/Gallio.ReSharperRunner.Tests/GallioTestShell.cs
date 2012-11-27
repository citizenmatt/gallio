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

#if ! RESHARPER_50_OR_NEWER
using System;
using System.IO;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.ReSharperRunner.Tests;
using JetBrains.ProjectModel;
using JetBrains.UI;
using JetBrains.Util;
using MbUnit.Framework;

#if RESHARPER_31
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Shell.Test;
#else
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.Application.Test;
#endif

namespace Gallio.ReSharperRunner.Tests
{
    internal class GallioTestShell : TestShell
    {
        public GallioTestShell()
#if RESHARPER_31
            : base(GallioTestShellHandler.TestAssembly, GallioTestShellHandler.ConfigName)
#else
            : base(GallioTestShellHandler.TestAssembly, GallioTestShellHandler.TestAssembly, GallioTestShellHandler.ConfigName)
#endif
        {
        }

#if ! RESHARPER_31 && ! RESHARPER_40
        public override void InitializeComponents()
        {
            // In ReSharper v4.5, the test assembly resolver expects to find
            // "Platform\test\data" and "Platform\test\assemblies" folders within bin.
#if ! RESHARPER_41
            string binDir = new Uri(typeof(TestAssemblyReferenceResolver).Assembly.CodeBase).LocalPath;
            Directory.CreateDirectory(Path.Combine(binDir, @"..\Platform\test\data"));
            Directory.CreateDirectory(Path.Combine(binDir, @"..\Platform\test\assemblies"));
#endif

#if RESHARPER_41
            // As of ReSharper v4.1, the way the images are loaded has changed somewhat.
            // Instead of always loading from JetBrains.ReSharper.Resources, the images are resolved
            // from the "ConfigurationAssembly", which is the one that contains "AllAssemblies.xml".
            // Of course, that's our test assembly which obviously does not have the resources we need.
            //
            // So we explicitly make JetBrains.ReSharper.Resources eligible for image loading.
            //
            // Another approach might be to hack the ApplicationConfiguration object after the
            // AllAssembliesXml has been loaded. -- Jeff.
            ImageLoader.ImageId.AssembliesEligibleDefault[0] = typeof(JetBrains.ReSharper.GlobalSettings).Assembly;
#endif

            base.InitializeComponents();
        }
#endif
    }
}
#endif