using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class DelegateShouldHaveBeenInvokedEqualsHint : IHint
    {
        static readonly MethodInfo ObjectEqualsMethodInfo = typeof (object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

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
                        if (CheckArgument(methE, true, out hint))
                        {
                            return true;
                        }

                        if (CheckArgument(methE, false, out hint))
                        {
                            return true;
                        }

                        hint = ", this is a suspicious comparison";
                        return true;
                    }
                }
            }

            hint = null;
            return false;
        }

        static bool CheckArgument(MethodCallExpression methE, bool left, out string hint)
        {
            int ix1 = left ? 0 : 1;
            int ix2 = left ? 1 : 0;

            if (typeof (Delegate).IsAssignableFrom(methE.Arguments[ix1].Type))
            {
                object leftR;
                try
                {
                    leftR = ExpressionParser.DynamicInvoke(Expression.Invoke(methE.Arguments[ix1]));
                }
                catch (InvalidOperationException) // delegate needs arguments
                {
                    hint = null;
                    return false;
                }

                if (Equals(leftR, ExpressionParser.DynamicInvoke(methE.Arguments[ix2])))
                {
                    hint = string.Format(", but would have been True if you had invoked '{0}'",
                        NodeFormatter.PrettyPrint(ExpressionParser.Parse(methE.Arguments[ix1])));
                    return true;
                }
            }

            hint = null;
            return false;
        }
    }
}