using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    public class MethodEqualsInsteadOfOperatorEqualsHintTests
    {
        class AlwaysEqual
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

            string description;
            Assert.IsTrue(hint.TryGetHint(exp.Body, out description));
            Assert.IsNotNull(description);
        }

        class NeverEqual
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

            string description;
            Assert.IsFalse(hint.TryGetHint(exp.Body, out description));
            Assert.IsNull(description);
        }
    }
}
