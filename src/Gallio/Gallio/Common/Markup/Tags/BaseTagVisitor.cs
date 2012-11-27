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

namespace Gallio.Common.Markup.Tags
{
    /// <summary>
    /// An abstract base class for tag visitors that recursively traverses all tags
    /// and does nothing else by default.
    /// </summary>
    public abstract class BaseTagVisitor : ITagVisitor
    {
        /// <inheritdoc />
        public virtual void VisitBodyTag(BodyTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitSectionTag(SectionTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitMarkerTag(MarkerTag tag)
        {
            tag.AcceptContents(this);
        }

        /// <inheritdoc />
        public virtual void VisitEmbedTag(EmbedTag tag)
        {
        }

        /// <inheritdoc />
        public virtual void VisitTextTag(TextTag tag)
        {
        }
    }
}
