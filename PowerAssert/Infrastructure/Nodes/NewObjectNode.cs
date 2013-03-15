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
        
        [NotNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker("new " + Type + "(", Value, depth);
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

    internal class MemberInitNode : Node
    {
        [NotNull]
        public NewObjectNode Constructor { get; set; }

        [NotNull]
        public List<Node> Bindings { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Constructor.Walk(walker, depth);
            walker("{");
            foreach (var node in Bindings.Take(1))
            {
                node.Walk(walker, depth + 1);
            }
            foreach (var node in Bindings.Skip(1))
            {
                walker(", ");
                node.Walk(walker, depth + 1);
            }
            walker("}");
        }
    }

    internal class MemberAssignmentNode : Node
    {
        [NotNull]
        public string MemberName { get; set; }

        [NotNull]
        public Node Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker(MemberName);
            walker(" = ");
            Value.Walk(walker, depth);
        }
    }
}