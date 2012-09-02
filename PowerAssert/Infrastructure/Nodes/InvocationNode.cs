using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class InvocationNode : Node
    {
        [CanBeNull]
        public IEnumerable<Node> Arguments { get; set; }

        [NotNull]
        public Node Expression { get; set; }

        internal override void Walk(Node.NodeWalker walker, int depth)
        {
            this.Expression.Walk(walker, depth);
            walker("(");
            foreach (var arg in this.Arguments.Take(1))
            {
                arg.Walk(walker, depth);
            }
            foreach (var arg in this.Arguments.Skip(1))
            {
                walker(", ");
                arg.Walk(walker, depth);
            }
            walker(")");
        }
    }
}
