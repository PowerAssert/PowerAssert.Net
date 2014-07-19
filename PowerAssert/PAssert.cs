using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert
{
    public static class PAssert
    {
        [ThreadStatic] internal static Type CurrentTestClass;
        static readonly Assembly MyAssembly = typeof (PAssert).Assembly;
        static readonly string CRLF = Environment.NewLine;


        public static TException Throws<TException>(Action a, Expression<Func<TException, bool>> exceptionAssertion = null) where TException : Exception
        {
            try
            {
                a();
            }
            catch (TException exception)
            {
                return ValidateExceptionAssertion(exceptionAssertion, exception);
            }

            throw new Exception("An exception of type " + typeof (TException).Name + " was expected, but no exception occured");
        }

        public static Task<TException> Throws<TException>(Func<Task> a, Expression<Func<TException, bool>> exceptionAssertion = null) where TException : Exception
        {
            return a().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // Unwrap the AggregateException
                    var baseException = task.Exception.GetBaseException();

                    var exception = baseException as TException;
                    if (exception != null)
                    {
                        return ValidateExceptionAssertion(exceptionAssertion, exception);
                    }

                    throw new Exception("An exception of type " + typeof (TException).Name + " was expected, but it was of type " + baseException.GetType());
                }

                throw new Exception("An exception of type " + typeof (TException).Name + " was expected, but no exception occured");
            });
        }

        static TException ValidateExceptionAssertion<TException>(Expression<Func<TException, bool>> exceptionAssertion, TException exception)
            where TException : Exception
        {
            if (exceptionAssertion != null)
            {
                IsTrue(Substitute(exceptionAssertion, exception));
            }
            return exception;
        }

        static Task<T> TaskFromResult<T>(T value)
        {
            var taskSource = new TaskCompletionSource<T>();
            taskSource.SetResult(value);
            return taskSource.Task;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void IsTrue(Expression<Func<bool>> expression)
        {
            Func<bool> func = expression.Compile();
            bool b;
            try
            {
                b = func();
            }
            catch (Exception e)
            {
                var output = RenderExpression(expression);
                throw new Exception("IsTrue encountered " + e.GetType().Name + ", expression was:" + CRLF + CRLF + output + CRLF + CRLF, e);
            }
            if (!b)
            {
                throw CreateException(expression, "IsTrue failed");
            }
        }

        static Expression<Func<bool>> Substitute<TException>(Expression<Func<TException, bool>> expression, TException exception)
        {
            var parameter = expression.Parameters[0];
            var constant = Expression.Constant(exception, typeof (TException));
            var visitor = new ReplacementVisitor(parameter, constant);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<bool>>(newBody);
        }

        static Exception CreateException(Expression<Func<bool>> expression, string message)
        {
            var output = RenderExpression(expression);
            return new Exception(message + ", expression was:" + CRLF + CRLF + output);
        }

        static string RenderExpression(Expression<Func<bool>> expression)
        {
            CurrentTestClass = new StackTrace(1, false).GetFrames().Select(x => x.GetMethod().DeclaringType).Where(x => x != null).First(x => x.Assembly != MyAssembly);
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] lines = NodeFormatter.Format(constantNode);
            return string.Join(CRLF, lines) + CRLF;
        }
    }
}