using System;
using System.Linq;
using System.Linq.Expressions;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    /// <summary>
    /// Because enums get erased into ints by the compiler, this helps show what the original values were
    /// </summary>
    class EnumComparisonShowValuesHint : IHint
    {
        static readonly Type[] EnumErasedTypes = new[]
        {
            typeof (byte),
            typeof (sbyte),
            typeof (short),
            typeof (ushort),
            typeof (int),
            typeof (uint),
            typeof (long),
            typeof (ulong),
        };

        public bool TryGetHint(ExpressionParser parser, Expression expression, out string hint)
        {
            var binE = expression as BinaryExpression;

            if (binE != null && binE.NodeType == ExpressionType.Equal)
            {
                var lr = Check(parser, binE.Left, binE.Right);
                const string format = ", {0} != {1}";
                if (lr != null)
                {
                    hint = string.Format(format, lr.Item1, lr.Item2);
                    return true;
                }
                var rl = Check(parser, binE.Right, binE.Left);
                if (rl != null)
                {
                    hint = string.Format(format, rl.Item2, rl.Item1);
                    return true;
                }
            }
            hint = null;
            return false;
        }

        Tuple<string, string> Check(ExpressionParser parser, Expression x, Expression y)
        {
            if (!EnumErasedTypes.Contains(x.Type))
            {
                return null;
            }

            var unE = x as UnaryExpression;

            if (unE != null && unE.NodeType == ExpressionType.Convert)
            {
                if (ReflectionShim.IsEnum(unE.Operand.Type))
                {
                    return Tuple.Create(Parse(parser, unE.Operand.Type, x), Parse(parser, unE.Operand.Type, y));
                }
            }
            return null;
        }

        string Parse(ExpressionParser parser, Type enumType, Expression expression)
        {
            return enumType.Name + "." + Enum.GetName(enumType, parser.DynamicInvoke(expression));
        }
    }
}