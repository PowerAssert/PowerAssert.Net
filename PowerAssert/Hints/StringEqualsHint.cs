using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    internal class StringEqualsHint : IHint
    {
        private static readonly MethodInfo[] StringEqualsMethodInfo = typeof (string).GetMethods()
            .Where(x => x.Name == "Equals" && !x.IsStatic && x.GetParameters().First().ParameterType == typeof(string)).ToArray();

        private static StringComparer GetComparerFromComparison(StringComparison comparison)
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
                default: throw new InvalidDataException("Unexpected StringComparison value.");
            }
        }
        public bool TryGetHint(Expression expression, out string hint)
        {
            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (StringEqualsMethodInfo.Any(x => x == methE.Method))
                {
                    var obj = Node.DynamicInvoke(methE.Object);
                    var arg = Node.DynamicInvoke(methE.Arguments.First());

                    var comparison = (StringComparison) (methE.Arguments.Select(Node.DynamicInvoke)
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