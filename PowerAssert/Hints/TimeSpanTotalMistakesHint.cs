using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class TimeSpanTotalMistakesHint : IHint   
    {
        public bool TryGetHint(Expression expression, out string hint)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null)
            {
                if (binaryExpression.NodeType == ExpressionType.Equal)
                {
                    var left = binaryExpression.Left as MemberExpression;
                    if (left != null)
                    {
                        if (CheckValues(out hint, left, binaryExpression.Right)) return true;
                    }

                    var right = binaryExpression.Right as MemberExpression;
                    if (right != null)
                    {
                        if (CheckValues(out hint, right, binaryExpression.Left)) return true;
                    }
                }
            }

            hint = null;
            return false;
        }

        private bool CheckValues(out string hint, MemberExpression left, Expression rightEx)
        {
            var totalValue = TryToGetTotalValue(left);
            if (totalValue.HasValue)
            {
                var right = ExpressionParser.DynamicInvoke(rightEx);
                var rightValue = Convert.ToInt64(right);

                if (totalValue.Value == rightValue)
                {
                    hint = string.Format(", but would have been True if you had used Total{0} instead of {0}", left.Member.Name);
                    return true;
                }
            }

            hint = null;
            return false;
        }

        long? TryToGetTotalValue(MemberExpression expresssion)
        {
            if (expresssion.Member.DeclaringType == typeof (TimeSpan))
            {
                var totalVersion = typeof(TimeSpan).GetProperty("Total" + expresssion.Member.Name);
                if (totalVersion != null)
                {
                    var owner = ExpressionParser.DynamicInvoke(expresssion.Expression);

                    return Convert.ToInt64(totalVersion.GetValue(owner, null));
                }
            }

            return null;
        }
    }
}
