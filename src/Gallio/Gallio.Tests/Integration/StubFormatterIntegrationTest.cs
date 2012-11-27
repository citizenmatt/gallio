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
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Runtime.Formatting;
using Gallio.Common.Reflection;
using Gallio.Runtime;
using MbUnit.Framework;

namespace Gallio.Tests.Integration
{
    /// <summary>
    /// This integration test verifies that <see cref="StubFormatter" /> is
    /// used when the runtime is not initialized.  The trick to doing this is
    /// in running the test code in an isolated AppDomain where the runtime does
    /// not exist.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(Formatter))]
    [TestsOn(typeof(StubFormatter))]
    public class StubFormatterIntegrationTest
    {
        [Test]
        public void StubFormatterIsUsedWhenRuntimeIsNotInitialized()
        {
            Type remoteCodeType = typeof(RemoteCode);

            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(remoteCodeType.Assembly));

            AppDomain appDomain = AppDomain.CreateDomain("Test", null, appDomainSetup);
            try
            {
                RemoteCode remoteCode = (RemoteCode)appDomain.CreateInstanceAndUnwrap(remoteCodeType.Assembly.FullName, remoteCodeType.FullName);

                string output = remoteCode.Run();

                Assert.AreEqual("\"abc\"", output);
            }
            finally
            {
                AppDomain.Unload(appDomain);
            }
        }

        public class RemoteCode : MarshalByRefObject
        {
            public string Run()
            {
                StringWriter textWriter = new StringWriter();
                textWriter.NewLine = "\n";
                Console.SetOut(textWriter);

                Assert.IsFalse(RuntimeAccessor.IsInitialized);
                Assert.IsInstanceOfType(typeof(StubFormatter), Formatter.Instance);

                textWriter.Write(Formatter.Instance.Format("abc"));

                return textWriter.ToString();
            }
        }
    }
}
