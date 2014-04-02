using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    class MethodEqualsInsteadOfOperatorEqualsHint : OperatorEqualsHintBase
    {
        protected override bool TryGetHint(object left, object right, out string hint)
        {
            if (Equals(left, right))
            {
                hint = ", but would have been True with Equals()";
                return true;
            }

            hint = null;
            return false;
        }

        private static MethodInfo EqualsMethodInfo = typeof (object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public);

        protected override IEnumerable<Alternative> GetAlternatives(Expression left, Expression right)
        {
            if (Equals(left, right))
            {
                yield return new Alternative
                {
                    Alteration = "Using .Equals() instead of ==",
                    Expression = Expression.Call(EqualsMethodInfo, left, right),
                };
            }
        }
    }
}