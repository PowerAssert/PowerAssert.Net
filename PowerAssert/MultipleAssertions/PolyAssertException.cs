using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerAssert.MultipleAssertions
{
    public class PolyAssertException : AggregateException
    {
        static readonly string Crlf = Environment.NewLine;
        static readonly string Seperator = Crlf + new string('=', 60) + Crlf;

        public PolyAssertException(IEnumerable<Exception> innerExceptions) : base(innerExceptions)
        {
        }

        public override string ToString()
        {
            int count = InnerExceptions.Count(PolyAssert.ExceptionIsNotLogMessage);
            var strings = new [] {string.Format("PolyAssert encountered {0} failures:", count), StackTrace, Seperator}.Concat(InnerExceptions.Select(FormatInnerException));

            return string.Join(Crlf + Crlf, strings);
        }

        static string FormatInnerException(Exception e)
        {
            return e is LogMessageException ? e.ToString() : Seperator + e + Seperator;
        }
    }
}