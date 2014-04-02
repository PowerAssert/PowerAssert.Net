using System;
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
        public object MemberValue { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Container.Walk(walker, depth + 1);
            if (MemberName == "get_Item")
            {
                walker("[", FormatObject(MemberValue), depth);
            }
            else
            {
                walker(".");
                walker(MemberName, FormatObject(MemberValue), depth);
            }
        }
    }
}