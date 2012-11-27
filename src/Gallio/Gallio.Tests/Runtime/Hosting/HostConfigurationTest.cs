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
using System.Text;
using Gallio.Common.Policies;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestsOn(typeof(HostConfiguration))]
    public class HostConfigurationTest
    {
        [Test]
        public void WriteToReencodesAccordingToTextWriter()
        {
            StringWriter writer = new StringWriter();
            Assert.AreEqual(Encoding.Unicode.EncodingName, writer.Encoding.EncodingName);

            HostConfiguration config = new HostConfiguration();
            config.WriteTo(writer);

            Assert.Contains(writer.ToString(), writer.Encoding.WebName);
        }

        [Test]
        public void WriteToReencodesAccordingToTextWriter_WhenConfigurationXmlContainsDifferentEncoding()
        {
            StringWriter writer = new StringWriter();
            Assert.AreEqual(Encoding.Unicode.EncodingName, writer.Encoding.EncodingName);

            HostConfiguration config = new HostConfiguration();
            config.ConfigurationXml = "<?xml version=\"1.0\" encoding=\"utf-32\"?><configuration />";
            config.WriteTo(writer);

            Assert.Contains(writer.ToString(), writer.Encoding.WebName);
        }

        [Test]
        public void WriteToThrowsIfTextWriterIsNull()
        {
            HostConfiguration config = new HostConfiguration();
            Assert.Throws<ArgumentNullException>(() => config.WriteTo(null));
        }

        [Test]
        public void WriteToFileThrowsIfPathIsNull()
        {
            HostConfiguration config = new HostConfiguration();
            Assert.Throws<ArgumentNullException>(() => config.WriteToFile(null));
        }

        [Test]
        public void WriteToFile()
        {
            HostConfiguration config = new HostConfiguration();
            string path = SpecialPathPolicy.For<HostConfigurationTest>().CreateTempFileWithUniqueName().FullName;
            try
            {
                config.WriteToFile(path);

                Assert.Contains(File.ReadAllText(path), "<configuration>");
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
