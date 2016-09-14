using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    public class MethodEqualsInsteadOfOperatorEqualsHintTests
    {
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        class AlwaysEqual
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        {
            public override bool Equals(object obj)
            {
                return true;
            }
        }

        [Test]
        public void TriggersIfComparesEqual()
        {
            var hint = new MethodEqualsInsteadOfOperatorEqualsHint();

            Expression<Func<bool>> exp = () => new AlwaysEqual() == new AlwaysEqual();
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, exp.Body, out description));
            Assert.IsNotNull(description);
        }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        class NeverEqual
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        {
            public override bool Equals(object obj)
            {
                return false;
            }
        }

        [Test]
        public void DoesntTriggerIfNotComparesEqual()
        {
            var hint = new MethodEqualsInsteadOfOperatorEqualsHint();

            Expression<Func<bool>> exp = () => new NeverEqual() == new NeverEqual();
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsFalse(hint.TryGetHint(p, exp.Body, out description));
            Assert.IsNull(description);
        }
    }
}