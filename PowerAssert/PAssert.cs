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
            var exception = AssertionResultToException(TestExpression(expression));
            if (exception != null)
                throw exception;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void IsTrue<T>(T target, Expression<Func<T, bool>> expression)
        {
            var exception = AssertionResultToException(TestExpression(expression, target));
            if (exception != null)
                throw exception;
        }

        static Expression<Func<bool>> Substitute<TException>(Expression<Func<TException, bool>> expression, TException exception)
        {
            var parameter = expression.Parameters[0];
            var constant = Expression.Constant(exception, typeof (TException));
            var visitor = new ReplacementVisitor(parameter, constant);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<bool>>(newBody);
        }

        static AssertionResult TestExpression(LambdaExpression expression, params object[] args)
        {
            var f = expression.Compile();
            try
            {
                if ((bool)f.DynamicInvoke(args))
                    return AssertionResult.Success;
            }
            catch (TargetInvocationException e)
            {
                return AssertionResult.Failure(RenderExpression(expression, args), e.InnerException);
            }
            return AssertionResult.Failure(RenderExpression(expression, args), null);
        }

        private static Exception AssertionResultToException(AssertionResult ret)
        {
            if (ret.Succeeded)
            {
                return null;
            }
            else if (ret.Exception != null)
            {
                return new Exception(string.Format("IsTrue encountered {0}, expression was:{2}{1}{2}",
                                                  ret.Exception.GetType().Name, ret.Output, CRLF + CRLF),
                                     ret.Exception);
            }
            else
            {
                return new Exception("IsTrue failed, expression was:" + CRLF + CRLF + ret.Output);
            }
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
