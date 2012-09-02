using System.Linq.Expressions;
using System.Collections.Generic;

namespace PowerAssert.Infrastructure
{
    internal class Util
    {
        internal static Dictionary<ExpressionType, string> Operators = new Dictionary<ExpressionType, string>
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
            {ExpressionType.RightShift, ">>"},
            {ExpressionType.Subtract, "-"},
            {ExpressionType.SubtractChecked, "-"},
        };
    }
}