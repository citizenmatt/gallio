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
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Common.Validation;
using Gallio.Runtime.Debugging;

namespace Gallio.Model.Schema
{
    /// <summary>
    /// Describes a test package in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="TestPackage"/>
    [Serializable]
    [XmlRoot("testPackage", Namespace=SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public sealed class TestPackageData : IValidatable
    {
        private readonly List<string> hintDirectories;
        private readonly List<string> files;
        private readonly List<string> excludedFrameworkIds;
        private PropertySet properties;

        /// <summary>
        /// Creates an empty test package.
        /// </summary>
        public TestPackageData()
        {
            hintDirectories = new List<string>();
            files = new List<string>();
            excludedFrameworkIds = new List<string>();
        }

        /// <summary>
        /// Copies the contents of a test package.
        /// </summary>
        /// <param name="source">The source test package.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public TestPackageData(TestPackage source)
            : this()
        {
            if (source == null)
                throw new ArgumentNullException("source");

            GenericCollectionUtils.ConvertAndAddAll(source.HintDirectories, hintDirectories, directory => directory.FullName);
            GenericCollectionUtils.ConvertAndAddAll(source.Files, files, file => file.FullName);
            excludedFrameworkIds.AddRange(source.ExcludedTestFrameworkIds);
            ShadowCopy = source.ShadowCopy;
            Debug = source.DebuggerSetup != null;
            ApplicationBaseDirectory = source.ApplicationBaseDirectory != null ? source.ApplicationBaseDirectory.FullName : null;
            WorkingDirectory = source.WorkingDirectory != null ? source.WorkingDirectory.FullName : null;
            RuntimeVersion = source.RuntimeVersion;
            properties = source.Properties.Copy();
        }

        /// <summary>
        /// Gets the list of relative or absolute paths of test files.
        /// </summary>
        /// <seealso cref="TestPackage.Files"/>
        [XmlArray("files", IsNullable = false)]
        [XmlArrayItem("file", typeof(string), IsNullable = false)]
        public List<string> Files
        {
            get { return files; }
        }

        /// <summary>
        /// Gets the list of hint directories used to resolve test assemblies and other files.
        /// </summary>
        /// <seealso cref="TestPackage.HintDirectories"/>
        [XmlArray("hintDirectories", IsNullable = false)]
        [XmlArrayItem("hintDirectory", typeof(string), IsNullable=false)]
        public List<string> HintDirectories
        {
            get { return hintDirectories; }
        }

        /// <summary>
        /// Gets the list of test framework IDs that are to be excluded from the test
        /// exploration process.
        /// </summary>
        /// <seealso cref="TestPackage.ExcludedTestFrameworkIds"/>
        [XmlArray("excludedFrameworkIds", IsNullable = false)]
        [XmlArrayItem("excludedFrameworkId", typeof(string), IsNullable = false)]
        public List<string> ExcludedFrameworkIds
        {
            get { return excludedFrameworkIds; }
        }

        /// <summary>
        /// Gets or sets whether assembly shadow copying is enabled.
        /// </summary>
        /// <seealso cref="TestPackage.ShadowCopy"/>
        [XmlAttribute("shadowCopy")]
        public bool ShadowCopy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to attach the debugger to the host.
        /// </summary>
        /// <seealso cref="TestPackage.DebuggerSetup"/>
        [XmlAttribute("debug")]
        public bool Debug
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application base directory, or null to use the directory
        /// containing each group of test files or some other suitable default chosen by the test framework.
        /// </summary>
        /// <seealso cref="TestPackage.ApplicationBaseDirectory"/>
        [XmlAttribute("applicationBaseDirectory")]
        public string ApplicationBaseDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the working directory, or null to use the directory containing
        /// each group of test files or some other suitable default chosen by the test framework.
        /// </summary>
        /// <seealso cref="TestPackage.WorkingDirectory"/>
        [XmlAttribute("workingDirectory")]
        public string WorkingDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the .Net runtime version, or null to auto-detect.
        /// </summary>
        /// <seealso cref="TestPackage.RuntimeVersion"/>
        [XmlAttribute("runtimeVersion")]
        public string RuntimeVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the properties for the test runner. (non-null)
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        /// <seealso cref="TestPackage.Properties"/>
        [XmlElement("properties", IsNullable = false)]
        public PropertySet Properties
        {
            get
            {
                if (properties == null)
                    properties = new PropertySet();
                return properties;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                properties = value;
            }
        }

        /// <summary>
        /// Initializes a test package with the contents of this structure.
        /// </summary>
        /// <param name="testPackage">The test package to populate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackage"/> is null.</exception>
        public void InitializeTestPackage(TestPackage testPackage)
        {
            if (testPackage == null)
                throw new ArgumentNullException("testPackage");

            GenericCollectionUtils.ForEach(files, x => testPackage.AddFile(new FileInfo(x)));
            GenericCollectionUtils.ForEach(hintDirectories, x => testPackage.AddHintDirectory(new DirectoryInfo(x)));
            GenericCollectionUtils.ForEach(excludedFrameworkIds, x => testPackage.AddExcludedTestFrameworkId(x));
            testPackage.ShadowCopy = ShadowCopy;
            testPackage.DebuggerSetup = Debug ? new DebuggerSetup() : null;
            testPackage.ApplicationBaseDirectory = ApplicationBaseDirectory != null ? new DirectoryInfo(ApplicationBaseDirectory) : null;
            testPackage.WorkingDirectory = WorkingDirectory != null ? new DirectoryInfo(WorkingDirectory) : null;
            testPackage.RuntimeVersion = RuntimeVersion;
            GenericCollectionUtils.ForEach(Properties, x => testPackage.AddProperty(x.Key, x.Value));
        }

        /// <summary>
        /// Creates a test package with the contents of this structure.
        /// </summary>
        /// <returns>The test package.</returns>
        public TestPackage ToTestPackage()
        {
            var testPackage = new TestPackage();
            InitializeTestPackage(testPackage);
            return testPackage;
        }

        /// <summary>
        /// Makes the test package paths relative to the specified base path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="basePath"/> is null.</exception>
        public void MakeRelativePaths(string basePath)
        {
            ApplyPathConversion(basePath, FileUtils.MakeRelativePath);
        }

        /// <summary>
        /// Makes the test package paths absolute given the specified base path.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="basePath"/> is null.</exception>
        public void MakeAbsolutePaths(string basePath)
        {
            ApplyPathConversion(basePath, FileUtils.MakeAbsolutePath);
        }

        private void ApplyPathConversion(string basePath, Func<string, string, string> converter)
        {
            if (basePath == null)
                throw new ArgumentNullException("basePath");

            if (WorkingDirectory != null)
                WorkingDirectory = converter(WorkingDirectory, basePath);
            if (ApplicationBaseDirectory != null)
                ApplicationBaseDirectory = converter(ApplicationBaseDirectory, basePath);
            if (ApplicationBaseDirectory != null)
                ApplicationBaseDirectory = converter(ApplicationBaseDirectory, basePath);
            GenericCollectionUtils.ConvertInPlace(files, x => converter(x, basePath));
            GenericCollectionUtils.ConvertInPlace(hintDirectories, x => converter(x, basePath));
        }

        /// <inheritdoc />
        public void Validate()
        {
            ValidationUtils.ValidateElementsAreNotNull("hintDirectories", hintDirectories);
            ValidationUtils.ValidateElementsAreNotNull("files", files);
            ValidationUtils.ValidateElementsAreNotNull("excludedFrameworkIds", excludedFrameworkIds);
        }
    }
}