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
using MbUnit.Framework;

#pragma warning disable 0618

namespace MbUnit.Compatibility.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(OldFileAssert))]
    public class OldFileAssertTest
    {
        [Test]
        public void AreEqualPath()
        {
            string pathFile = Assembly.GetExecutingAssembly().Location;
            OldFileAssert.AreEqual(pathFile, pathFile);
        }

        [Test]
        public void AreEqualFileInfo()
        {
            string pathFile = Assembly.GetExecutingAssembly().Location;
            FileInfo file = new FileInfo(pathFile);
            FileInfo file2 = new FileInfo(pathFile);
            OldFileAssert.AreEqual(file, file2);
        }

        [Test]
        public void AreStreamContentEqual()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string path = Path.GetDirectoryName(assemblyPath);
            string pathFile = Path.Combine(path, "MbUnitFileAssert_Test.tmp");


            try
            {
                StreamWriter strWriter = new StreamWriter(pathFile);

                strWriter.WriteLine("Testing MbUnit");
                strWriter.Close();

                Stream str = new FileStream(pathFile, FileMode.Open);

                File.Copy(pathFile, pathFile + "TestCopy", true);
                Stream str2 = new FileStream(pathFile + "TestCopy", FileMode.Open);

                OldFileAssert.AreStreamContentEqual(str, str2);
            }
            finally
            {
                File.Delete(pathFile + "TestCopy");
            }

        }

        [Test]
        public void Exists()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            OldFileAssert.Exists(path);
        }

        [Test]
        public void NotExists()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            OldFileAssert.NotExists(path + "MbUnitTest");
        }
    }
}
