using System;
using ApprovalTests.Reporters;

namespace PowerAssertTests.Approvals
{
    public class CiReporter : QuietReporter
    {
        public override bool IsWorkingInThisEnvironment(string forFile)
        {
            return Environment.GetEnvironmentVariable("CI_SERVER") != null;
        }
    }
}