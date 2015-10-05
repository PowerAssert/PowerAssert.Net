using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    class ArrayIndexNode : Node
    {
        [NotNull]
        public Node Array { get; set; }

        [NotNull]
        public Node Index { get; set; }

        [NotNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth, bool wrap)
        {
            Array.Walk(walker, depth + 1, Priority < Array.Priority);
            walker("[", Value, depth);
            Index.Walk(walker, depth + 1, false);
            walker("]");
        }
    }
}