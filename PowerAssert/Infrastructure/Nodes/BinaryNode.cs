using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace PowerAssert.Infrastructure.Nodes
{
    class BinaryNode : Node
    {
        public static readonly Dictionary<string, int> PriorityTable
            = new[] { "* / %", "+ -", "<< >>", "< > <= >= is as", "== !=", "&", "^", "|", "&&", "||", "??", "=>" }
                .Select((text, index) => text.Split(' ').Select(ope => new {ope, index}))
                .SelectMany(x => x)
                .ToDictionary(kv => kv.ope, kv => kv.index + 1);

        [NotNull]
        public Node Left { get; set; }

        [NotNull]
        public Node Right { get; set; }

        [NotNull]
        public string Operator { get; set; }

        [CanBeNull]
        public string Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth, bool wrap)
        {
            if (wrap)
            {
                walker("(", Value, depth);
                depth += 1;
            }
            Left.Walk(walker, depth + 1, Priority < Left.Priority);
            walker(" ");
            walker(Operator, Value, depth);
            walker(" ");
            Right.Walk(walker, depth + 1, Priority <= Right.Priority);
            if (wrap)
                walker(")");
        }

        public override int Priority
        {
            get { return PriorityTable[Operator]; }
        }
    }
}