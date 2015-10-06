using System.Linq.Expressions;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    interface IHint
    {
        bool TryGetHint(ExpressionParser parser, Expression expression, out string hint);
    }
}