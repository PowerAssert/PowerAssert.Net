using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    class EnumerableOperatorEqualsHint : OperatorEqualsHintBase
    {
        protected override bool TryGetHint(object left, object right, out string hint)
        {
            if (left is IEnumerable && right is IEnumerable)
            {
                if (((IEnumerable)left).Cast<object>().SequenceEqual(((IEnumerable)right).Cast<object>()))
                {
                    hint = ", but would have been True with .SequenceEqual()";
                    return true;
                }
            }

            hint = null;
            return false;
        }

        private static MethodInfo SequenceEqualMethodInfo = typeof (Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "SequenceEqual" && method.GetParameters().Length == 2);

        private static Type GetEnumerableTypeArgument(Type input)
        {
            return input.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                .Select(x => x.GetGenericArguments().First()).FirstOrDefault() ?? typeof (object);
        }

        protected override IEnumerable<Alternative> GetAlternatives(Expression left, Expression right)
        {
            if (typeof (IEnumerable).IsAssignableFrom(left.Type) &&
                typeof (IEnumerable).IsAssignableFrom(right.Type))
            {
                var innerLeft = GetEnumerableTypeArgument(left.Type);
                var innerRight = GetEnumerableTypeArgument(right.Type);

                var commonType = innerLeft == innerRight
                    ? innerLeft
                    : innerLeft.IsAssignableFrom(innerRight)
                        ? innerLeft
                        : innerRight.IsAssignableFrom(innerLeft)
                            ? innerRight
                            : typeof (object);

                yield return new Alternative
                {
                    Alteration = "Using Enumerable.SequenceEqual instead of object.Equals",
                    Expression = Expression.Call(SequenceEqualMethodInfo.MakeGenericMethod(commonType), left, right)
                };
            }
        }
    }
}