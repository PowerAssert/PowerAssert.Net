using System;

namespace PowerAssert.Hints
{
    class StringOperatorEqualsHint : OperatorEqualsHintBase
    {
        protected override bool TryGetHint(object left, object right, out string hint)
        {
            if (left is string && right is string)
            {
                if (((string)left).Equals((string)right, StringComparison.OrdinalIgnoreCase)) // TODO: think about ordinal vs culture-invariant here...
                {
                    hint = ", but would have been True if case-insensitive";
                    return true;
                }

                hint = HintUtils.GetStringDifferHint((string)left, (string)right, StringComparer.CurrentCulture);
                return hint != null;
            }

            hint = null;
            return false;
        }
    }
}