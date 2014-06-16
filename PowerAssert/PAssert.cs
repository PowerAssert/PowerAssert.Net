using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert
{
    public static class PAssert
    {
        [ThreadStatic]
        internal static Type CurrentTestClass;
        static readonly Assembly MyAssembly = typeof(PAssert).Assembly;


        public static TException Throws<TException>(Action a, Expression<Func<TException, bool>> exceptionAssertion = null) where TException : Exception
        {
            try
            {
                a();
            }
            catch (TException exception)
            {
                if (exceptionAssertion != null)
                {
                    IsTrue(Substitute(exceptionAssertion, exception));
                }
                return exception;
            }

            throw new Exception("An exception of type " + typeof(TException).Name + " was expected, but no exception occured");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void IsTrue(Expression<Func<bool>> expression)
        {
            Func<bool> func = expression.Compile();
            if (!func())
            {
                CurrentTestClass = new StackTrace(1, false).GetFrames().Select(x => x.GetMethod().DeclaringType).Where(x => x != null).First(x => x.Assembly != MyAssembly);
                throw CreateException(expression, "IsTrue failed");
            }
        }

        static Expression<Func<bool>> Substitute<TException>(Expression<Func<TException, bool>> expression, TException exception)
        {
            var parameter = expression.Parameters[0];
            var constant = Expression.Constant(exception, typeof(TException));
            var visitor = new ReplacementVisitor(parameter, constant);
            var newBody = visitor.Visit(expression.Body);
            return Expression.Lambda<Func<bool>>(newBody);
        }

        static Exception CreateException(Expression<Func<bool>> expression, string message)
        {
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] lines = NodeFormatter.Format(constantNode);
            string nl = Environment.NewLine;
            return new Exception(message + ", expression was:" + nl + nl + String.Join(nl, lines));
        }
    }
}
