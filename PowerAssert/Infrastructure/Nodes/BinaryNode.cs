using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using PowerAssert.Hints;

namespace PowerAssert.Infrastructure.Nodes
{
    internal class BinaryNode : Node
    {
        public static BinaryNode Create(BinaryExpression e)
        {
            return e.NodeType == ExpressionType.ArrayIndex
                       ? new ArrayIndexNode { Left = Parse(e.Left), Right = Parse(e.Right), Value = GetValue(e), Type = ExpressionType.ArrayIndex }
                       : new BinaryNode
                       {
                           Value = GetValue(e),
                           Left = Parse(e.Left),
                           Right = Parse(e.Right),
                           Type = e.NodeType,
                       };
        }

        private IEnumerable<Alternative> YieldWithFancies(IHint hinter, string alteration, Expression expression)
        {
            yield return new Alternative { Alteration = alteration, Expression = expression };

            var fancy = hinter as IFancyHint<BinaryExpression>;
            if (fancy != null)
            {
                foreach (var alt in fancy.GetAlternatives(expression))
                {
                    yield return alt;
                }
            } 
 
        }

        public override IEnumerable<Alternative> EnumerateAlternatives(IHint hinter)
        {
            // this is a bit clumsy - we never want to change something on both
            // sides of the operator at once as that would be cheating :)

            var plainRight = Right.EnumerateAlternatives(hinter).First();
            var plainLeft = Left.EnumerateAlternatives(hinter).First();

            foreach (var alt in YieldWithFancies(hinter, null, Expression.MakeBinary(Type, plainLeft.Expression, plainRight.Expression)))
                yield return alt;

            foreach (var altL in Left.EnumerateAlternatives(hinter).Skip(1))
                foreach (var alt in YieldWithFancies(hinter, altL.Alteration, Expression.MakeBinary(Type, altL.Expression, plainRight.Expression)))
                    yield return alt;

            foreach (var altR in Right.EnumerateAlternatives(hinter).Skip(1))
                foreach (var alt in YieldWithFancies(hinter, altR.Alteration, Expression.MakeBinary(Type, plainLeft.Expression, altR.Expression)))
                    yield return alt;
        }

        [NotNull]
        public Node Left { get; set; }

        [NotNull]
        public Node Right { get; set; }

        [NotNull]
        public string Operator
        {
            get { return Util.BinaryOperators[Type]; }
        }

        public ExpressionType Type { get; set; }

        [CanBeNull]
        public object Value { get; set; }

        internal override void Walk(NodeWalker walker, int depth)
        {
            Left.Walk(walker, depth+1);
            walker(" ");
            walker(Operator, FormatObject(Value), depth);
            walker(" ");
            Right.Walk(walker, depth + 1);
        }
    }
}