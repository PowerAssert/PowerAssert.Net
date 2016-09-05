using System;
using System.Linq;

namespace PowerAssert.MultipleAssertions
{
    public class Error
    {
        internal static readonly string Crlf = Environment.NewLine;
        static readonly string Seperator = new string('=', 60);

        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string Path { get; set; }
        public int Line { get; set; }

        public bool CausesFail { get; set; }

        public Error(string message, string path, int line)
        {
            Message = message;
            Path = path;
            Line = line;
        }

        public Error(Exception exception, string path, int line):this(exception.Message, path, line)
        {
            Exception = exception;
            CausesFail = true;
        }

        public override string ToString()
        {
            if(!CausesFail)
            {
                return string.Concat("> ", Path, ":", Line, "> ", Message, Crlf);
            }
            return string.Concat(Seperator, Crlf, "ERROR ", Path, ":", Line, ":", Crlf, Seperator, Crlf, Message, Crlf, Seperator, Crlf, Crlf);

        }
    }
}