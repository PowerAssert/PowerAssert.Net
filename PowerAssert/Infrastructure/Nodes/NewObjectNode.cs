using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class NewObjectNode : Node
    {
        [NotNull]
        public List<Node> Parameters { get; set; }

        [NotNull]
        public string Type { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker("new " + Type + "(");
            foreach (var node in Parameters.Take(1))
            {
                node.Walk(walker, depth);
            }
            foreach (var node in Parameters.Skip(1))
            {
                walker(", ");
                node.Walk(walker, depth);
            }
            walker(")");
        }
    }
}