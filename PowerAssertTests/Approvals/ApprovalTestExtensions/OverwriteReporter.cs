using System.IO;
using ApprovalTests.Core;

namespace PowerAssertTests.Approvals.ApprovalTestExtensions
{
    public class OverwriteReporter : IApprovalFailureReporter
    {
        public void Report(string approved, string received)
        {
            File.Copy(received, approved, true);
        }
    }
}