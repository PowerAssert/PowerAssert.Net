using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class SequenceEqualHint : IHint
    {
        private static readonly MethodInfo SequenceEqualMethodInfo =
            typeof (Enumerable).GetMethods().Single(x => x.Name == "SequenceEqual" &&
                                                         x.GetParameters().Count() == 2); // TODO: equality comparer

        public bool TryGetHint(Expression expression, out string hint)
        {
            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                var typeParams = methE.Method.GetGenericArguments();
                if (typeParams.Count() == 1)
                {
                    var instantiatedMethodInfo = SequenceEqualMethodInfo.MakeGenericMethod(typeParams);
                    if (methE.Method == instantiatedMethodInfo)
                    {
                        var left = ExpressionParser.DynamicInvoke(methE.Arguments[0]);
                        var right = ExpressionParser.DynamicInvoke(methE.Arguments[1]);

                        var enumLeft = ((IEnumerable) left).GetEnumerator();
                        var enumRight = ((IEnumerable) right).GetEnumerator();
                        {
                            int i = 0;
                            while (enumLeft.MoveNext() && enumRight.MoveNext())
                            {
                                if (!Equals(enumLeft.Current, enumRight.Current))
                                {
                                    hint = string.Format(", enumerables differ at index {0}, {1} != {2}", i,
                                        ExpressionParser.FormatObject(enumLeft.Current),
                                        ExpressionParser.FormatObject(enumRight.Current));
                                    return true;
                                }
                                ++i;
                            } 
                        }
                    }
                }
            }

            hint = null;
            return false;
        }
    }
}