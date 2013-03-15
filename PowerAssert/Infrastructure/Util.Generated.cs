using System.Linq.Expressions;
using System.Collections.Generic;

namespace PowerAssert.Infrastructure
{
    internal class Util
    {
        internal static Dictionary<ExpressionType, string> BinaryOperators = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.AndAlso, "&&"},
            {ExpressionType.OrElse, "||"},
            {ExpressionType.Add, "+"},
            {ExpressionType.AddChecked, "+"},
            {ExpressionType.And, "&"},
            {ExpressionType.Divide, "/"},
            {ExpressionType.Equal, "=="},
            {ExpressionType.ExclusiveOr, "^"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LeftShift, "<<"},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
            {ExpressionType.Modulo, "%"},
            {ExpressionType.Multiply, "*"},
            {ExpressionType.MultiplyChecked, "*"},
            {ExpressionType.NotEqual, "!="},
            {ExpressionType.Or, "|"},
            {ExpressionType.Power, "^"},
            {ExpressionType.RightShift, ">>"},
            {ExpressionType.Subtract, "-"},
            {ExpressionType.SubtractChecked, "-"},
            {ExpressionType.Assign, "="},
            {ExpressionType.AddAssign, "+="},
            {ExpressionType.AndAssign, "&="},
            {ExpressionType.DivideAssign, "/="},
            {ExpressionType.ExclusiveOrAssign, "^="},
            {ExpressionType.LeftShiftAssign, "<<="},
            {ExpressionType.ModuloAssign, "%="},
            {ExpressionType.MultiplyAssign, "*="},
            {ExpressionType.OrAssign, "|="},
            {ExpressionType.PowerAssign, "**="},
            {ExpressionType.RightShiftAssign, ">>="},
            {ExpressionType.SubtractAssign, "-="},
            {ExpressionType.AddAssignChecked, "+="},
            {ExpressionType.MultiplyAssignChecked, "*="},
            {ExpressionType.SubtractAssignChecked, "-="},
        };
    }
}

