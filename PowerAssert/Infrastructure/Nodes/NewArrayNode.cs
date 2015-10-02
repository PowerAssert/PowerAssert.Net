using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    class NewArrayNode : Node
    {
        [NotNull]
        public List<Node> Items { get; set; }

        [NotNull]
        public string Type { get; set; }

        internal override void Walk(NodeWalker walker, int depth, bool wrap)
        {
            walker("new " + Type + "[]{");
            foreach (var node in Items.Take(1))
            {
                node.Walk(walker, depth, false);
            }
            foreach (var node in Items.Skip(1))
            {
                walker(", ");
                node.Walk(walker, depth, false);
            }
            walker("}");
        }
    }
}