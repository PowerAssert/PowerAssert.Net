using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Hints;

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

            string description;
            Assert.IsFalse(hint.TryGetHint(x.Body, out description));
            Assert.IsNull(description);
        }


        [Test]
        public void HintPicksUpDiff()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "x".Equals("X");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }


        [Test]
        public void ShouldPickUpDecomposedCharacters()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => ConstantStrings.AcuteEComposed.Equals(ConstantStrings.AcuteEDecomposed);

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("decomposed"));
        }


        [Test]
        public void ShouldPickUpTabVsSpace()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => " ".Equals("\t");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("tab"));
        }


        [Test]
        public void ShouldPickUpMismatchedNewlines()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => new string('\n', 1).Equals("\r\n");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("carriage-return"));
        }

        [Test]
        public void ShouldPickUpControlCharacters()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\0Hello".Equals("Hello");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("control"));
        }

        [Test]
        public void ShouldPickUpFormatCharacters()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\u202DHello".Equals("Hello");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("format"));
        }

        [Test]
        public void ShouldPickUpDecomposedCharacters_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => ConstantStrings.AcuteEDecomposed.Equals(ConstantStrings.AcuteEComposed);

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("decomposed"));
        }


        [Test]
        public void ShouldPickUpTabVsSpace_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\t".Equals(" ");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("tab"));
        }


        [Test]
        public void ShouldPickUpMismatchedNewlines_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "\r\n".Equals(new string('\n', 1));

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("carriage-return"));
        }

        [Test]
        public void ShouldPickUpControlCharacters_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "Hello".Equals("\0Hello");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("control"));
        }

        [Test]
        public void ShouldPickUpFormatCharacters_Right()
        {
            var hint = new StringEqualsHint();

            Expression<Func<bool>> x = () => "Hello".Equals("\u202DHello");

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("format"));
        }
    }
}
