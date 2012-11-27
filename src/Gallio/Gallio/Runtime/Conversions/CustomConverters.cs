﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Common;

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// Extensibility point for object conversion managed by <see cref="RuleBasedConverter"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the methods <see cref="Register"/> and <see cref="Unregister"/> to add and remove 
    /// custom type converters.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example registers a custom converter that converts an <see cref="Int32"/> value to a <c>Foo</c> object.
    /// <code><![CDATA[
    /// public class Foo
    /// {
    ///     public Foo(int value)
    ///     {
    ///         // Construction logic here...
    ///     }
    /// }
    /// 
    /// var customConverters = new CustomConverters();
    /// customConverters.Register<int, Foo>(x => new Foo(x));
    /// ]]></code>
    /// </example>
    public sealed class CustomConverters
    {
        private readonly IDictionary<ConversionKey, Data> converters = new Dictionary<ConversionKey, Data>();
        private readonly object syncRoot = new object();

        /// <summary>
        /// Registers a custom converter that transforms an object of the source type into an object of the target type.
        /// </summary>
        /// <param name="sourceType">The type of the object to convert.</param>
        /// <param name="targetType">The type of the result of the conversion.</param>
        /// <param name="converter">A delegate that performs the conversion.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceType"/>, <paramref name="targetType"/> or <paramref name="converter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom converter for the specified types was already registered.</exception>
        public void Register(Type sourceType, Type targetType, Conversion converter)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (targetType == null)
                throw new ArgumentNullException("targetType");
            if (converter == null)
                throw new ArgumentNullException("converter");

            lock (syncRoot)
            {
                var key = new ConversionKey(sourceType, targetType);
                Data data;

                if (converters.TryGetValue(key, out data))
                {
                    data.Count++;
                }
                else
                {
                    converters[key] = new Data(converter);
                }
            }
        }

        /// <summary>
        /// Registers a strongly-tped custom converter for the specified types.
        /// </summary>
        /// <typeparam name="TSource">The type of the object to convert.</typeparam>
        /// <typeparam name="TTarget">The type of the result of the conversion.</typeparam>
        /// <param name="converter">A delegate that performs the conversion.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="converter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom converter for the specified types was already registered.</exception>
        public void Register<TSource, TTarget>(Conversion<TSource, TTarget> converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");

            Register(typeof(TSource), typeof(TTarget), x => converter((TSource)x));
        }

        /// <summary>
        /// Unregisters the custom converter for the specified types.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom converter exists for the specified types,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <param name="sourceType">The searched source type.</param>
        /// <param name="targetType">The searched target type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceType"/> or <paramref name="targetType"/> is null.</exception>
        public void Unregister(Type sourceType, Type targetType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (targetType == null)
                throw new ArgumentNullException("targetType");

            lock (syncRoot)
            {
                var key = new ConversionKey(sourceType, targetType);
                Data data;

                if (converters.TryGetValue(key, out data))
                {
                    if (data.Count > 0)
                    {
                        data.Count--;
                    }
                    else
                    {
                        converters.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the custom converter for the specified types.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom converter exists for the specified types,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">The searched source type.</typeparam>
        /// <typeparam name="TTarget">The searched target type.</typeparam>
        public void Unregister<TSource, TTarget>()
        {
            Unregister(typeof(TSource), typeof(TTarget));
        }

        // Returns the converter for the searched types, or null if none was registered.
        internal Conversion Find(ConversionKey key)
        {
            lock (syncRoot)
            {
                Data data;
                return converters.TryGetValue(key, out data) ? data.Converter : null;
            }
        }

        // Removes all the registered custom converters.
        internal void UnregisterAll()
        {
            lock (syncRoot)
            {
                converters.Clear();
            }
        }

        private sealed class Data
        {
            private readonly Conversion converter;

            public Conversion Converter
            {
                get
                {
                    return converter;
                }
            }

            public int Count
            {
                get;
                set;
            }

            public Data(Conversion converter)
            {
                this.converter = converter;
            }
        }
    }
}
