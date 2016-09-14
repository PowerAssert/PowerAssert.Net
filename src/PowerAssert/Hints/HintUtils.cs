using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;

namespace PowerAssert.Hints
{
    static class HintUtils
    {
        [CanBeNull]
        public static string GetStringDifferHint(string left, string right, StringComparer comparer)
        {
            if (left == null)
            {
                return ", left is null";
            }
            if (right == null)
            {
                return ", right is null";
            }

            for (int i = 1; i <= left.Length && i <= right.Length; ++i)
            {
                //TODO: this doesn't know about surrogate pairs or things like esszett
                if (!comparer.Equals(left.Substring(0, i), right.Substring(0, i)))
                {
                    var result = CheckCharactersAt(left, right, i - 1, comparer);
                    if (result != null)
                    {
                        return result;
                    }

                    return string.Format(", strings differ at index {0}, '{1}' != '{2}'", i - 1,
                        left[i - 1], right[i - 1]);
                }
            }

            return null; //err: they don't differ!
        }

        /// <summary>
        /// Checks for some special reasons strings might not match.
        /// </summary>
        static string CheckCharactersAt(string left, string right, int index, StringComparer comparer)
        {
            char leftC = left[index];
            char rightC = right[index];

            if (leftC == '\t' && rightC == ' ')
            {
                return string.Format(", left string contains a tab, but right string contains a space at index {0}", index);
            }

            if (leftC == ' ' && rightC == '\t')
            {
                return string.Format(", left string contains a tab, but right string contains a space at index {0}", index);
            }

            if (leftC == '\r' && rightC == '\n')
            {
                return string.Format(", left string contains a carriage-return, but right string contains a newline at index {0}", index);
            }

            if (leftC == '\n' && rightC == '\r')
            {
                return string.Format(", left string contains a newline, but right string contains a carriage-return at index {0}", index);
            }

            if (char.IsControl(leftC))
            {
                return string.Format(", left string contains control character '{0}' at index {1}", PrintableChar(leftC), index);
            }

            if (char.IsControl(rightC))
            {
                return string.Format(", right string contains control character '{0}' at index {1}", PrintableChar(rightC), index);
            }

            if (CharUnicodeInfo.GetUnicodeCategory(leftC) == UnicodeCategory.Format)
            {
                return string.Format(", left string contains format character '{0}' at index {1}", PrintableChar(leftC), index);
            }

            if (CharUnicodeInfo.GetUnicodeCategory(rightC) == UnicodeCategory.Format)
            {
                return string.Format(", right string contains format character '{0}' at index {1}", PrintableChar(rightC), index);
            }

            if (index + 1 < left.Length)
            {
                if (CheckIfIsDecomposedVersionOf(left, right, index))
                {
                    return string.Format(", left string contains a decomposed character '{1}' at index {0}", index, char.ConvertFromUtf32(char.ConvertToUtf32(right, index)));
                }
            }

            if (index + 1 < right.Length)
            {
                if (CheckIfIsDecomposedVersionOf(right, left, index))
                {
                    return string.Format(", right string contains a decomposed character '{1}' at index {0}", index, char.ConvertFromUtf32(char.ConvertToUtf32(left, index)));
                }
            }

            return null;
        }

        static bool CheckIfIsDecomposedVersionOf(string left, string right, int index)
        {
            var sb = new StringBuilder();
            sb.Append(left[index]);
            for (int i = index + (char.IsSurrogatePair(left, index) ? 2 : 1); i < left.Length; i += char.IsSurrogatePair(left, i) ? 2 : 1)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(left, i);
                if (cat == UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(char.ConvertFromUtf32(char.ConvertToUtf32(left, i)));
                }
            }

            var leftDecomposed = sb.ToString();
            var rightComposed = char.ConvertFromUtf32(char.ConvertToUtf32(right, index));

            if (leftDecomposed.Normalize() == rightComposed)
            {
                return true;
            }

            return false;
        }

        static string PrintableChar(char c)
        {
            return string.Format("U+{0:x4}", (int) c);
        }
    }
}