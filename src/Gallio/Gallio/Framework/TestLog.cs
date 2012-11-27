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
using System.Drawing;
using System.Xml.Serialization;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using Gallio.Common.Media;

namespace Gallio.Framework
{
    /// <summary>
    /// The test log class provides services for writing information to the
    /// execution log associated with a test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test log records the output of a test during its execution including any text
    /// that was written to console output streams, exceptions that occurred, and 
    /// anything else the test writer might want to save.
    /// </para>
    /// <para>
    /// A log consists of zero or more log streams that are opened automatically
    /// on demand to capture independent sequences of log output.  Each stream can
    /// further be broken down into possibly nested sections to classify output
    /// during different phases of test execution (useful for drilling into complex tests).
    /// In addition to text, a log can contain attachments that are either attached
    /// at the top level of the log or embedded into log streams.  Attachments are
    /// typed by mime-type and can contain Text, Xml, Images, Blobs, or any other content.
    /// Certain test frameworks may automatically create attachments to gather all manner
    /// of diagnostic information over the course of the test.
    /// </para>
    /// <para>
    /// Test log messages are not echoed to the console or runtime log during test execution,
    /// they are incorporated into the test report.  Use <see cref="DiagnosticLog" /> instead if you
    /// wish to write messages immediately where the operator can see them.
    /// </para>
    /// </remarks>
    /// <seealso cref="DiagnosticLog"/>
    public static class TestLog
    {
        #region Current log writer accessor
        /// <summary>
        /// Gets the current log writer.
        /// </summary>
        /// <returns>The execution log, never null.</returns>
        /// <exception cref="InvalidOperationException">Thrown if there is no current log writer.</exception>
        public static MarkupDocumentWriter Writer
        {
            get
            {
                TestContext context = TestContext.CurrentContext;
                if (context == null)
                    throw new InvalidOperationException("There is no test context currently available.  Consequently there is no current log writer.");
                return context.LogWriter;
            }
        }
        #endregion

        #region Current log stream writer accessors
        /// <summary>
        /// Gets the current stream writer for captured <see cref="Console.In" /> messages.
        /// </summary>
        public static MarkupStreamWriter ConsoleInput
        {
            get { return Writer.ConsoleInput; }
        }

        /// <summary>
        /// Gets the current stream writer for captured <see cref="Console.Out" /> messages.
        /// </summary>
        public static MarkupStreamWriter ConsoleOutput
        {
            get { return Writer.ConsoleOutput; }
        }

        /// <summary>
        /// Gets the current stream writer for captured <see cref="Console.Error" /> messages.
        /// </summary>
        public static MarkupStreamWriter ConsoleError
        {
            get { return Writer.ConsoleError; }
        }

        /// <summary>
        /// Gets the current stream writer for diagnostic <see cref="Debug" /> and <see cref="Trace" /> messages.
        /// </summary>
        public static MarkupStreamWriter DebugTrace
        {
            get { return Writer.DebugTrace; }
        }

        /// <summary>
        /// Gets the current stream writer for reporting failure messages and exceptions.
        /// </summary>
        public static MarkupStreamWriter Failures
        {
            get { return Writer.Failures; }
        }

        /// <summary>
        /// Gets the current stream writer for reporting warning messages.
        /// </summary>
        public static MarkupStreamWriter Warnings
        {
            get { return Writer.Warnings; }
        }

        /// <summary>
        /// Gets the current stream writer for generic log messages such as those written
        /// using the static methods of this class.
        /// </summary>
        public static MarkupStreamWriter Default
        {
            get { return Writer.Default; }
        }
        #endregion

        #region Current log writer shortcuts
        /// <summary>
        /// Flushes the log writer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        public static void Flush()
        {
            Writer.Flush();
        }

        /// <summary>
        /// Attaches an attachment to the execution log.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachment">The attachment to include.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.Attach"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static Attachment Attach(Attachment attachment)
        {
            return Writer.Attach(attachment);
        }

        /// <summary>
        /// Attaches an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment AttachPlainText(string attachmentName, string text)
        {
            return Writer.AttachPlainText(attachmentName, text);
        }

        /// <summary>
        /// Attaches an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment AttachHtml(string attachmentName, string html)
        {
            return Writer.AttachHtml(attachmentName, html);
        }

        /// <summary>
        /// Attaches an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment AttachXHtml(string attachmentName, string xhtml)
        {
            return Writer.AttachXHtml(attachmentName, xhtml);
        }

        /// <summary>
        /// Attaches an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment AttachXml(string attachmentName, string xml)
        {
            return Writer.AttachXml(attachmentName, xml);
        }

        /// <summary>
        /// Attaches an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static BinaryAttachment AttachImage(string attachmentName, Image image)
        {
            return Writer.AttachImage(attachmentName, image);
        }

        /// <summary>
        /// Attaches a video attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="video">The video to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachVideo"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="video"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static BinaryAttachment AttachVideo(string attachmentName, Video video)
        {
            return Writer.AttachVideo(attachmentName, video);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachObjectAsXml(string, object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment AttachObjectAsXml(string attachmentName, object obj)
        {
            return Writer.AttachObjectAsXml(attachmentName, obj);
        }

        /// <summary>
        /// Attaches an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current log
        /// writer as returned by the <see cref="Writer" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null.</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupDocumentWriter.AttachObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment AttachObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return Writer.AttachObjectAsXml(attachmentName, obj, xmlSerializer);
        }
        #endregion

        #region Current default log stream writer shortcuts
        /// <summary>
        /// Writes a character.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The character value.</param>
        public static void Write(char value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The string value.</param>
        public static void Write(string value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes a formatted object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The object value.</param>
        public static void Write(object value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes an array of characters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The array of characters.</param>
        public static void Write(char[] value)
        {
            Default.Write(value);
        }

        /// <summary>
        /// Writes an array of characters.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="buffer">The character buffer.</param>
        /// <param name="index">The index of the first character in the buffer to write.</param>
        /// <param name="count">The number of characters from the buffer to write.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> or <paramref name="count"/> are out of range.</exception>
        public static void Write(char[] buffer, int index, int count)
        {
            Default.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes a formatted string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format string arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="format"/> is null.</exception>
        /// <seealso cref="String.Format(string, object[])"/>
        public static void Write(string format, params object[] args)
        {
            Default.Write(format, args);
        }

        /// <summary>
        /// Writes a line delimiter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        public static void WriteLine()
        {
            Default.WriteLine();
        }

        /// <summary>
        /// Writes a character followed by a line delimiter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The character value.</param>
        public static void WriteLine(char value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes a string followed by a line delimiter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The string value.</param>
        public static void WriteLine(string value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes a formatted object followed by a line delimiter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The object value.</param>
        public static void WriteLine(object value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes an array of characters followed by a line delimiter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="value">The array of characters.</param>
        public static void WriteLine(char[] value)
        {
            Default.WriteLine(value);
        }

        /// <summary>
        /// Writes a formatted string followed by a line delimiter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="format">The format string.</param>
        /// <param name="args">The format string arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="format"/> is null.</exception>
        /// <seealso cref="String.Format(string, object[])"/>
        public static void WriteLine(string format, params object[] args)
        {
            Default.WriteLine(format, args);
        }

        /// <summary>
        /// Writes a test log stream writable object to the stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="obj">The object to write, or null if none.</param>
        public static void Write(IMarkupStreamWritable obj)
        {
            Default.Write(obj);
        }

        /// <summary>
        /// Writes an exception.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="exception">The exception to write.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public static void WriteException(Exception exception)
        {
            Default.WriteException(exception);
        }

        /// <summary>
        /// Writes an exception within its own section.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="exception">The exception to write.</param>
        /// <param name="sectionName">The section name, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>,
        /// or <paramref name="sectionName"/> is null.</exception>
        public static void WriteException(Exception exception, string sectionName)
        {
            Default.WriteException(exception, sectionName);
        }

        /// <summary>
        /// Writes an exception.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="exception">The exception data to write.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public static void WriteException(ExceptionData exception)
        {
            Default.WriteException(exception);
        }

        /// <summary>
        /// Writes an exception within its own section which provides additional cues for interpretation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// <para>
        /// The exception will not be terminated by a new line.
        /// </para>
        /// </remarks>
        /// <param name="exception">The exception data to write.</param>
        /// <param name="sectionName">The section name, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is null.</exception>
        public static void WriteException(ExceptionData exception, string sectionName)
        {
            Default.WriteException(exception, sectionName);
        }

        /// <summary>
        /// Writes highlighted text.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Highlights can be used to emphasize important information such differences 
        /// between similar expected and actual values.
        /// </para>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// <para>
        /// This is a convenience method that simply encapsulates the highlighted text within a
        /// marker region of type <see cref="Marker.Highlight" />.
        /// </para>
        /// </remarks>
        /// <param name="text">The text to write, or null if none.</param>
        public static void WriteHighlighted(string text)
        {
            Default.WriteHighlighted(text);
        }

        /// <summary>
        /// Writes an ellipsis to indicate where content has been elided for brevity.
        /// </summary>
        /// <remarks>
        /// <para>
        /// An ellipsis may be used, for example, when printing assertion failures to clearly
        /// identify sections where the user is not being presented all of the information
        /// because it was too long and had to be truncated.
        /// </para>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// <para>
        /// This is a convenience method that simply encapsulates "..." within a
        /// marked region of type <see cref="Marker.Ellipsis" />.  However, tools
        /// may reinterpret the special marker to make the "..." less ambiguous.
        /// </para>
        /// </remarks>
        public static void WriteEllipsis()
        {
            Default.WriteEllipsis();
        }

        /// <summary>
        /// Begins a section with the specified name.  May be nested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A section groups together related content in the test log to make it
        /// easier to distinguish.  The section name is used as a heading.
        /// </para>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// using (Log.BeginSection("Doing something interesting"))
        /// {
        ///     Log.WriteLine("Ah ha!");
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="sectionName">The name of the section.</param>
        /// <returns>A Disposable object that calls <see cref="End" /> when disposed.  This
        /// is a convenience for use with the C# "using" statement.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sectionName"/> is null.</exception>
        public static IDisposable BeginSection(string sectionName)
        {
            return Default.BeginSection(sectionName);
        }

        /// <summary>
        /// Begins a marked region.  May be nested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A marker is a hidden tag that labels its contents with a semantic class.
        /// It is roughly equivalent in operation to an HTML "span" tag.  Various tools
        /// may inspect the markers and modify the presentation accordingly.
        /// </para>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// using (Log.BeginMarker(Marker.Monospace))
        /// {
        ///     Log.WriteLine(contents);
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="marker">The marker.</param>
        /// <returns>A Disposable object that calls <see cref="End" /> when disposed.  This
        /// is a convenience for use with the C# "using" statement.</returns>
        public static IDisposable BeginMarker(Marker marker)
        {
            return Default.BeginMarker(marker);
        }

        /// <summary>
        /// Ends the region most recently started with one of the Begin* methods.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if there is no current nested region.</exception>
        public static void End()
        {
            Default.End();
        }

        /// <summary>
        /// Embeds an attachment.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <remarks>
        /// An attachment instance can be embedded multiple times efficiently since each
        /// embedded copy is typically represented as a link to the same common attachment instance.
        /// </remarks>
        /// <param name="attachment">The attachment to embed.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.Embed"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static Attachment Embed(Attachment attachment)
        {
            return Default.Embed(attachment);
        }

        /// <summary>
        /// Embeds another copy of an existing attachment.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <remarks>
        /// <para>
        /// This method can be used to
        /// repeatedly embed an existing attachment at multiple points in multiple
        /// streams without needing to keep the <see cref="Attachment" /> instance
        /// itself around.  This can help to reduce memory footprint since the
        /// original <see cref="Attachment" /> instance can be garbage collected shortly
        /// after it is first attached.
        /// </para>
        /// <para>
        /// An attachment instance can be embedded multiple times efficiently since each
        /// embedded copy is typically represented as a link to the same common attachment instance.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the existing attachment to embed.</param>
        /// <seealso cref="MarkupStreamWriter.EmbedExisting"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachmentName"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if no attachment with the specified
        /// name has been previously attached.</exception>
        public static void EmbedExisting(string attachmentName)
        {
            Default.EmbedExisting(attachmentName);
        }

        /// <summary>
        /// Embeds an plain text attachment with mime-type <see cref="MimeTypes.PlainText" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="text">The text to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedPlainText"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment EmbedPlainText(string attachmentName, string text)
        {
            return Default.EmbedPlainText(attachmentName, text);
        }

        /// <summary>
        /// Embeds an HTML attachment with mime-type <see cref="MimeTypes.Html" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="html">The HTML to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="html"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment EmbedHtml(string attachmentName, string html)
        {
            return Default.EmbedHtml(attachmentName, html);
        }

        /// <summary>
        /// Embeds an XHTML attachment with mime-type <see cref="MimeTypes.XHtml" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xhtml">The XHTML to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedXHtml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xhtml"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment EmbedXHtml(string attachmentName, string xhtml)
        {
            return Default.EmbedXHtml(attachmentName, xhtml);
        }

        /// <summary>
        /// Embeds an XML attachment with mime-type <see cref="MimeTypes.Xml" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="xml">The XML to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedXml"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xml"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment EmbedXml(string attachmentName, string xml)
        {
            return Default.EmbedXml(attachmentName, xml);
        }

        /// <summary>
        /// Embeds an image attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="image">The image to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedImage"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static BinaryAttachment EmbedImage(string attachmentName, Image image)
        {
            return Default.EmbedImage(attachmentName, image);
        }

        /// <summary>
        /// Embeds a video attachment with a mime-type compatible with its internal representation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="video">The video to attach.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedVideo"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="video"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static BinaryAttachment EmbedVideo(string attachmentName, Video video)
        {
            return Default.EmbedVideo(attachmentName, video);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the default <see cref="XmlSerializer" /> for the object's type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedObjectAsXml(string, object)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment EmbedObjectAsXml(string attachmentName, object obj)
        {
            return Default.EmbedObjectAsXml(attachmentName, obj);
        }

        /// <summary>
        /// Embeds an XML-serialized object as an XML attachment with mime-type <see cref="MimeTypes.Xml" />
        /// using the specified <see cref="XmlSerializer" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a convenience method that forwards the request to the current default
        /// log stream writer as returned by the <see cref="Default" /> property.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The name of the attachment to create or null to
        /// automatically assign one.  The attachment name must be unique within the scope of the
        /// currently executing test step.</param>
        /// <param name="obj">The object to serialize and embed, must not be null.</param>
        /// <param name="xmlSerializer">The <see cref="XmlSerializer" /> to use, or null to use the default <see cref="XmlSerializer" />
        /// for the object's type.</param>
        /// <returns>The attachment.</returns>
        /// <seealso cref="MarkupStreamWriter.EmbedObjectAsXml(string, object, XmlSerializer)"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is already an attachment
        /// with the same name.</exception>
        public static TextAttachment EmbedObjectAsXml(string attachmentName, object obj, XmlSerializer xmlSerializer)
        {
            return Default.EmbedObjectAsXml(attachmentName, obj, xmlSerializer);
        }
        #endregion
    }
}