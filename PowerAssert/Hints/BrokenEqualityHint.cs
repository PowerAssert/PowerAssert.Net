using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class BrokenEqualityHint : IHint
    {
        private static readonly MethodInfo ObjectEqualsMethodInfo = typeof(object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public);

        public bool TryGetHint(Expression expression, out string hint)
        {
            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (methE.Method == ObjectEqualsMethodInfo)
                {
                    var left = ExpressionParser.DynamicInvoke(methE.Object);
                    var right = ExpressionParser.DynamicInvoke(methE.Arguments[0]);

                    if (left != null && left == right)
                    {
                        hint = ", type " + ExpressionParser.NameOfType(left.GetType()) + " has a broken equality implementation (both sides are the same object)";
                        return true;
                    }
                }
            }

            hint = null;
            return false;
        }
    }
}