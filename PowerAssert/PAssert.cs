using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert
{
    public static class PAssert
    {
        public static TException Throws<TException>(Action a) where TException : Exception
        {
            try
            {
                a();
            }
            catch (TException exception)
            {
                return exception;
            }

            throw new Exception("An exception of type " + typeof(TException).Name + " was expected, but no exception occured");
        }

        public static void IsTrue(Expression<Func<bool>> expression)
        {
            Func<bool> func = expression.Compile();
            if (!func())
            {
                var method = new StackFrame(1, false).GetMethod();
                _currentTestClass = method != null ? method.DeclaringType : null;
                throw CreateException(expression, "IsTrue failed");
            }
        }

        static Exception CreateException(Expression<Func<bool>> expression, string message)
        {
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] lines = NodeFormatter.Format(constantNode);
            string nl = Environment.NewLine;
            return new Exception(message + ", expression was:" + nl + nl + String.Join(nl, lines));
        }

        [ThreadStatic]
        private static Type _currentTestClass;

        internal static Type CurrentTestClass
        {
            get { return _currentTestClass; }
        }
    }
}
