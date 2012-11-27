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
using System.Xml;
using Gallio.ReSharperRunner.Provider.Facade;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// This task specifies a single test assembly that contains tests to be executed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It should always appear after <see cref="GallioTestRunTask" />.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class GallioTestAssemblyTask : FacadeTask, IEquatable<GallioTestAssemblyTask>
    {
        private readonly string assemblyLocation;

        public GallioTestAssemblyTask(string assemblyLocation, string typeName, string shortName) 
            : base(typeName, shortName)
        {
            this.assemblyLocation = assemblyLocation;
        }

        public GallioTestAssemblyTask(XmlElement element)
            : base(element)
        {
            assemblyLocation = element.GetAttribute("assemblyLocation");
        }

        public string AssemblyLocation
        {
            get { return assemblyLocation; }
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);

            element.SetAttribute("assemblyLocation", assemblyLocation);
        }

        public bool Equals(GallioTestAssemblyTask other)
        {
            return other != null && assemblyLocation == other.assemblyLocation;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioTestAssemblyTask);
        }

        public override int GetHashCode()
        {
            return 0x11111111 ^ assemblyLocation.GetHashCode();
        }
    }
}