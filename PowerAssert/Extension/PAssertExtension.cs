using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PowerAssert.Extension
{
    public static class PAssertExtension
    {
        public static void PAssert<T>(this T target, Expression<Func<T, bool>> expression)
        {
            PowerAssert.PAssert.IsTrue(target, expression);
        }

        public static void PAssertAll<T>(this T target, params Expression<Func<T, bool>>[] expressions)
        {
            PowerAssert.PAssert.AreTrue(target, expressions);
        }
    }
}
