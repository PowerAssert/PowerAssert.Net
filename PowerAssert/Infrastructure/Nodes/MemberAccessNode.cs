using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class MemberAccessNode : Node
    {
        [NotNull]
        public Node Container { get; set; }

        [NotNull]
        public string MemberName { get; set; }

        [NotNull]
        public string MemberValue { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Container.Walk(walker, depth);
            if (MemberName == "get_Item")
            {
                walker("[", MemberValue, depth + 1);
            }
            else
            {
                walker(".");
                walker(MemberName, MemberValue, depth + 1);
            }
        }
    }
}