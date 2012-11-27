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
using System.Xml;
using Gallio.Common;
using Gallio.Common.Markup;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A report writer provides services for formatting or saving a report
    /// to a <see cref="IReportContainer" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The writer tracks the paths of the report documents paths
    /// that are created within the report container.  Likewise it remembers whether
    /// attachments have been saved so as to avoid redundantly resaving them when
    /// the same report is generated in multiple formats.
    /// </para>
    /// </remarks>
    public interface IReportWriter
    {
        /// <summary>
        /// Gets the report being generated.
        /// </summary>
        Report Report { get; }

        /// <summary>
        /// Gets the report container.
        /// </summary>
        IReportContainer ReportContainer { get; }

        /// <summary>
        /// Gets the paths of all report documents that have been generated within
        /// the report container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Report documents are the primary files when reports
        /// are formatted or saved such as an XML file, a Text file, or the main
        /// HTML file that contains the body of the report.
        /// </para>
        /// </remarks>
        IList<string> ReportDocumentPaths { get; }

        /// <summary>
        /// Adds the path of a report document that has been created within the report container.
        /// </summary>
        /// <param name="path">The document path.</param>
        void AddReportDocumentPath(string path);

        /// <summary>
        /// Serializes the report to XML.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        /// <param name="attachmentContentDisposition">The content disposition to use for all attachments.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="xmlWriter"/> is null.</exception>
        void SerializeReport(XmlWriter xmlWriter, AttachmentContentDisposition attachmentContentDisposition);

        /// <summary>
        /// Saves the report as an XML file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The saved report XML file is automatically added to the <see cref="ReportDocumentPaths" /> list.
        /// </para>
        /// <para>
        /// The path of the saved report is constructed by appending the extension ".xml"
        /// to the container's <see cref="IReportContainer.ReportName" />.
        /// </para>
        /// <para>
        /// This method may do nothing if the report has already been saved.
        /// </para>
        /// </remarks>
        /// <param name="attachmentContentDisposition">The content disposition to use for all attachments.  If
        /// the content disposition is <see cref="AttachmentContentDisposition.Link" /> then
        /// this method will automatically call <see cref="SaveReportAttachments" /> to save the attachments.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown <paramref name="progressMonitor" /> is null.</exception>
        void SaveReport(AttachmentContentDisposition attachmentContentDisposition, IProgressMonitor progressMonitor);

        /// <summary>
        /// Saves all report attachments as individual content files to the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The convention for attachments is to save them in a directory with the same name
        /// as the container's <see cref="IReportContainer.ReportName" /> arranged in folders
        /// first by test id then by step id then by attachment name.
        /// For example: "Report\{testid}\{stepid}\attachment.jpg".
        /// </para>
        /// <para>
        /// This method may do nothing if the attachments have already been saved.
        /// </para>
        /// </remarks>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null.</exception>
        void SaveReportAttachments(IProgressMonitor progressMonitor);

        /// <summary>
        /// Perform an action with updated content paths and dispositions
        /// </summary>
        /// <param name="attachmentContentDisposition">The attachment content disposition to use.</param>
        /// <param name="action">The action to perform.</param>
        void WithUpdatedContentPathsAndDisposition(AttachmentContentDisposition attachmentContentDisposition, Action action);
    }
}
