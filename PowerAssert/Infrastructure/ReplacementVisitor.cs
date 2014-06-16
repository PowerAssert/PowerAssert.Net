using System.Linq.Expressions;

namespace PowerAssert.Infrastructure
{
    class ReplacementVisitor : ExpressionVisitor
    {
        readonly Expression _original, _replacement;

        public ReplacementVisitor(Expression original, Expression replacement)
        {
            _original = original;
            _replacement = replacement;
        }

        public override Expression Visit(Expression node)
        {
            return node == _original ? _replacement : base.Visit(node);
        }
    }
}