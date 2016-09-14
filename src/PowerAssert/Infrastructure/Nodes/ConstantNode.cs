using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    class ConstantNode : Node
    {
        [NotNull]
        public string Text { get; set; }

        [CanBeNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth, bool wrap)
        {
            walker(Text, Value, depth);
        }
    }
}