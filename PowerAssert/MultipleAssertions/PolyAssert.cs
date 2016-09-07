using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
// ReSharper disable ExplicitCallerInfoArgument (inner calls)

namespace PowerAssert.MultipleAssertions
{
    public class PolyAssert : IDisposable
    {
        readonly List<Error> _errors = new List<Error>();

        /// <summary>
        /// Write a log message to be printed IF the PolyAssert has any errors
        /// </summary>
        public void Log(string s, CallerLocation location = null)
        {
            _errors.Add(new Error(s, CallerLocation.Ensure(location)));
        }

        /// <summary>
        /// Calls PAssert.IsTrue and stores the exception, if one occurs
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void IsTrue(Expression<Func<bool>> expression, CallerLocation location = null)
        {
            Try(() => PAssert.IsTrue(expression), CallerLocation.Ensure(location));
        }

        /// <summary>
        /// Calls PAssert.IsTrue and stores the exception, if one occurs
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void IsTrue<TTarget>(TTarget target, Expression<Func<TTarget, bool>> expression, CallerLocation location = null)
        {
            Try(() => PAssert.IsTrue(target, expression), CallerLocation.Ensure(location));
        }

        /// <summary>
        /// Runs any action and stores the exception, if one occurs
        /// </summary>
        public void Try(Action action, CallerLocation location = null)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _errors.Add(new Error(e, CallerLocation.Ensure(location)));
            }
        }

        /// <summary>
        /// Stores a failure message, if shouldFail is true
        /// </summary>
        public void FailIf(bool shouldFail, string message, CallerLocation location = null)
        {
            if (shouldFail)
            {
                _errors.Add(new Error(message, CallerLocation.Ensure(location)) { CausesFail = true});
            }
        }

        public void Dispose()
        {
            StopIfErrorsHaveOccurred();
        }

        /// <summary>
        /// If any errors have happened already, raise them all now (prevents further test execution)
        /// </summary>
        public void StopIfErrorsHaveOccurred()
        {
            if (_errors.Any(x => x.CausesFail))
            {
                throw new PolyAssertException(_errors);
            }
        }
    }
}
