using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class TimeSpanTotalMistakesHint : IHint
    {
        static readonly MethodInfo[] EqualsMethodInfos =
        {
            typeof (object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public),
            typeof (int).GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Single(x => x.Name == "Equals" && x.GetParameters().First().ParameterType == typeof (int)),
        };

        static readonly MethodInfo[] StaticEqualsMethodInfos =
        {
            typeof (object).GetMethod("Equals", BindingFlags.Static | BindingFlags.Public),
        };

        public bool TryGetHint(ExpressionParser parser, Expression expression, out string hint)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression != null && binaryExpression.NodeType == ExpressionType.Equal)
            {
                var left = binaryExpression.Left as MemberExpression;
                if (left != null)
                {
                    if (CheckValues(parser, out hint, left, binaryExpression.Right))
                    {
                        return true;
                    }
                }

                var right = binaryExpression.Right as MemberExpression;
                if (right != null)
                {
                    if (CheckValues(parser, out hint, right, binaryExpression.Left))
                    {
                        return true;
                    }
                }
            }

            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (EqualsMethodInfos.Any(x => x == methE.Method))
                {
                    var objectToCheck = methE.Object as MemberExpression;
                    if (objectToCheck != null)
                    {
                        if (CheckValues(parser, out hint, objectToCheck, methE.Arguments.Single()))
                        {
                            return true;
                        }
                    }

                    var argToCheck = methE.Arguments.Single() as MemberExpression;
                    if (argToCheck != null)
                    {
                        if (CheckValues(parser, out hint, argToCheck, methE.Object))
                        {
                            return true;
                        }
                    }
                }
                else if (StaticEqualsMethodInfos.Any(x => x == methE.Method))
                {
                    // this gets a little hairy as there is a conversion to object before the Equals method is called

                    var leftOuter = methE.Arguments[0] as UnaryExpression;
                    if (leftOuter != null && leftOuter.NodeType == ExpressionType.Convert)
                    {
                        var leftInner = leftOuter.Operand as MemberExpression;
                        if (leftInner != null)
                        {
                            if (CheckValues(parser, out hint, leftInner, methE.Arguments[1]))
                            {
                                return true;
                            }
                        }
                    }

                    var rightOuter = methE.Arguments[1] as UnaryExpression;
                    if (rightOuter != null && rightOuter.NodeType == ExpressionType.Convert)
                    {
                        var rightInner = rightOuter.Operand as MemberExpression;
                        if (rightInner != null)
                        {
                            if (CheckValues(parser, out hint, rightInner, methE.Arguments[0]))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            hint = null;
            return false;
        }

        bool CheckValues(ExpressionParser parser, out string hint, MemberExpression left, Expression rightEx)
        {
            var totalValue = TryToGetTotalValue(parser, left);
            if (totalValue.HasValue)
            {
                var right = parser.DynamicInvoke(rightEx);
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

        long? TryToGetTotalValue(ExpressionParser parser, MemberExpression expresssion)
        {
            if (expresssion.Member.DeclaringType == typeof (TimeSpan))
            {
                var totalVersion = typeof (TimeSpan).GetProperty("Total" + expresssion.Member.Name);
                if (totalVersion != null)
                {
                    var owner = parser.DynamicInvoke(expresssion.Expression);

                    return Convert.ToInt64(totalVersion.GetValue(owner, null));
                }
            }

            return null;
        }
    }
}