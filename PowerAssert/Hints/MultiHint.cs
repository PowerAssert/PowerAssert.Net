using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PowerAssert.Infrastructure;

namespace PowerAssert.Hints
{
    class MultiHint : IHint
    {
        readonly IEnumerable<IHint> _hints;

        public MultiHint(params IHint[] hints) : this((IEnumerable<IHint>) hints.ToArray())
        {
        }

        public MultiHint(IEnumerable<IHint> hints)
        {
            _hints = hints.ToArray();
        }

        public bool TryGetHint(ExpressionParser parser, Expression expression, out string hint)
        {
            foreach (var hinter in _hints)
            {
                if (hinter.TryGetHint(parser, expression, out hint))
                {
                    return true;
                }
            }

            hint = null;
            return false;
        }
    }
}