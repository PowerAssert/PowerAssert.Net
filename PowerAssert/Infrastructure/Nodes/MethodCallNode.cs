using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    class MethodCallNode : MemberAccessNode
    {
        internal MethodCallNode()
        {
            Parameters = new List<Node>();
        }

        [NotNull]
        public List<Node> Parameters { get; set; }

        internal override void Walk(NodeWalker walker, int depth, bool wrap)
        {
            base.Walk(walker, depth, wrap);
            if (MemberName != "get_Item")
            {
                walker("(");
            }
            foreach (var parameter in Parameters.Take(1))
            {
                parameter.Walk(walker, depth + 1, false);
            }
            foreach (var parameter in Parameters.Skip(1))
            {
                walker(", ");
                parameter.Walk(walker, depth + 1, false);
            }
            walker(MemberName == "get_Item" ? "]" : ")");
        }
    }
}