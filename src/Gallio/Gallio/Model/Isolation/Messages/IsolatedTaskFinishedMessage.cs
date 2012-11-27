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
using Gallio.Common.Diagnostics;
using Gallio.Common.Messaging;
using Gallio.Common.Validation;

namespace Gallio.Model.Isolation.Messages
{
    /// <summary>
    /// Tells the server that the client has finished processing an isolated task.
    /// </summary>
    [Serializable]
    public class IsolatedTaskFinishedMessage : Message
    {
        /// <summary>
        /// Gets or sets the unique id of the isolated task.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the isolated task result.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the exception data.
        /// </summary>
        public ExceptionData Exception { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            if (Id == Guid.Empty)
                throw new ValidationException("Id should be set.");
        }
    }
}
