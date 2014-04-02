using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class NewObjectNode : Node
    {
        [NotNull]
        public List<Node> Parameters { get; private set; }

        [NotNull]
        public string Type { get; private set; }
        
        [NotNull]
        public string Value { get; private set; }

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

        public static NewObjectNode Create(NewExpression e)
        {
            return new NewObjectNode
            {
                Type = NameOfType(e.Type),
                Parameters = e.Arguments.Select(Parse).ToList(),
                Value = FormatObject(GetValue(e))
            };
        }
    }
}