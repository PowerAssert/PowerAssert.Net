using System;
using System.IO;
using ApprovalTests.Core;

namespace PowerAssertTests.Approvals.ApprovalTestExtensions
{
    public class CiReporter : IEnvironmentAwareReporter, IApprovalFailureReporter
    {
        public bool IsWorkingInThisEnvironment(string forFile)
        {
            return Environment.GetEnvironmentVariable("CI_SERVER") != null;
        }

        public void Report(string approved, string received)
        {
            Console.Out.WriteLine("APPROVED:");
            Console.Out.WriteLine(File.ReadAllText(approved));
            Console.Out.WriteLine();
            Console.Out.WriteLine("RECEIVED:");
            Console.Out.WriteLine(File.ReadAllText(received));
        }
    }
}