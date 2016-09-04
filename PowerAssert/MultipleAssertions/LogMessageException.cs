using System;

namespace PowerAssert.MultipleAssertions
{
    class LogMessageException : Exception
    {
        public LogMessageException(string message) : base(message)
        {
        }

        public override string ToString()
        {
            return "> " + Message;
        }
    }
}