using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    internal class EnumerableEqualsHint : IHint
    {
        private static readonly MethodInfo ObjectInstanceEqualsMethodInfo = typeof (object).GetMethods().Single(x => x.Name == "Equals" && !x.IsStatic);
        public bool TryGetHint(Expression expression, out string hint)
        {
            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (methE.Method == ObjectInstanceEqualsMethodInfo)
                {
                    var obj = ExpressionParser.DynamicInvoke(methE.Object) as IEnumerable;
                    var arg = ExpressionParser.DynamicInvoke(methE.Arguments.First()) as IEnumerable;
                    if (obj != null && arg != null)
                    {
                        if (obj.Cast<object>().SequenceEqual(arg.Cast<object>()))
                        {
                            hint = ", but would have been True with .SequenceEqual()";
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
