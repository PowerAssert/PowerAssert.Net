#if !NOAPPROVALS
using System.IO;
using ApprovalTests.Core;

namespace PowerAssertTests.Approvals.ApprovalTestExtensions
{
    public class OverwriteReporter : IApprovalFailureReporter, IReporterWithApprovalPower
    {
        public void Report(string approved, string received)
        {
            File.Copy(received, approved, true);
        }

        public bool ApprovedWhenReported()
        {
            return true;
        }
    }
}
#endif