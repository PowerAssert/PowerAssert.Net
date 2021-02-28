using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
using ApprovalTests.Reporters;
using PowerAssertTests.Approvals;
using PowerAssertTests.Approvals.ApprovalTestExtensions;

[assembly: FrontLoadedReporter(typeof (CiReporter))]
[assembly: UseReporter(typeof (HappyDiffReporter))]