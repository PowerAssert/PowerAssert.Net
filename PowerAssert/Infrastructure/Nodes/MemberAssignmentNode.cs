using System.Linq.Expressions;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
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

        public static Node Create(MemberAssignment e)
        { 
            return new MemberAssignmentNode { MemberName = e.Member.Name, Value = Parse(e.Expression) };
        }
    }
}