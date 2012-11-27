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

namespace Gallio.Ambience
{
    /// <summary>
    /// Represents a container of Ambient data and providers operations to
    /// query, store and update its contents.
    /// </summary>
    public interface IAmbientDataContainer
    {
        /// <summary>
        /// Gets all objects of a particular type in the container.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <returns>The data set.</returns>
        /// <exception cref="AmbienceException">Thrown if the operation failed.</exception>
        IAmbientDataSet<T> Query<T>();

        /// <summary>
        /// Gets all objects of a particular type in the container that match a particular filtering criteria.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="predicate">The filtering criteria.</param>
        /// <returns>The data set.</returns>
        /// <exception cref="AmbienceException">Thrown if the operation failed.</exception>
        IAmbientDataSet<T> Query<T>(Predicate<T> predicate);

        /// <summary>
        /// Deletes the object from the container.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        /// <exception cref="AmbienceException">Thrown if the operation failed.</exception>
        void Delete(object obj);

        /// <summary>
        /// Stores or updates an object in the container.
        /// </summary>
        /// <param name="obj">The object to store.</param>
        /// <exception cref="AmbienceException">Thrown if the operation failed.</exception>
        void Store(object obj);

        /// <summary>
        /// Deletes all objects in the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use with caution!
        /// </para>
        /// </remarks>
        /// <exception cref="AmbienceException">Thrown if the operation failed.</exception>
        void DeleteAll();
    }
}
