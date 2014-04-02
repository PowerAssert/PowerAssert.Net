using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    public abstract class OperatorEqualsHintBase : IHint, IFancyHint<BinaryExpression>
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
                    left = Node.DynamicInvoke(be.Left);
                    right = Node.DynamicInvoke(be.Right);
                }
                catch (TargetInvocationException exception)
                {
                    hint = Node.FormatTargetInvocationException(exception);
                    return true;
                }

                return TryGetHint(left, right, out hint);
            }

            hint = null;
            return false;
        }

        protected abstract bool TryGetHint(object left, object right, out string hint);
        protected abstract IEnumerable<Alternative> GetAlternatives(Expression left, Expression right);

        public IEnumerable<Alternative> GetAlternatives(Expression expression)
        {
            var exp = expression as BinaryExpression;
            if (exp != null) return GetAlternatives(exp);
            return Enumerable.Empty<Alternative>();
        }

        public IEnumerable<Alternative> GetAlternatives(BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Equal)
            {
                foreach (var alt in GetAlternatives(expression.Left, expression.Right))
                {
                    yield return alt;
                }
            }
        }
    }
}