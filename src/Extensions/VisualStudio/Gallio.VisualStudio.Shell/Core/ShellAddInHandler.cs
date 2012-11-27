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
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Extensibility;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// The Shell add-in functions as an adjunct to the package to provide the user with
    /// the ability to enable and disable it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is also used to access a few APIs of the DTE (such as AddNamedCommand) that require an add-in instance.
    /// </para>
    /// <para>
    /// This class does not directly use any Gallio types that are defined in other assemblies
    /// because it may not be possible to resolve those types until the shell has been initialized
    /// by <see cref="ShellProxy"/>.
    /// </para>
    /// </remarks>
    [ComVisible(true)]
    public class ShellAddInHandler : IDTExtensibility2, IDTCommandTarget
    {
        private DTE2 dte;
        private AddIn addIn;

        /// <summary>
        /// Creates the add-in handler.
        /// </summary>
        public ShellAddInHandler()
        {
        }

        /// <summary>
        /// Gets the automation object.
        /// </summary>
        public DTE2 DTE
        {
            get { return dte; }
        }

        /// <summary>
        /// Gets the add-in instance.
        /// </summary>
        public AddIn AddIn
        {
            get { return addIn; }
        }

        /// <summary>
        /// Implements the OnConnection method of the IDTExtensibility2 interface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Receives notification that the Add-in is being loaded.
        /// </para>
        /// </remarks>
        /// <param term='application'>Root object of the host application.</param>
        /// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
        /// <param term='addInInst'>Object representing this Add-in.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            dte = (DTE2)application;
            addIn = (AddIn)addInInst;

            ShellProxy.Instance.AddInConnected(this);
        }

        /// <summary>
        /// Implements the OnDisconnection method of the IDTExtensibility2 interface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Receives notification that the Add-in is being unloaded.
        /// </para>
        /// </remarks>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            ShellProxy.Instance.AddInDisconnected();

            addIn = null;
            dte = null;
        }

        /// <summary>
        /// Implements the OnAddInsUpdate method of the IDTExtensibility2 interface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Receives notification when the collection of Add-ins has changed.
        /// </para>
        /// </remarks>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />               
        void IDTExtensibility2.OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>
        /// Implements the OnStartupComplete method of the IDTExtensibility2 interface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Receives notification that the host application has completed loading.
        /// </para>
        /// </remarks>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>
        /// Implements the OnBeginShutdown method of the IDTExtensibility2 interface.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Receives notification that the host application is being unloaded.
        /// </para>
        /// </remarks>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        void IDTExtensibility2.OnBeginShutdown(ref Array custom)
        {
        }

        void IDTCommandTarget.QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            vsCommandStatus statusOptionCopy = statusOption;
            object commandTextCopy = commandText;

            ShellProxy.Instance.InvokeHook(hooks => hooks.HandleQueryStatus(commandName, neededText, ref statusOptionCopy, ref commandTextCopy));

            statusOption = statusOptionCopy;
            commandText = commandTextCopy;
        }

        void IDTCommandTarget.Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            object variantInCopy = variantIn;
            object variantOutCopy = variantOut;
            bool handledCopy = handled;

            ShellProxy.Instance.InvokeHook(hooks => hooks.HandleExec(commandName, executeOption, ref variantInCopy, ref variantOutCopy, ref handledCopy));

            variantIn = variantInCopy;
            variantOut = variantOutCopy;
            handled = handledCopy;
        }
    }
}
