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
using Gallio.Framework;
using Gallio.Runtime.Logging;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestFixture]
    [TestsOn(typeof(BaseHostFactory))]
    public class BaseHostFactoryTest
    {
        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfHostSetupIsNull()
        {
            StubHostFactory factory = new StubHostFactory();
            factory.CreateHost(null, new MarkupStreamLogger(TestLog.Default));
        }

        [Test, ExpectedArgumentNullException]
        public void CreateHostThrowsIfLoggerIsNull()
        {
            StubHostFactory factory = new StubHostFactory();
            factory.CreateHost(new HostSetup(), null);
        }

        [Test]
        public void CreateHostDelegatesToCreateHostImplWithACanonicalizedHostSetup()
        {
            StubHostFactory factory = new StubHostFactory();

            HostSetup originalHostSetup = new HostSetup();
            originalHostSetup.WorkingDirectory = "";
            ILogger logger = new MarkupStreamLogger(TestLog.Default);
            Assert.IsNotNull(factory.CreateHost(originalHostSetup, logger));

            Assert.AreNotSame(originalHostSetup, factory.HostSetup);
            Assert.AreEqual(Environment.CurrentDirectory, factory.HostSetup.WorkingDirectory);
            Assert.AreSame(logger, factory.Logger);
        }

        private class StubHostFactory : BaseHostFactory
        {
            public HostSetup HostSetup;
            public ILogger Logger;

            protected override IHost CreateHostImpl(HostSetup hostSetup, ILogger logger)
            {
                HostSetup = hostSetup;
                Logger = logger;
                return new MockRepository().Stub<IHost>();
            }
        }
    }
}