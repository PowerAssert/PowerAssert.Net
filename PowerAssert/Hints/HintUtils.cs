using System;

namespace PowerAssert.Hints
{
    static class HintUtils
    {
        public static string GetStringDifferHint(string left, string right, StringComparer comparer)
        {
            if (left == null) return ", left is null";
            if (right == null) return ", right is null";

            for (int i = 1; i <= left.Length && i <= right.Length; ++i)
            {
                //TODO: this doesn't know about surrogate pairs or things like esszett
                if (!comparer.Equals(left.Substring(0, i), right.Substring(0, i)))
                    return string.Format(", strings differ at index {0}, '{1}' != '{2}'", i - 1,
                        left[i - 1], right[i - 1]);
            }

            return ""; //err: they don't differ!
        }
    }
}