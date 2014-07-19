using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    public class SequenceEqualsHintTests
    {
        [Test]
        public void ShouldTriggerOnSequencesComparedWithSequenceEqual()
        {
            var hint = new SequenceEqualHint();

            Expression<Func<bool>> exp = () => new List<int> {1}.SequenceEqual(new[] {2});

            string description;
            Assert.IsTrue(hint.TryGetHint(exp.Body, out description));
            Assert.IsNotNull(description);
        }

        [Test]
        public void ShouldNotTriggerOnSequencesComparedWithEquals()
        {
            var hint = new SequenceEqualHint();

            Expression<Func<bool>> exp = () => new List<int> {1}.Equals(new[] {2});

            string description;
            Assert.IsFalse(hint.TryGetHint(exp.Body, out description));
            Assert.IsNull(description);
        }
    }
}