using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;
using PowerAssert.MultipleAssertions;

namespace PowerAssert
{
    public static class PAssert
    {
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

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void IsTrue<T>(T target, Expression<Func<T, bool>> expression)
        {
            Func<T, bool> func = expression.Compile();
            bool b;
            try
            {
                b = func(target);
            }
            catch (Exception e)
            {
                var output = RenderExpression(expression, target);
                throw new Exception("IsTrue encountered " + e.GetType().Name + ", expression was:" + CRLF + CRLF + output + CRLF + CRLF, e);
            }
            if (!b)
            {
                throw CreateException(expression, "IsTrue failed", target);
            }
        }

        /// <summary>
        /// Starts a PolyAssert. PolyAssert is used to make multiple assertions without aborting the test after the first failure. Each method on PolyAssert will store any errors that occured.
        /// When the PolyAssert is disposed (or StopIfErrorsHaveOccurred is called) then any (and ALL) errors that happened will be raised.
        /// 
        /// For example:
        /// <code>
        ///using (var poly = PAssert.Poly())
        ///{
        ///    poly.IsTrue(() => x == 5);
        ///    poly.Try(() => Assert.Fail("Wah wah"));
        ///    poly.Log("PolyAssert.Log messages are only printed if the test fails");
        ///}
        /// </code>
        /// </summary>
        public static PolyAssert Poly()
        {
            return new PolyAssert();
        }

        static Expression<Func<bool>> Substitute<TException>(Expression<Func<TException, bool>> expression, TException exception)
        {
            var parameter = expression.Parameters[0];
            var constant = Expression.Constant(exception, typeof (TException));
            var visitor = new ReplacementVisitor(parameter, constant);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<bool>>(newBody);
        }

        static Exception CreateException(LambdaExpression expression, string message, params object []parameterValues)
        {
            var output = RenderExpression(expression, parameterValues);
            return new Exception(message + ", expression was:" + CRLF + CRLF + output);
        }

        static string RenderExpression(LambdaExpression expression, params object []parameterValues)
        {
            var parser = new ExpressionParser(expression.Body, expression.Parameters.ToArray(), parameterValues);
            Node constantNode = parser.Parse();
            string[] lines = NodeFormatter.Format(constantNode);
            return string.Join(CRLF, lines) + CRLF;
        }
    }
}
