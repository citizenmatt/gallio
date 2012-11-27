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
using System.Xml.Serialization;

namespace Gallio.Common.Xml
{
    ///<summary>
    /// Provides Xml serialization and deserialization services.
    ///</summary>
    public interface IXmlSerializer
    {
        /// <summary>
        /// Saves an object graph to a pretty-printed Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <param name="filename">The filename.</param>
        /// <typeparam name="T">The root object type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="root"/>
        /// or <paramref name="filename"/> is null.</exception>
        void SaveToXml<T>(T root, string filename);

        /// <summary>
        /// Loads an object graph from an Xml file using <see cref="XmlSerializer" />.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The root object.</returns>
        /// <typeparam name="T">The root object type.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filename"/> is null.</exception>
        T LoadFromXml<T>(string filename);
    }
}