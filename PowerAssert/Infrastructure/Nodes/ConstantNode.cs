using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using PowerAssert.Hints;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class NameOnlyNode : Node
    {
        [NotNull]
        public string Name { get; set; }

        internal override void Walk(Node.NodeWalker walker, int depth)
        {
            walker(Name, null, depth);
        }
    }

    internal class NamedNode : Node
    {
        [NotNull]
        public string Name { get; set; }

        [CanBeNull]
        public object Value { get; set; }

        [CanBeNull]
        public Expression Expression { get; set; }

        public override IEnumerable<Alternative> EnumerateAlternatives(IHint hinter)
        {
            yield return new Alternative
            {
                Expression = Expression ?? Expression.Constant(Value)
            };
        }

        internal override void Walk(Node.NodeWalker walker, int depth)
        {
            walker(Name, FormatObject(Value), depth);
        }
    }

    internal class ConstantNode : Node
    {
        [CanBeNull]
        public object Value { get; set; }

        public override IEnumerable<Alternative> EnumerateAlternatives(IHint hinter)
        {
            yield return new Alternative {Expression = Expression.Constant(Value)};
        }

        internal override void Walk(NodeWalker walker, int depth)
        {
            walker(FormatObject(Value), null, depth);
        }
    }
}