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
using System.Collections.ObjectModel;
using Gallio.Common.Collections;
using Gallio.Common.Markup.Tags;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// A structured text object contains attachments and formatted text with rich
    /// presentation elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Structured text is emitted by a <see cref="StructuredTextWriter" />.
    /// </para>
    /// </remarks>
    /// <seealso cref="StructuredTextWriter"/>
    [Serializable]
    public sealed class StructuredText : IEquatable<StructuredText>, IMarkupStreamWritable
    {
        private readonly BodyTag bodyTag;
        private readonly IList<Attachment> attachments;

        /// <summary>
        /// Creates a simple structured text object over a plain text string.
        /// </summary>
        /// <param name="text">The text string.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public StructuredText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            bodyTag = new BodyTag();
            bodyTag.Contents.Add(new TextTag(text));

            attachments = EmptyArray<Attachment>.Instance;
        }

        /// <summary>
        /// Creates a structured text object that wraps the body tag of a structured markup document stream
        /// and no attachments.
        /// </summary>
        /// <param name="bodyTag">The body tag to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bodyTag"/> is null.</exception>
        public StructuredText(BodyTag bodyTag)
            : this(bodyTag, EmptyArray<Attachment>.Instance)
        {
        }

        /// <summary>
        /// Creates a structured text object that wraps the body tag of a structured markup document stream
        /// and a list of attachments.
        /// </summary>
        /// <param name="bodyTag">The body tag to wrap.</param>
        /// <param name="attachments">The list of attachments.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bodyTag"/>,
        /// or <paramref name="attachments"/> is null.</exception>
        public StructuredText(BodyTag bodyTag, IList<Attachment> attachments)
        {
            if (bodyTag == null)
                throw new ArgumentNullException("bodyTag");
            if (attachments == null)
                throw new ArgumentNullException("attachments");

            this.bodyTag = bodyTag;
            this.attachments = attachments;
        }

        /// <summary>
        /// Gets a copy of the body tag that describes the structured text.
        /// </summary>
        public BodyTag BodyTag
        {
            get { return bodyTag.Clone(); }
        }

        /// <summary>
        /// Gets the immutable list of attachments belonging to the structured text.
        /// </summary>
        public IList<Attachment> Attachments
        {
            get { return new ReadOnlyCollection<Attachment>(attachments); }
        }

        /// <summary>
        /// Returns the total length of all <see cref="TextTag" />s that appear within
        /// the structured text body.
        /// </summary>
        /// <returns>The total text length.</returns>
        public int GetTextLength()
        {
            TextLengthVisitor visitor = new TextLengthVisitor();
            bodyTag.Accept(visitor);
            return visitor.Length;
        }

        /// <summary>
        /// Writes the structured text to a markup stream writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
        public void WriteTo(MarkupStreamWriter writer)
        {
            WritePreambleTo(writer);

            bodyTag.WriteTo(writer);
        }

        /// <summary>
        /// Writes the structured text to a markup stream writer and truncates its text
        /// to a particular maximum length, omitting all subsequent contents.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="maxLength">The maximum length of text to write.</param>
        /// <returns>True if truncation occurred.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="writer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxLength"/> is negative.</exception>
        public bool TruncatedWriteTo(MarkupStreamWriter writer, int maxLength)
        {
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength", "Max length must not be negative.");
            WritePreambleTo(writer);

            TruncateTextVisitor visitor = new TruncateTextVisitor(writer, maxLength);
            bodyTag.Accept(visitor);
            return visitor.Truncating;
        }

        private void WritePreambleTo(MarkupStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (Attachment attachment in attachments)
                writer.Container.Attach(attachment);
        }

        /// <summary>
        /// Formats the structured text to a string, discarding unrepresentable formatting details.
        /// </summary>
        /// <returns>The structured text as a string.</returns>
        public override string ToString()
        {
            return bodyTag.ToString();
        }

        /// <inheritdoc />
        public bool Equals(StructuredText other)
        {
            return other != null
                && GenericCollectionUtils.ElementsEqual(attachments, other.attachments)
                && bodyTag.Equals(other.bodyTag);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as StructuredText);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return attachments.Count ^ bodyTag.GetHashCode();
        }

        /// <summary>
        /// Returns true if two structured text objects are equal.
        /// </summary>
        /// <param name="a">The first structured text object to compare.</param>
        /// <param name="b">The second structured text object to compare.</param>
        /// <returns>True if the structured text objects are equal.</returns>
        public static bool operator ==(StructuredText a, StructuredText b)
        {
            return ReferenceEquals(a, b) || ! ReferenceEquals(a, null) && a.Equals(b);
        }

        /// <summary>
        /// Returns true if two structured text objects are not equal.
        /// </summary>
        /// <param name="a">The first structured text object to compare.</param>
        /// <param name="b">The second structured text object to compare.</param>
        /// <returns>True if the structured text objects are not equal.</returns>
        public static bool operator !=(StructuredText a, StructuredText b)
        {
            return !(a == b);
        }

        private sealed class TextLengthVisitor : BaseTagVisitor
        {
            public int Length { get; private set;}

            public override void VisitTextTag(TextTag tag)
            {
                Length += tag.Text.Length;
            }
        }

        private sealed class TruncateTextVisitor : BaseTagVisitor
        {
            private readonly MarkupStreamWriter writer;
            private readonly int maxLength;
            private int length;

            public TruncateTextVisitor(MarkupStreamWriter writer, int maxLength)
            {
                this.writer = writer;
                this.maxLength = maxLength;
            }

            public bool Truncating
            {
                get { return length > maxLength; }
            }

            public override void VisitSectionTag(SectionTag tag)
            {
                if (!Truncating)
                {
                    using (writer.BeginSection(tag.Name))
                        tag.AcceptContents(this);
                }
            }

            public override void VisitMarkerTag(MarkerTag tag)
            {
                if (!Truncating)
                {
                    using (writer.BeginMarker(tag.Marker))
                        tag.AcceptContents(this);
                }
            }

            public override void VisitEmbedTag(EmbedTag tag)
            {
                if (!Truncating)
                {
                    writer.EmbedExisting(tag.AttachmentName);
                }
            }

            public override void VisitTextTag(TextTag tag)
            {
                if (! Truncating)
                {
                    length += tag.Text.Length;

                    if (length > maxLength)
                        writer.Write(tag.Text.Substring(0, tag.Text.Length - length + maxLength));
                    else
                        writer.Write(tag.Text);
                }
            }
        }
    }
}
