using System;
using System.Collections.Generic;
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
        public void Log(string s, [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            _errors.Add(new Error(s, path, line));
        }

        /// <summary>
        /// Calls PAssert.IsTrue and stores the exception, if one occurs
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void IsTrue(Expression<Func<bool>> expression, [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            Try(() => PAssert.IsTrue(expression), path, line);
        }

        /// <summary>
        /// Calls PAssert.IsTrue and stores the exception, if one occurs
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void IsTrue<TTarget>(TTarget target, Expression<Func<TTarget, bool>> expression, [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            Try(() => PAssert.IsTrue(target, expression), path, line);
        }

        /// <summary>
        /// Runs any action and stores the exception, if one occurs
        /// </summary>
        public void Try(Action action, [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _errors.Add(new Error(e, path, line));
            }
        }

        /// <summary>
        /// Stores a failure message, if shouldFail is true
        /// </summary>
        public void FailIf(bool shouldFail, string message, [CallerFilePath] string path = null, [CallerLineNumber] int line = 0)
        {
            if (shouldFail)
            {
                _errors.Add(new Error(message, path, line) { CausesFail = true});
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