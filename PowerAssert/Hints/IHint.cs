using System.Collections.Generic;
using System.Linq.Expressions;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    public interface IHint
    {
        bool TryGetHint(Expression expression, out string hint);
    }

    public interface IFancyHint
    {
        IEnumerable<Alternative> GetAlternatives(Expression expression);
    }

    public interface IFancyHint<in T> : IFancyHint
        where T : Expression
    {
        IEnumerable<Alternative> GetAlternatives(T expression);
    }
}