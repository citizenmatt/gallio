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
using System.Diagnostics;
using System.IO;
using Gallio.Common.IO;
using Gallio.Properties;

namespace Gallio.Common.Caching
{
    // TODO: Support cache item expiration.

    /// <summary>
    /// A simple disk cache that stores its contents in a particular directory using hashes
    /// of the key values to ensure uniqueness.
    /// </summary>
    public class SimpleDiskCache : IDiskCache, IDiskCacheGroupCollection
    {
        private readonly string cacheDirectoryPath;

        /// <summary>
        /// Creates a simple disk cache.
        /// </summary>
        /// <param name="cacheDirectoryPath">The path of the cache directory.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="cacheDirectoryPath"/> is null.</exception>
        public SimpleDiskCache(string cacheDirectoryPath)
        {
            if (cacheDirectoryPath == null)
                throw new ArgumentNullException("cacheDirectoryPath");
            this.cacheDirectoryPath = Path.GetFullPath(cacheDirectoryPath);
        }

        /// <summary>
        /// Gets the path of the cache directory.
        /// </summary>
        public string CacheDirectoryPath
        {
            get { return cacheDirectoryPath; }
        }

        /// <inheritdoc />
        public IDiskCacheGroupCollection Groups
        {
            get { return this; }
        }

        /// <inheritdoc />
        public void Purge()
        {
            try
            {
                FileUtils.DeleteAll(cacheDirectoryPath);
            }
            catch (Exception ex)
            {
                throw new DiskCacheException(String.Format(Resources.SimpleDiskCache_PurgeException, cacheDirectoryPath), ex);
            }
        }

        /// <summary>
        /// Gets the group with the given key.
        /// </summary>
        /// <param name="key">The key, nor null.</param>
        /// <returns>The cache group.</returns>
        protected virtual IDiskCacheGroup GetGroup(string key)
        {
            var hash = new Hash64();
            hash = hash.Add(key);
            string path = Path.Combine(cacheDirectoryPath, hash.ToString());
            return new Group(this, key, new DirectoryInfo(path));
        }

        IDiskCacheGroup IDiskCacheGroupCollection.this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                return GetGroup(key);
            }
        }

        /// <summary>
        /// Represents a disk cache group.
        /// </summary>
        protected class Group : IDiskCacheGroup
        {
            private readonly IDiskCache cache;
            private readonly string key;
            private readonly DirectoryInfo location;

            /// <summary>
            /// Creates a group.
            /// </summary>
            /// <param name="cache">The cache.</param>
            /// <param name="key">The cache group key.</param>
            /// <param name="location">The location of the cache group.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="cache"/>,
            /// <paramref name="key"/> or <paramref name="location" /> is null.</exception>
            public Group(IDiskCache cache, string key, DirectoryInfo location)
            {
                if (cache == null)
                    throw new ArgumentNullException("cache");
                if (key == null)
                    throw new ArgumentNullException("key");
                if (location == null)
                    throw new ArgumentNullException("location");

                this.cache = cache;
                this.key = key;
                this.location = location;
            }

            /// <inheritdoc />
            public IDiskCache Cache
            {
                get { return cache; }
            }

            /// <inheritdoc />
            public string Key
            {
                get { return key; }
            }

            /// <inheritdoc />
            public DirectoryInfo Location
            {
                get { return location; }
            }

            /// <inheritdoc />
            public bool Exists
            {
                get
                {
                    try
                    {
                        return location.Exists;
                    }
                    catch (Exception ex)
                    {
                        throw new DiskCacheException(String.Format(Resources.SimpleDiskCache_DiskCacheGroupIndeterminateExistance, location.FullName), ex);
                    }
                }
            }

            /// <inheritdoc />
            public void Create()
            {
                CreateIfAbsent(location);
            }

            /// <inheritdoc />
            public void Delete()
            {
                try
                {
                    if (location.Exists)
                        location.Delete(true);
                }
                catch (DirectoryNotFoundException)
                {
                }
                catch (Exception ex)
                {
                    throw new DiskCacheException(String.Format(Resources.SimpleDiskCache_DiskCacheGroupDeleteException, location.FullName), ex);
                }
            }

            /// <inheritdoc />
            public FileInfo GetFileInfo(string relativeFilePath)
            {
                if (relativeFilePath == null)
                    throw new ArgumentNullException("relativeFilePath");

            	var fileName = Path.Combine(location.FullName, relativeFilePath);

				Debug.Assert(fileName.Length < 260, "Max length for a file path is 260");

				return new FileInfo(fileName);
            }

            /// <inheritdoc />
            public DirectoryInfo GetSubdirectoryInfo(string relativeDirectoryPath)
            {
                if (relativeDirectoryPath == null)
                    throw new ArgumentNullException("relativeDirectoryPath");
                return new DirectoryInfo(Path.Combine(location.FullName, relativeDirectoryPath));
            }

            /// <inheritdoc />
            public Stream OpenFile(string relativeFilePath, FileMode mode, FileAccess access, FileShare share)
            {
                FileInfo fileInfo = GetFileInfo(relativeFilePath);
                if (mode != FileMode.Open)
                    CreateIfAbsent(fileInfo.Directory);

                try
                {
                    return fileInfo.Open(mode, access, share);
                }
                catch (Exception ex)
                {
                    throw new DiskCacheException(String.Format(Resources.SimpleDiskCache_DiskCacheFileOpenException, fileInfo.FullName), ex);
                }
            }

            /// <inheritdoc />
            public DirectoryInfo CreateSubdirectory(string relativeDirectoryPath)
            {
                DirectoryInfo directoryInfo = GetSubdirectoryInfo(relativeDirectoryPath);
                CreateIfAbsent(directoryInfo);
                return directoryInfo;
            }

            private static void CreateIfAbsent(DirectoryInfo directoryInfo)
            {
                try
                {
                    if (!directoryInfo.Exists)
                        directoryInfo.Create();
                }
                catch (Exception ex)
                {
                    throw new DiskCacheException(String.Format(Resources.SimpleDiskCache_DiskCacheDirectoryOpenException, directoryInfo.FullName), ex);
                }
            }
        }
    }
}
