using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    public class TimeSpanTotalMistakesHintTests
    {
        [Test]
        public void ShouldTriggerOnTotalMisusageWithOperatorEquals_Left()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => TimeSpan.FromMinutes(63).Minutes == 63;

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void ShouldTriggerOnTotalMisusageWithEquals_Left()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => TimeSpan.FromMinutes(63).Minutes.Equals(63);

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void ShouldTriggerOnTotalMisusageWithOperatorEquals_Right()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => 63 == TimeSpan.FromMinutes(63).Minutes;

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void ShouldTriggerOnTotalMisusageWithEquals_Right()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => 63.Equals(TimeSpan.FromMinutes(63).Minutes);

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void ShouldTriggerOnTotalMisusageWithObjectEquals_Right()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => Equals(63, TimeSpan.FromMinutes(63).Minutes);

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }

        [Test]
        public void ShouldTriggerOnTotalMisusageWithObjectEquals_Left()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => Equals(TimeSpan.FromMinutes(63).Minutes, 63);

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void DoesntExplodeOnNonTotalMethods()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => Equals(TimeSpan.FromMinutes(63).Ticks, 63);

            string description;
            Assert.IsFalse(hint.TryGetHint(x.Body, out description));
            Assert.IsNull(description);
        }


        [Test]
        public void DoesntTriggerIfTheyWouldNotBeEqual()
        {
            var hint = new TimeSpanTotalMistakesHint();

            Expression<Func<bool>> x = () => Equals(TimeSpan.FromMinutes(63).Minutes, 62);

            string description;
            Assert.IsFalse(hint.TryGetHint(x.Body, out description));
            Assert.IsNull(description);
        }
    }
}