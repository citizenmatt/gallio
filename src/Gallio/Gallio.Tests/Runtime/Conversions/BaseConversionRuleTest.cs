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
using System.Text;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Conversions
{
    /// <summary>
    /// Abstract base class for <see cref="IConversionRule" /> tests.
    /// Automatically sets up a <see cref="RuleBasedConverter" /> populated with
    /// the rule and a <see cref="ConvertibleToConvertibleConversionRule"/>
    /// </summary>
    public abstract class BaseConversionRuleTest<T>
        where T : IConversionRule, new()
    {
        private IConverter converter;
        private IExtensionPoints extensionPoints;

        protected IConverter Converter
        {
            get
            {
                return converter;
            }
        }

        protected IExtensionPoints ExtensionPoints
        {
            get
            {
                return extensionPoints;
            }
        }

        [SetUp]
        public void SetUpConverter()
        {
            extensionPoints = new DefaultExtensionPoints();
            converter = new RuleBasedConverter(extensionPoints, new IConversionRule[]
            {
                new T(),
                new ConvertibleToConvertibleConversionRule()
            });
        }
    }
}
