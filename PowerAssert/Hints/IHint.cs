using System.Linq.Expressions;

namespace PowerAssert.Hints
{
    interface IHint
    {
        bool TryGetHint(Expression expression, out string hint);
    }
}