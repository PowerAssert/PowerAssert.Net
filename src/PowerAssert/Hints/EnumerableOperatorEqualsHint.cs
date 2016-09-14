using System.Collections;
using System.Linq;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class EnumerableOperatorEqualsHint : OperatorEqualsHintBase
    {
        protected override bool TryGetHint(ExpressionParser parser, object left, object right, out string hint)
        {
            if (left is IEnumerable && right is IEnumerable)
            {
                if (((IEnumerable) left).Cast<object>().SequenceEqual(((IEnumerable) right).Cast<object>()))
                {
                    hint = ", but would have been True with .SequenceEqual()";
                    return true;
                }
            }

            hint = null;
            return false;
        }
    }
}