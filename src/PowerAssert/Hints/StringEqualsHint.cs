using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class StringEqualsHint : IHint
    {
        static readonly MethodInfo[] StringEqualsMethodInfo = typeof (string).GetMethods()
            .Where(x => x.Name == "Equals" && !x.IsStatic && x.GetParameters().First().ParameterType == typeof (string)).ToArray();

        static StringComparer GetComparerFromComparison(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
                default:
                    throw new InvalidDataException("Unexpected StringComparison value.");
            }
        }

        public bool TryGetHint(ExpressionParser parser, Expression expression, out string hint)
        {
            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (StringEqualsMethodInfo.Any(x => x == methE.Method))
                {
                    var obj = parser.DynamicInvoke(methE.Object);
                    var arg = parser.DynamicInvoke(methE.Arguments.First());

                    var comparison = (StringComparison) (methE.Arguments.Select(parser.DynamicInvoke)
                        .FirstOrDefault(x => x is StringComparison) ?? StringComparison.CurrentCulture);

                    hint = HintUtils.GetStringDifferHint((string) obj, (string) arg, GetComparerFromComparison(comparison));
                    return hint != null;
                }
            }

            hint = null;
            return false;
        }
    }
}