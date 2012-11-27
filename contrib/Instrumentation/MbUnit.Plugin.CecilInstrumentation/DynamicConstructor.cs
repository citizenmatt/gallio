// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection.Emit;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Describes a module in a dynamic assembly.
    /// </summary>
    internal class DynamicConstructor
    {
        private readonly ConstructorBuilder builder;
        private readonly Type[] parameterTypes;

        public DynamicConstructor(ConstructorBuilder builder, Type[] parameterTypes)
        {
            this.builder = builder;
            this.parameterTypes = parameterTypes;
        }

        public ConstructorBuilder Builder
        {
            get { return builder; }
        }

        public Type[] ParameterTypes
        {
            get { return parameterTypes; }
        }
    }
}
