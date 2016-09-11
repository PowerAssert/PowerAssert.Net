using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PowerAssert.MultipleAssertions
{
    public class Error
    {
        static Assembly MyAssembly = typeof(Error).GetTypeInfo().Assembly;

        internal static readonly string Crlf = Environment.NewLine;

        static readonly string Seperator = new string('=', 60);


        public Error(string message)
        {
            Message = message;
            var stackFrames = from f in new StackTrace(1, true).GetFrames()
                let m = f.GetMethod()
                where m != null
                let t = m.DeclaringType
                where t.Assembly != MyAssembly
                select f;
            var frame = stackFrames.FirstOrDefault();
            if (frame != null)
            {
                var method = frame.GetMethod();
                var typeName = method.DeclaringType == null ? "" : method.DeclaringType.Name;
                Location = string.Format("in {0}.{1} at {2}:{3}", typeName, method.Name, frame.GetFileName(), frame.GetFileLineNumber());
            }
            else
            {
                Location = "(Unknown location)";
            }

        }


        public Error(Exception exception):this(exception.Message)
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