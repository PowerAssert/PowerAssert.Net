using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using PowerAssert.Hints;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    class EnumerableOperatorEqualsHintTests
    {
        [Test]
        public void ShouldntTriggerOnNonSequenceEqualEnumerables()
        {
            var hint = new EnumerableOperatorEqualsHint();

            var x = new[] { 2 };
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x == y;

            string message;
            Assert.IsFalse(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNull(message);
        }

        [Test]
        public void ShouldTriggerOnSequenceEqualEnumerables()
        {
            var hint = new EnumerableOperatorEqualsHint();

            var x = new[] { 3 };
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x == y;

            string message;
            Assert.IsTrue(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNotNull(message);
        }

        [Test]
        public void ShouldntTriggerOnEqualsMethod()
        {
            var hint = new EnumerableOperatorEqualsHint();

            var x = new[] { 3 };
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x.Equals(y);

            string message;
            Assert.IsFalse(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNull(message);
        }

        [Test]
        public void ShouldTriggerForDifferentEnumerables()
        {
            var hint = new EnumerableOperatorEqualsHint();

            object x = new[] { 3 };
            object y = new List<int> { 3 };

            Expression<Func<bool>> assertion = () => x == y;

            string message;
            Assert.IsTrue(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNotNull(message);
        }
    }
}
