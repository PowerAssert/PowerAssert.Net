using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;

namespace PowerAssertTests.Hints
{
    // ReSharper disable EqualExpressionComparison

    [TestFixture]
    internal class DelegateShouldHaveBeenInvokedEqualsHintTests
    {
        [Test]
        public void ShouldBeTriggeredWithoutClosure()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            Func<int> f = () => 3;

            Expression<Func<bool>> ex = () => Equals(f, 3);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(ex.Body, out ignored));
            Assert.IsNotNull(ignored);
        }

        [Test]
        public void ShouldBeTriggeredWithClosure()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            int n = 3;
            Func<int> f = () => n; // now this func requires a closure

            Expression<Func<bool>> ex = () => Equals(f, 3);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(ex.Body, out ignored));
            Assert.IsNotNull(ignored);
        }

        [Test]
        public void ShouldNotBeTriggeredIfBothAreDelegates()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            Func<int> f = () => 3;

            Expression<Func<bool>> ex = () => Equals(f, f);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(ex.Body, out ignored));

            Assert.IsTrue(ignored.Contains("suspicious"));
        }

        [Test]
        public void ShouldNotBeTriggeredIfDelegateRequiresArgument()
        {
            var hint = new DelegateShouldHaveBeenInvokedEqualsHint();

            int n = 3;
            Func<int, int> f = _ => 3;

            Expression<Func<bool>> ex = () => Equals(f, n);

            string ignored;
            Assert.IsTrue(hint.TryGetHint(ex.Body, out ignored));

            Assert.IsTrue(ignored.Contains("suspicious"));
        }
    }
}
