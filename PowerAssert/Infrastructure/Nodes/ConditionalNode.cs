using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class ConditionalNode : Node
    {
        [NotNull]
        public Node Condition { get; set; }

        [NotNull]
        public Node TrueNode { get; set; }

        [NotNull]
        public Node FalseNode { get; set; }

        public bool TestValue { get; set; }
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker("(", TestValue ? TrueValue : FalseValue, depth);
            Condition.Walk(walker, depth + 1);
            walker(" ? ");
            TrueNode.Walk(walker, depth + 1);
            walker(" : ");
            FalseNode.Walk(walker, depth + 1);
            walker(")");
        }
    }
}