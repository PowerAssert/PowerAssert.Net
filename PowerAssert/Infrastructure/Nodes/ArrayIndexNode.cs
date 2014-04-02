using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class ArrayIndexNode : BinaryNode
    {
        internal override void Walk(NodeWalker walker, int depth)
        {
            Left.Walk(walker, depth + 1);
            walker("[", FormatObject(Value), depth);
            Right.Walk(walker, depth + 1);
            walker("]");
        }
    }
}