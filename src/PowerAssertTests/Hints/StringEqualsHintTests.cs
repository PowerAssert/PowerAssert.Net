using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    class StringEqualsHintTests
    {
        [Test]
        public void HintObeysComparer()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "x".Equals("X", StringComparison.OrdinalIgnoreCase);
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsFalse(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNull(description);
        }


        [Test]
        public void HintPicksUpDiff()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "x".Equals("X");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void ShouldPickUpDecomposedCharacters()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => ConstantStrings.AcuteEComposed.Equals(ConstantStrings.AcuteEDecomposed);
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("decomposed"));
        }


        [Test]
        public void ShouldPickUpTabVsSpace()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => " ".Equals("\t");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("tab"));
        }


        [Test]
        public void ShouldPickUpMismatchedNewlines()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => new string('\n', 1).Equals("\r\n");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("carriage-return"));
        }

        [Test]
        public void ShouldPickUpControlCharacters()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\0Hello".Equals("Hello");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("control"));
        }

        [Test]
        public void ShouldPickUpFormatCharacters()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\u202DHello".Equals("Hello");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("format"));
        }

        [Test]
        public void ShouldPickUpDecomposedCharacters_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => ConstantStrings.AcuteEDecomposed.Equals(ConstantStrings.AcuteEComposed);
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("decomposed"));
        }


        [Test]
        public void ShouldPickUpTabVsSpace_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\t".Equals(" ");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("tab"));
        }


        [Test]
        public void ShouldPickUpMismatchedNewlines_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\r\n".Equals(new string('\n', 1));
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("carriage-return"));
        }

        [Test]
        public void ShouldPickUpControlCharacters_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "Hello".Equals("\0Hello");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("control"));
        }

        [Test]
        public void ShouldPickUpFormatCharacters_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "Hello".Equals("\u202DHello");
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("format"));
        }
    }
}