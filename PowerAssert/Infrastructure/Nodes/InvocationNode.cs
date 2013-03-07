using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class InvocationNode : Node
    {
        [NotNull]
        public IEnumerable<Node> Arguments { get; set; }

        [NotNull]
        public Node Expression { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Expression.Walk(walker, depth);
            walker("(");
            foreach (var arg in Arguments.Take(1))
            {
                arg.Walk(walker, depth);
            }
            foreach (var arg in Arguments.Skip(1))
            {
                walker(", ");
                arg.Walk(walker, depth);
            }
            walker(")");
        }
    }
}
