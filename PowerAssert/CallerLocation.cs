using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PowerAssert
{
    public class CallerLocation
    {
        static Assembly MyAssembly = typeof(CallerLocation).Assembly;

        public string FileName { get; private set; }
        public int LineNumber { get; private set; }
        public MethodBase Method { get; private set; }

        public CallerLocation(StackFrame f)
        {
            if (f == null)
            {
                FileName = "";
                LineNumber = 0;
                Method = null;
            }
            else
            {
                FileName = f.GetFileName();
                LineNumber = f.GetFileLineNumber();
                Method = f.GetMethod();
            }
        }

        public Type DeclaringType
        {
            get { return Method == null ? null : Method.DeclaringType; }
        }

        public override string ToString()
        {
            if (Method == null)
            {
                return "(Unknown location)";
            }
            var ret = string.Format("in {0}.{1}", Method.DeclaringType.Name, Method.Name);

            if (!string.IsNullOrEmpty(FileName))
            {
                ret += string.Format(" at {0}:{1}", FileName, LineNumber);
            }
            return ret;
        }

        public static CallerLocation Unknown = new CallerLocation(null);

        public static CallerLocation FindFromStackFrames()
        {
            var stackFrames = new StackTrace(1, true).GetFrames();
            var location = (
                    from f in stackFrames
                    let m = f.GetMethod()
                    where m != null && m.DeclaringType.Assembly != MyAssembly
                    select new CallerLocation(f)
                ).FirstOrDefault();

            return location ?? CallerLocation.Unknown;
        }

        internal static CallerLocation Ensure(CallerLocation location)
        {
            return location ?? FindFromStackFrames();
        }
    }
}
