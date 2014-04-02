using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class InvocationNode : Node
    {
        [NotNull]
        public IEnumerable<Node> Arguments { get; private set; }

        [NotNull]
        public Node Expression { get; private set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Expression.Walk(walker, depth);
            walker("(");
            foreach (var arg in Arguments.Take(1))
            {
                arg.Walk(walker, depth + 1);
            }
            foreach (var arg in Arguments.Skip(1))
            {
                walker(", ");
                arg.Walk(walker, depth + 1);
            }
            walker(")");
        }

        public static Node Create(InvocationExpression e)
        {
            return new InvocationNode
            {
                Arguments = e.Arguments.Select(Parse),
                Expression = Parse(e.Expression)
            };
        }
    }
}
