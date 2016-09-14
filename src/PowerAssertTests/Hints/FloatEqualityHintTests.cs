using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    public class FloatEqualityHintTests
    {
        [Test]
        public void TestFloatDoubleComparison()
        {
            double d = 0.1;
            float f = (float) d*100;
            f /= 100;

            var floatHint = new FloatEqualityHint();

            Expression<Func<bool>> exp = () => d == f;
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsTrue(floatHint.TryGetHint(p, exp.Body, out description));
            Assert.IsNotNull(description);
        }

        [Test]
        public void TestDoubleDoubleComparison()
        {
            double d = 0.1;
            double f = (float) d*100;
            f /= 100;

            var floatHint = new FloatEqualityHint();

            Expression<Func<bool>> exp = () => d == f;
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsTrue(floatHint.TryGetHint(p, exp.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void TestFloatFloatComparison()
        {
            float d = 0.1f;
            float f = d*100;
            f /= 100;

            var floatHint = new FloatEqualityHint();

            Expression<Func<bool>> exp = () => d == f;
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsTrue(floatHint.TryGetHint(p, exp.Body, out description));
            Assert.IsNotNull(description);
        }
    }
}