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
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;
using Gallio.Common.Remoting;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Remoting
{
    [TestFixture]
    [TestsOn(typeof(BinaryTcpClientChannel))]
    [TestsOn(typeof(BinaryTcpServerChannel))]
    [DependsOn(typeof(BaseClientChannelTest))]
    [DependsOn(typeof(BaseServerChannelTest))]
    public class BinaryTcpChannelTest
    {
        private const int PortNumber = 33333;
        private const string ServiceName = "Test";

        [Test, ExpectedArgumentNullException]
        public void BinaryTcpClientChannelConstructorThrowsIfHostNameIsNull()
        {
            new BinaryTcpClientChannel(null, 1, TimeSpan.FromSeconds(30));
        }

        [Test, ExpectedArgumentNullException]
        public void BinaryTcpServerChannelConstructorThrowsIfHostNameIsNull()
        {
            new BinaryTcpServerChannel(null, 1);
        }

        [Test]
        public void RegisteredServiceCanBeAccessedWithGetService()
        {
            var hostFactory = (IsolatedAppDomainHostFactory)RuntimeAccessor.ServiceLocator.ResolveByComponentId(IsolatedAppDomainHostFactory.ComponentId);
            using (IHost host = hostFactory.CreateHost(new HostSetup(), new MarkupStreamLogger(TestLog.Default)))
            {
                HostAssemblyResolverHook.InstallCallback(host);

                host.GetHostService().Do<object, object>(RemoteCallback, null);

                using (BinaryTcpClientChannel clientChannel = new BinaryTcpClientChannel("localhost", PortNumber, TimeSpan.FromSeconds(30)))
                {
                    TestService serviceProxy =
                        (TestService)clientChannel.GetService(typeof(TestService), ServiceName);
                    Assert.AreEqual(42, serviceProxy.Add(23, 19));
                }
            }
        }

        public static object RemoteCallback(object dummy)
        {
            BinaryTcpServerChannel serverChannel = new BinaryTcpServerChannel("localhost", PortNumber);
            TestService serviceProvider = new TestService();
            serverChannel.RegisterService(ServiceName, serviceProvider);
            return null;
        }

        public class TestService : MarshalByRefObject
        {
            public int Add(int x, int y)
            {
                return x + y;
            }
        }
    }
}
