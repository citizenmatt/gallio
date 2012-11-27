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

namespace Gallio.Common.Caching
{
    /// <summary>
    /// A disk cache manages temporary files and directories stored on disk and
    /// arranged into groups associated with arbitrary string keys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The files and directories within each group are assumed to have the same lifetime.
    /// </para>
    /// </remarks>
    /// <example>
    /// Write a file to the cache:
    /// <code><![CDATA[
    /// IDiskCache cache = ... // Get the cache.
    /// 
    /// using (var writer = new StreamWriter(cache.Groups["SomeKey"].OpenFile("Foo",
    ///     FileMode.OpenOrCreate, FileAccess.Write, FileShare.Exclusive)))
    /// {
    ///     writer.WriteLine("Contents...");
    /// }
    /// ]]></code>
    /// </example>
    public interface IDiskCache
    {
        /// <summary>
        /// Gets the collection of disk cache groups.
        /// </summary>
        IDiskCacheGroupCollection Groups { get; }

        /// <summary>
        /// Deletes all items in the cache.
        /// </summary>
        /// <exception cref="DiskCacheException">Thrown if an error occurs.</exception>
        void Purge();
    }
}
