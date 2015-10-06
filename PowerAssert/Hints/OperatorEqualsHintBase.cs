using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    abstract class OperatorEqualsHintBase : IHint
    {
        public bool TryGetHint(ExpressionParser parser, Expression expression, out string hint)
        {
            if (expression is BinaryExpression && expression.NodeType == ExpressionType.Equal)
            {
                var be = (BinaryExpression) expression;
                object left;
                object right;
                try
                {
                    left = parser.DynamicInvoke(be.Left);
                    right = parser.DynamicInvoke(be.Right);
                }
                catch (TargetInvocationException exception)
                {
                    hint = ObjectFormatter.FormatTargetInvocationException(exception);
                    return true;
                }

                return TryGetHint(parser, left, right, out hint);
            }

            hint = null;
            return false;
        }

        protected abstract bool TryGetHint(ExpressionParser parser, object left, object right, out string hint);
    }
}