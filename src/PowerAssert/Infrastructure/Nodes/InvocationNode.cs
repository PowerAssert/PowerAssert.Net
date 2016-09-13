using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    class InvocationNode : Node
    {
        [NotNull]
        public IEnumerable<Node> Arguments { get; set; }

        [NotNull]
        public Node Expression { get; set; }

        internal override void Walk(NodeWalker walker, int depth, bool wrap)
        {
            Expression.Walk(walker, depth, Priority < Expression.Priority);
            walker("(");
            foreach (var arg in Arguments.Take(1))
            {
                arg.Walk(walker, depth + 1, false);
            }
            foreach (var arg in Arguments.Skip(1))
            {
                walker(", ");
                arg.Walk(walker, depth + 1, false);
            }
            walker(")");
        }
    }
}