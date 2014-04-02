using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    internal class MultiHint : IHint, IFancyHint<Expression>
    {
        private readonly IEnumerable<IHint> _hints;

        public MultiHint(params IHint[] hints) : this((IEnumerable<IHint>)hints.ToArray())
        { }

        public MultiHint(IEnumerable<IHint> hints)
        {
            _hints = hints.ToArray();
        }

        public bool TryGetHint(Expression expression, out string hint)
        {
            foreach (var hinter in _hints)
            {
                if (hinter.TryGetHint(expression, out hint))
                    return true;
            }

            hint = null;
            return false;
        }

        public IEnumerable<Alternative> GetAlternatives(Expression expression)
        {
            foreach (var hinter in _hints)
            {
                var fancy = hinter as IFancyHint;
                if (fancy == null) continue;
                foreach (var alternative in fancy.GetAlternatives(expression))
                    yield return alternative;
            }
        }
    }
}