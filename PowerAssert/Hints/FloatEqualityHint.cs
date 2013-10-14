using System;

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

                var diff = Math.Abs(dLeft - dRight)/Math.Max(dLeft, dRight);
                if (diff < maxProportionDifference)
                {
                    hint = string.Format(", but the values only differ by {0:e2}%", diff*100);
                    return true;
                }
            }

            hint = null;
            return false;
        }

    }
}
