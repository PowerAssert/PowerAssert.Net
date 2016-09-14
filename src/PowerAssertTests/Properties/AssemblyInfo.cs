﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if !NOAPPROVALS
using ApprovalTests.Reporters;
using PowerAssertTests.Approvals;
using PowerAssertTests.Approvals.ApprovalTestExtensions;
#endif

[assembly: AssemblyTitle("PowerAssertTests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("PowerAssertTests")]
[assembly: AssemblyCopyright("Copyright © Rob Fonseca-Ensor")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("b66d2326-b90c-4500-9aca-2158fde6e1e4")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#if !NOAPPROVALS
[assembly: FrontLoadedReporter(typeof (CiReporter))]
[assembly: UseReporter(typeof (HappyDiffReporter))]
#endif