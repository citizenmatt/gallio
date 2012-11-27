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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Media;
using Gallio.Common.Normalization;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// An attachment is an embedded object in a markup document.  An attachment must specify a
    /// content type (a MIME type), and some contents.
    /// </summary>
    [Serializable]
    public abstract class Attachment : INormalizable<Attachment>
    {
        private readonly string name;
        private readonly string contentType;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The name of attachment, or null to automatically assign one.  The attachment
        /// name must be unique within the scope of the currently executing test step.</param>
        /// <param name="contentType">The content type, not null.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> is null.</exception>
        internal /*to prevent subclassing outside of the framework*/ Attachment(string name, string contentType)
        {
			if (name != null && name.Length > 100)
				throw new ArgumentException("name must be 100 chars or less", "name");

            if (contentType == null)
                throw new ArgumentNullException("contentType");

            this.name = name ?? Hash64.CreateUniqueHash().ToString();
            this.contentType = contentType;
        }

        /// <summary>
        /// Gets the name of the attachment, not null.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the content type of the attachment specified as a MIME type, not null.
        /// <seealso cref="MimeTypes"/> for definitions of common supported MIME types.
        /// </summary>
        public string ContentType
        {
            get { return contentType; }
        }

        /// <summary>
        /// Creates a plain text attachment.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="text">The text string, not null.</param>
        /// <returns>The attachment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public static TextAttachment CreatePlainTextAttachment(string name, string text)
        {
            return new TextAttachment(name, MimeTypes.PlainText, text);
        }

        /// <summary>
        /// Creates an HTML attachment.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="html">The html string, not null.</param>
        /// <returns>The attachment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null.</exception>
        public static TextAttachment CreateHtmlAttachment(string name, string html)
        {
            return new TextAttachment(name, MimeTypes.Html, html);
        }

        /// <summary>
        /// Creates an XHTML attachment.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="xhtml">The xhtml string, not null.</param>
        /// <returns>The attachment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null.</exception>
        public static TextAttachment CreateXHtmlAttachment(string name, string xhtml)
        {
            return new TextAttachment(name, MimeTypes.XHtml, xhtml);
        }

        /// <summary>
        /// Creates an XML attachment.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="xml">The XML string, not null.</param>
        /// <returns>The attachment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null.</exception>
        public static TextAttachment CreateXmlAttachment(string name, string xml)
        {
            return new TextAttachment(name, MimeTypes.Xml, xml);
        }

        /// <summary>
        /// Creates an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="image">The image to attach.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        public static BinaryAttachment CreateImageAttachment(string name, Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            // TODO: Choose a better mime-type based on the image format.
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return new BinaryAttachment(name, MimeTypes.Png, stream.ToArray());
            }
        }

        /// <summary>
        /// Creates an video attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="video">The video to attach.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="video"/> is null.</exception>
        public static BinaryAttachment CreateVideoAttachment(string name, Video video)
        {
            if (video == null)
                throw new ArgumentNullException("video");

            using (MemoryStream stream = new MemoryStream())
            {
                video.Save(stream);
                return new BinaryAttachment(name, video.MimeType, stream.ToArray());
            }
        }

        /// <summary>
        /// Creates XML attachment with mime-type <see cref="MimeTypes.Xml" /> by serializing an object.
        /// </summary>
        /// <param name="name">The attachment name, or null to automatically assign one.</param>
        /// <param name="obj">The object to serialize and embed, must not be null.</param>
        /// <param name="xmlSerializer">The xml serializer to use, or null to use the default based on the object's type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        /// <seealso cref="XmlSerializer"/>
        public static TextAttachment CreateObjectAsXmlAttachment(string name, object obj, XmlSerializer xmlSerializer)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (xmlSerializer == null)
                xmlSerializer = new XmlSerializer(obj.GetType());

            using (StringWriter writer = new StringWriter())
            {
                xmlSerializer.Serialize(writer, obj);
                return CreateXmlAttachment(name, writer.ToString());
            }
        }

        /// <summary>
        /// Generates serializable attachment data from an attachment.
        /// </summary>
        /// <returns>The attachment data.</returns>
        public abstract AttachmentData ToAttachmentData();

        /// <summary>
        /// Recovers the attachment information from serializable attachment data.
        /// </summary>
        /// <param name="data">The attachment data.</param>
        /// <returns>The attachment.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
        public static Attachment FromAttachmentData(AttachmentData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return (data.Type == AttachmentType.Text)
                ? (Attachment)new TextAttachment(data.Name, data.ContentType, data.GetText())
                : (Attachment)new BinaryAttachment(data.Name, data.ContentType, data.GetBytes());
        }

        /// <inheritdoc />
        public abstract Attachment Normalize();
    }
}