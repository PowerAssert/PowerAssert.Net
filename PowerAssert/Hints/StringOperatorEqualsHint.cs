using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    public class StringOperatorEqualsHint : OperatorEqualsHintBase
    {
        protected override bool TryGetHint(object left, object right, out string hint)
        {
            if (left is string && right is string)
            {
                if (((string)left).Equals((string)right, StringComparison.OrdinalIgnoreCase)) // TODO: think about ordinal vs culture-invariant here...
                {
                    hint = ", but would have been True if case-insensitive";
                    return true;
                }

                hint = HintUtils.GetStringDifferHint((string)left, (string)right, StringComparer.CurrentCulture);
                return hint != null;
            }

            hint = null;
            return false;
        }

        private static readonly MethodInfo StringEqualsMethodInfo = typeof (string)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Single(method => method.Name == "Equals" && method.GetParameters().Length == 3);

        protected override IEnumerable<Alternative> GetAlternatives(Expression left, Expression right)
        {
            if (left.Type == typeof (string) && right.Type == typeof (string))
            {
                yield return new Alternative
                {
                    Alteration = "Using case-insensitive string comparison",
                    Expression = Expression.Call(StringEqualsMethodInfo, left, right, Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                };
            }
        }
    }
}