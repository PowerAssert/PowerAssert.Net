using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerAssert.Infrastructure
{
    public class AssertionResult
    {
        public bool Succeeded { get; private set; }
        public string Output { get; private set; }
        public Exception Exception { get; private set; }

        private AssertionResult(bool succeeded, string output, Exception exception)
        {
            this.Succeeded = succeeded;
            this.Output = output;
            this.Exception = exception;
        }

        private static AssertionResult _success = new AssertionResult(true, null, null);
        public static AssertionResult Success
        {
            get { return _success; }
        }

        public static AssertionResult Failure(string output, Exception exception)
        {
            return new AssertionResult(false, output, exception);
        }
    }
}
