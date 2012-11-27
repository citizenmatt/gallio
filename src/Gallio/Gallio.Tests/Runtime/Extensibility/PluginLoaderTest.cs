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
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Common.Reflection;
using Gallio.Common.Validation;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Extensibility.Schema;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Extensibility
{
    [TestsOn(typeof(PluginLoader))]
    public class PluginLoaderTest
    {
        [Test]
        public void PopulateCatalog_WhenCatalogIsNull_Throws()
        {
            var loader = new PluginLoader();

            Assert.Throws<ArgumentNullException>(() => loader.PopulateCatalog(null, NullProgressMonitor.CreateInstance()));
        }

        [Test]
        public void AddPluginPath_WhenPluginPathIsNull_Throws()
        {
            var loader = new PluginLoader();

            Assert.Throws<ArgumentNullException>(() => loader.AddPluginPath(null));
        }

        [Test]
        public void AddPluginXml_WhenPluginXmlIsNull_Throws()
        {
            var loader = new PluginLoader();

            Assert.Throws<ArgumentNullException>(() => loader.AddPluginXml(null, new DirectoryInfo(@"C:\")));
        }

        [Test]
        public void AddPluginXml_WhenBaseDirectoryIsNull_Throws()
        {
            var loader = new PluginLoader();

            Assert.Throws<ArgumentNullException>(() => loader.AddPluginXml("", null));
        }

        [Test]
        public void DefinePreprocessorConstant_WhenConstantIsNull_Throws()
        {
            var loader = new PluginLoader();

            Assert.Throws<ArgumentNullException>(() => loader.DefinePreprocessorConstant(null));
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathContainsInvalidDirectory_IgnoresIt()
        {
            var loader = new PluginLoader();
            var catalog = MockRepository.GenerateMock<IPluginCatalog>();
            loader.AddPluginPath(@"C:\This\Directory\Does\Not\Exist");

            loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

            catalog.VerifyAllExpectations(); // nothing added to catalog
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathRefersToDirectoryWithAtLeastOnePluginFile_LoadsIt()
        {
            string pluginContents = GenerateValidPluginXml();

            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new PluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                loader.AddPluginPath(pluginDir);

                Plugin plugin = null;
                DirectoryInfo baseDirectory = null;
                catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                    .Do((Gallio.Common.Action<Plugin, DirectoryInfo>) delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                {
                    plugin = pluginArg;
                    baseDirectory = baseDirectoryArg;
                });

                loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

                catalog.VerifyAllExpectations(); // added one plugin

                Assert.Multiple(() =>
                {
                    Assert.AreEqual("pluginId", plugin.PluginId);
                    Assert.AreEqual("serviceId", plugin.Services[0].ServiceId);
                    Assert.AreEqual("componentId", plugin.Components[0].ComponentId);
                    Assert.AreEqual("Assembly1", plugin.Assemblies[0].FullName);
                    Assert.AreEqual("CodeBase.dll", plugin.Assemblies[0].CodeBase);
                    Assert.AreEqual("Assembly2", plugin.Assemblies[1].FullName);

                    Assert.AreEqual(pluginDir, baseDirectory.ToString());
                });
            }, pluginContents);
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathRefersToPluginFile_LoadsIt()
        {
            string pluginContents = GenerateValidPluginXml();

            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new PluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                loader.AddPluginPath(pluginFile);

                Plugin plugin = null;
                DirectoryInfo baseDirectory = null;
                catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                    .Do((Gallio.Common.Action<Plugin, DirectoryInfo>)delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                    {
                        plugin = pluginArg;
                        baseDirectory = baseDirectoryArg;
                    });

                loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

                catalog.VerifyAllExpectations(); // added one plugin

                Assert.Multiple(() =>
                {
                    Assert.AreEqual("pluginId", plugin.PluginId);
                    Assert.AreEqual("serviceId", plugin.Services[0].ServiceId);
                    Assert.AreEqual("componentId", plugin.Components[0].ComponentId);
                    Assert.AreEqual("Assembly1", plugin.Assemblies[0].FullName);
                    Assert.AreEqual("CodeBase.dll", plugin.Assemblies[0].CodeBase);
                    Assert.AreEqual("Assembly2", plugin.Assemblies[1].FullName);

                    Assert.AreEqual(pluginDir, baseDirectory.ToString());
                });
            }, pluginContents);
        }

        [Test]
        public void PopulateCatalog_WhenPluginXmlIsValid_LoadsIt()
        {
            string pluginContents = GenerateValidPluginXml();

            var loader = new PluginLoader();
            var catalog = MockRepository.GenerateMock<IPluginCatalog>();
            loader.AddPluginXml(pluginContents, new DirectoryInfo(@"C:\"));

            Plugin plugin = null;
            DirectoryInfo baseDirectory = null;
            catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                .Do((Gallio.Common.Action<Plugin, DirectoryInfo>)delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                {
                    plugin = pluginArg;
                    baseDirectory = baseDirectoryArg;
                });

            loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

            catalog.VerifyAllExpectations(); // added one plugin

            Assert.Multiple(() =>
            {
                Assert.AreEqual("pluginId", plugin.PluginId);
                Assert.AreEqual("serviceId", plugin.Services[0].ServiceId);
                Assert.AreEqual("componentId", plugin.Components[0].ComponentId);
                Assert.AreEqual("Assembly1", plugin.Assemblies[0].FullName);
                Assert.AreEqual("CodeBase.dll", plugin.Assemblies[0].CodeBase);
                Assert.AreEqual("Assembly2", plugin.Assemblies[1].FullName);

                Assert.AreEqual(@"C:\", baseDirectory.ToString());
            });
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathContainsUnparsablePluginFile_Throws()
        {
            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new PluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                loader.AddPluginPath(pluginFile);

                var ex = Assert.Throws<RuntimeException>(() => loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance()));
                Assert.AreEqual(string.Format("Failed to read and parse plugin metadata file '{0}'.", pluginFile), ex.Message);
                Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);

                catalog.VerifyAllExpectations(); // no plugins added
            }, "<badxml />");
        }

        [Test]
        public void PopulateCatalog_WhenPluginXmlCannotBeParsed_Throws()
        {
            var loader = new PluginLoader();
            var catalog = MockRepository.GenerateMock<IPluginCatalog>();
            loader.AddPluginXml("<badxml />", new DirectoryInfo(@"C:\"));

            var ex = Assert.Throws<RuntimeException>(() => loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance()));
            Assert.AreEqual("Failed to read and parse plugin metadata from Xml configuration.", ex.Message);
            Assert.IsInstanceOfType<InvalidOperationException>(ex.InnerException);

            catalog.VerifyAllExpectations(); // no plugins added
        }

        [Test]
        public void PopulateCatalog_WhenPluginPathContainsInvalidPluginFile_Throws()
        {
            string pluginContents = GenerateInvalidPluginXml();

            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new PluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                loader.AddPluginPath(pluginFile);

                var ex = Assert.Throws<RuntimeException>(() => loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance()));
                Assert.AreEqual(string.Format("Failed to read and parse plugin metadata file '{0}'.", pluginFile), ex.Message);
                Assert.IsInstanceOfType<ValidationException>(ex.InnerException);

                catalog.VerifyAllExpectations(); // no plugins added
            }, pluginContents);
        }

        [Test]
        public void PopulateCatalog_WhenPluginXmlIsInvalid_Throws()
        {
            string pluginContents = GenerateInvalidPluginXml();

            var loader = new PluginLoader();
            var catalog = MockRepository.GenerateMock<IPluginCatalog>();
            loader.AddPluginXml(pluginContents, new DirectoryInfo(@"C:\"));

            var ex = Assert.Throws<RuntimeException>(() => loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance()));
            Assert.AreEqual("Failed to read and parse plugin metadata from Xml configuration.", ex.Message);
            Assert.IsInstanceOfType<ValidationException>(ex.InnerException);

            catalog.VerifyAllExpectations(); // no plugins added
        }

        [Test]
        public void PopulateCatalog_WhenPluginFileContainsPreprocessorInstructions_AppliesThem()
        {
            string pluginContents = "<plugin pluginId=\"pluginId\" xmlns=\"http://www.gallio.org/\"><traits><?ifdef A?><name>A</name><?endif?><?ifdef B?><property>B</property><?endif?></traits></plugin>";

            RunWithTemporaryPluginFile((pluginDir, pluginFile) =>
            {
                var loader = new PluginLoader();
                var catalog = MockRepository.GenerateMock<IPluginCatalog>();
                loader.AddPluginPath(pluginFile);
                loader.DefinePreprocessorConstant("A");

                Plugin plugin = null;
                catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                    .Do((Gallio.Common.Action<Plugin, DirectoryInfo>)delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                    {
                        plugin = pluginArg;
                    });

                loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

                catalog.VerifyAllExpectations(); // added one plugin

                Assert.AreEqual(new PropertySet() { { "name", "A" } }, plugin.Traits.PropertySet);
            }, pluginContents);
        }

        [Test]
        public void PopulateCatalog_WhenPluginXmlContainsPreprocessorInstructions_AppliesThem()
        {
            string pluginContents = "<plugin pluginId=\"pluginId\" xmlns=\"http://www.gallio.org/\"><traits><?ifdef A?><name>A</name><?endif?><?ifdef B?><property>B</property><?endif?></traits></plugin>";

            var loader = new PluginLoader();
            var catalog = MockRepository.GenerateMock<IPluginCatalog>();
            loader.AddPluginXml(pluginContents, new DirectoryInfo(@"C:\"));
            loader.DefinePreprocessorConstant("A");

            Plugin plugin = null;
            catalog.Expect(x => x.AddPlugin(null, null)).IgnoreArguments()
                .Do((Gallio.Common.Action<Plugin, DirectoryInfo>)delegate(Plugin pluginArg, DirectoryInfo baseDirectoryArg)
                {
                    plugin = pluginArg;
                });

            loader.PopulateCatalog(catalog, NullProgressMonitor.CreateInstance());

            catalog.VerifyAllExpectations(); // added one plugin

            Assert.AreEqual(new PropertySet() { { "name", "A" } }, plugin.Traits.PropertySet);
        }

        [Test]
        public void InstallationId_CanGetSet()
        {
            var loader = new PluginLoader();

            Assert.AreEqual(Guid.Empty, loader.InstallationId);

            Guid guid = Guid.NewGuid();
            loader.InstallationId = guid;

            Assert.AreEqual(guid, loader.InstallationId);
        }

        internal static void RunWithTemporaryPluginFile(Gallio.Common.Action<string, string> action, string pluginFileContents)
        {
            var pluginDir = SpecialPathPolicy.For<PluginLoaderTest>().GetTempDirectory().FullName;
            if (Directory.Exists(pluginDir))
                Directory.Delete(pluginDir, true);
            var pluginFile = Path.Combine(pluginDir, "Sample.plugin");
            try
            {
                Directory.CreateDirectory(pluginDir);
                System.IO.File.WriteAllText(pluginFile, pluginFileContents);

                action(pluginDir, pluginFile);
            }
            finally
            {
                if (Directory.Exists(pluginDir))
                    Directory.Delete(pluginDir, true);
            }
        }

        internal static string GenerateValidPluginXml()
        {
            Plugin samplePlugin = new Plugin("pluginId")
            {
                Assemblies =
                {
                    new Assembly("Assembly1") { CodeBase = "CodeBase.dll" },
                    new Assembly("Assembly2")
                },
                Services =
                {
                    new Service("serviceId", "Service, Assembly")
                },
                Components =
                {
                    new Component("componentId", "serviceId", "Component, Assembly")
                }
            };

            return Assert.XmlSerialize(samplePlugin);
        }

        private static string GenerateInvalidPluginXml()
        {
            Plugin samplePlugin = new Plugin("pluginId");

            return Assert.XmlSerialize(samplePlugin).Replace("pluginId=\"pluginId\"", "");
        }
    }
}
