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

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// A slot represents a field, property or parameter.  It is used to
    /// simplify the handling of data binding since all three of these types
    /// are similar in that they can hold values of some type.
    /// </summary>
    public interface ISlotInfo : ICodeElementInfo, IEquatable<ISlotInfo>
    {
        /// <summary>
        /// Gets the type of value held in the slot.
        /// </summary>
        ITypeInfo ValueType { get; }

        /// <summary>
        /// Gets the positional index of a method parameter slot, -1 if the
        /// slot is a method return parameter, or 0 if the slot is of some other kind.
        /// </summary>
        int Position { get; }
    }
}
