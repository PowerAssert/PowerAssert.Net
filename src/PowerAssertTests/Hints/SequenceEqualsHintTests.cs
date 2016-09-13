using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

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
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, exp.Body, out description));
            Assert.IsNotNull(description);
        }

        [Test]
        public void ShouldNotTriggerOnSequencesComparedWithEquals()
        {
            var hint = new SequenceEqualHint();

            Expression<Func<bool>> exp = () => new List<int> {1}.Equals(new[] {2});
            var p = new ExpressionParser(exp.Body);

            string description;
            Assert.IsFalse(hint.TryGetHint(p, exp.Body, out description));
            Assert.IsNull(description);
        }
    }
}