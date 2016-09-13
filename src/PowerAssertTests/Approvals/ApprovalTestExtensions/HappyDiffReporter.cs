#if !NOAPPROVALS
using ApprovalTests.Core;
using ApprovalTests.Reporters;

namespace PowerAssertTests.Approvals.ApprovalTestExtensions
{
    public class HappyDiffReporter : FirstWorkingReporter
    {
        public HappyDiffReporter()
            : base(
                (IEnvironmentAwareReporter) CodeCompareReporter.INSTANCE,
                (IEnvironmentAwareReporter) BeyondCompareReporter.INSTANCE,
                (IEnvironmentAwareReporter) TortoiseDiffReporter.INSTANCE,
                (IEnvironmentAwareReporter) AraxisMergeReporter.INSTANCE,
                (IEnvironmentAwareReporter) P4MergeReporter.INSTANCE,
                (IEnvironmentAwareReporter) WinMergeReporter.INSTANCE,
                (IEnvironmentAwareReporter) KDiffReporter.INSTANCE,
                (IEnvironmentAwareReporter) FrameworkAssertReporter.INSTANCE,
                (IEnvironmentAwareReporter) QuietReporter.INSTANCE)
        {
        }
    }
}
#endif