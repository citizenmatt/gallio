﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.ActionManagement;
using JetBrains.UI;

#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
using JetBrains.UI.Shell.PluginSupport;
#elif RESHARPER_60_OR_NEWER
using JetBrains.Application.PluginSupport;
#else
using JetBrains.UI.Application.PluginSupport;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Gallio.ReSharperRunner")]
[assembly: AssemblyDescription("Gallio test runner for ReSharper.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Gallio")]
[assembly: AssemblyCopyright("Copyright © 2005-2010 Gallio Project - http://www.gallio.org/")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6a16e984-55e4-486e-9028-d79806561088")]

// Don't care about CLS compliance for this assembly.
[assembly: CLSCompliant(false)]

// The neutral resources language is US English.
// Telling the system that this is the case yields a small performance improvement during startup.
[assembly: NeutralResourcesLanguage("en-US")]

[assembly: InternalsVisibleTo("Gallio.ReSharperRunner.Tests")]

// ReSharper plugin attributes.
[assembly: PluginTitle("Gallio Test Runner")]
[assembly: PluginDescription("Gallio test runner for ReSharper.")]
[assembly: PluginVendor("Gallio")]

// ReSharper resources
[assembly: ActionsXml("Gallio.ReSharperRunner.Resources.Actions.xml")]
#if !RESHARPER_70
[assembly: ImagesBase("Gallio.ReSharperRunner.Resources")]
#endif