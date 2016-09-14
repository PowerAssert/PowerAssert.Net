using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

namespace PowerAssertTests.Hints
{
    // ReSharper disable EqualExpressionComparison

    [TestFixture]
    class DelegateShouldHaveBeenInvokedEqualsHintTests
    {
        [Test]
        public void ShouldBeTriggeredWithoutClosure_Left()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            Func<int> f = () => 3;

            Expression<Func<bool>> ex = () => Equals(f, 3);

            var p = new ExpressionParser(ex.Body);
            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, ex.Body, out ignored));
            Assert.IsNotNull(ignored);
        }

        [Test]
        public void ShouldBeTriggeredWithClosure_Left()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            int n = 3;
            Func<int> f = () => n; // now this func requires a closure

            Expression<Func<bool>> ex = () => Equals(f, 3);
            var p = new ExpressionParser(ex.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, ex.Body, out ignored));
            Assert.IsNotNull(ignored);
        }


        [Test]
        public void ShouldBeTriggeredWithoutClosure_Right()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            Func<int> f = () => 3;

            Expression<Func<bool>> ex = () => Equals(3, f);
            var p = new ExpressionParser(ex.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, ex.Body, out ignored));
            Assert.IsNotNull(ignored);
        }

        [Test]
        public void ShouldBeTriggeredWithClosure_Right()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            int n = 3;
            Func<int> f = () => n; // now this func requires a closure

            Expression<Func<bool>> ex = () => Equals(3, f);
            var p = new ExpressionParser(ex.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, ex.Body, out ignored));
            Assert.IsNotNull(ignored);
        }

        [Test]
        public void ShouldNotBeTriggeredIfBothAreDelegates()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            Func<int> f = () => 3;

            Expression<Func<bool>> ex = () => Equals(f, f);
            var p = new ExpressionParser(ex.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, ex.Body, out ignored));

            Assert.IsTrue(ignored.Contains("suspicious"));
        }

        [Test]
        public void ShouldNotBeTriggeredIfDelegateRequiresArgument()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            int n = 3;
            Func<int, int> f = _ => 3;

            Expression<Func<bool>> ex = () => Equals(f, n);
            var p = new ExpressionParser(ex.Body);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(p, ex.Body, out ignored));

            Assert.IsTrue(ignored.Contains("suspicious"));
        }
    }
}