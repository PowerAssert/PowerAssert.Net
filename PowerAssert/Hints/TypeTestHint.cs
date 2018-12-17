using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class TypeTestHint : IHint
    {
        public bool TryGetHint(ExpressionParser parser, Expression expression, out string hint)
        {
            if (expression is TypeBinaryExpression typeBinaryExp && typeBinaryExp.NodeType == ExpressionType.TypeIs)
            {
                var expressionOperand = parser.DynamicInvoke(typeBinaryExp.Expression);
                var typeOperand = typeBinaryExp.TypeOperand;

                if (!typeOperand.IsInstanceOfType(expressionOperand))
                {
                    hint = $", {expressionOperand} is not assignable to {typeOperand}";
                    return true;
                }
            }

            hint = null;
            return false;
        }
    }
}
