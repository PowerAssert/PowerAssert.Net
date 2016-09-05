using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerAssert.MultipleAssertions
{
    public class PolyAssertException : Exception
    {
        public IReadOnlyList<Error> Errors { get; set; }

        public PolyAssertException(IReadOnlyList<Error> errors) : base(BuildMessage(errors))
        {
            Errors = errors;
        }

        static string BuildMessage(IReadOnlyList<Error> errors)
        {
            int fails = errors.Count(x => x.CausesFail);

            return string.Format("PolyAssert encountered {0} failures:{1}", fails, Error.Crlf)+string.Concat(errors);
        }

        
    }
}