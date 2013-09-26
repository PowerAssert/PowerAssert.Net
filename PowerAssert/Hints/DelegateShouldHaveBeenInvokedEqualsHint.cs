using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class DelegateShouldHaveBeenInvokedEqualsHint : IHint
    {
        private static readonly MethodInfo ObjectEqualsMethodInfo = typeof (object).GetMethod("Equals", BindingFlags.Static|BindingFlags.Public);

        public bool TryGetHint(Expression expression, out string hint)
        {
            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (methE.Method == ObjectEqualsMethodInfo)
                {
                    var left = ExpressionParser.DynamicInvoke(methE.Arguments[0]);
                    var right = ExpressionParser.DynamicInvoke(methE.Arguments[1]);

                    if (left is Delegate || right is Delegate)
                    {
                        var del = left is Delegate ? (Delegate) left : (Delegate) right;
                        var other = left is Delegate ? right : left;

                        if (!del.Method.GetParameters().Any() && Equals(del.Method.Invoke(null, new object[0]), other))
                        {
                            var delExp = left is Delegate ? methE.Arguments[0] : methE.Arguments[1];

                            hint = ", but would have been True if you had invoked '" + NodeFormatter.PrettyPrint(ExpressionParser.Parse(delExp)) + "'";
                            return true;
                        }
                        else
                        {
                            hint = ", this is a suspicious comparison";
                            return true;
                        }
                    }
                }
            }

            hint = null;
            return false;
        }
    }
}