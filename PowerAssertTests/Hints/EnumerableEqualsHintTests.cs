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
    class EnumerableEqualsHintTests
    {
        [Test]
        public void ShouldntTriggerOnNonSequenceEqualEnumerables()
        {
            var hint = new EnumerableEqualsHint();

            var x = new[] { 2 };
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x.Equals(y);

            string message;
            Assert.IsFalse(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNull(message);
        }

        [Test]
        public void ShouldTriggerOnSequenceEqualEnumerables()
        {
            var hint = new EnumerableEqualsHint();

            var x = new[] { 3 };
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x.Equals(y);

            string message;
            Assert.IsTrue(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNotNull(message);
        }

        [Test]
        public void ShouldntTriggerOnOperatorEquals()
        {
            var hint = new EnumerableEqualsHint();

            var x = new[] { 3 };
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x == y;

            string message;
            Assert.IsFalse(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNull(message);
        }

        [Test]
        public void ShouldTriggerForDifferentEnumerables()
        {
            var hint = new EnumerableEqualsHint();

            var x = new[] { 3 };
            var y = new List<int> { 3 };

            Expression<Func<bool>> assertion = () => x.Equals(y);

            string message;
            Assert.IsTrue(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNotNull(message);
        }

        private class StupidClass : IEnumerable<int>
        {
            public bool Equals(int[] other)
            {
                return false;
            }

            public IEnumerator<int> GetEnumerator()
            {
                yield return 3;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Test]
        public void ShouldntTriggerForRandomMethodCalledEqualsEvenIfItSequenceEquals()
        {
            var hint = new EnumerableEqualsHint();

            var x = new StupidClass();
            var y = new[] { 3 };

            Expression<Func<bool>> assertion = () => x.Equals(y);

            string message;
            Assert.IsFalse(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNull(message);
        }

        [Test]
        public void DoesntExplodeIfOneIsNull()
        {
            var hint = new EnumerableEqualsHint();

            var x = new[] { 3 };
            object y = null;

            Expression<Func<bool>> assertion = () => x.Equals(y);

            string message;
            Assert.IsFalse(hint.TryGetHint(assertion.Body, out message));
            Assert.IsNull(message);
        }
    }
}
