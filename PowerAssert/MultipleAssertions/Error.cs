using System;
using System.Diagnostics;
using System.Linq;

namespace PowerAssert.MultipleAssertions
{
    public class Error
    {
        internal static readonly string Crlf = Environment.NewLine;

        static readonly string Seperator = new string('=', 60);

        public Error(string message, CallerLocation location)
        {
            Message = message;
            Location = location.ToString();
        }


        public Error(Exception exception, CallerLocation location):this(exception.Message, location)
        {
            Exception = exception;
            CausesFail = true;
        }


        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string Location { get; set; }

        public bool CausesFail { get; set; }

        public override string ToString()
        {
            if(!CausesFail)
            {
                return string.Concat("> ", Message, Crlf);
            }
            return string.Concat(Seperator, Crlf, "ERROR ", Location, ":", Crlf, Seperator, Crlf, Message, Crlf, Seperator, Crlf, Crlf);

        }
    }
}
