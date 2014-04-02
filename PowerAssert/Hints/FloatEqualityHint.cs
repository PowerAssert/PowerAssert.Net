using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    public class FloatEqualityHint : OperatorEqualsHintBase
    {
        private const double maxProportionDifference = 0.001;

        protected override bool TryGetHint(object left, object right, out string hint)
        {
            if ((left is float || left is double) && (right is float || right is double))
            {
                var dLeft = Convert.ToDouble(left);
                var dRight = Convert.ToDouble(right);

                var diff = ProportionalDiff(dLeft, dRight);
                if (diff < maxProportionDifference)
                {
                    hint = string.Format(", but the values only differ by {0:e2}%", diff*100);
                    return true;
                }
            }

            hint = null;
            return false;
        }

        private static double ProportionalDiff(double left, double right)
        {
            return Math.Abs(left - right)/Math.Max(left, right);
        }

        private static bool FuzzyEqual(double left, double right)
        {
            return ProportionalDiff(left, right) < maxProportionDifference;
        }

        protected override IEnumerable<Alternative> GetAlternatives(Expression left, Expression right)
        {
            if ((typeof (double) == left.Type || typeof(float) == left.Type) && 
                (typeof (double) == right.Type || typeof(float) == right.Type))
            {
                yield break; //TODO!!
            }
        }
    }
}
