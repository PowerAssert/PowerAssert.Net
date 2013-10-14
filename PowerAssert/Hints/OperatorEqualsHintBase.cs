using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    public abstract class OperatorEqualsHintBase : IHint
    {
        public bool TryGetHint(Expression expression, out string hint)
        {
            if (expression is BinaryExpression && expression.NodeType == ExpressionType.Equal)
            {
                var be = (BinaryExpression) expression;
                object left;
                object right;
                try
                {
                    left = ExpressionParser.DynamicInvoke(be.Left);
                    right = ExpressionParser.DynamicInvoke(be.Right);
                }
                catch (TargetInvocationException exception)
                {
                    hint = ExpressionParser.FormatTargetInvocationException(exception);
                    return true;
                }

                return TryGetHint(left, right, out hint);
            }

            hint = null;
            return false;
        }

        protected abstract bool TryGetHint(object left, object right, out string hint);
    }
}