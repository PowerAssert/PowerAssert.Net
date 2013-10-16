using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using PowerAssert.Hints;

namespace PowerAssertTests.Hints
{
    static class ConstantStrings
    {
        public static readonly string AcuteEComposed = "É".Normalize(NormalizationForm.FormC);
        public static readonly string AcuteEDecomposed = "É".Normalize(NormalizationForm.FormD);
    }

    [TestFixture]
    class StringOperatorEqualsHintTests
    {
        [Test]
        public void DoesTriggerOnDifferentStrings()
        {
            var hint = new StringOperatorEqualsHint();

            Expression<Func<bool>> x = () => new string('x', 1) == "X"; // prevent inlining the constant

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);
        }

        [Test]
        public void ShouldPickUpDecomposedCharacters()
        {
            var hint = new StringOperatorEqualsHint();

            Expression<Func<bool>> x = () => ConstantStrings.AcuteEComposed == ConstantStrings.AcuteEDecomposed;

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("decomposed"));
        }

        [Test]
        public void ShouldPickUpTabVsSpace()
        {
            var hint = new StringOperatorEqualsHint();

            Expression<Func<bool>> x = () => new string(' ', 1) == "\t";// prevent inlining the constant

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("tab"));
        }


        [Test]
        public void ShouldPickUpMismatchedNewlines()
        {
            var hint = new StringOperatorEqualsHint();

            Expression<Func<bool>> x = () => new string('\n', 1) == "\r\n";// prevent inlining the constant

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("carriage-return"));
        }

        [Test]
        public void ShouldPickUpControlCharacters()
        {
            var hint = new StringOperatorEqualsHint();

            Expression<Func<bool>> x = () => new string('\0', 1) + "Hello" == "Hello";// prevent inlining the constant

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("control"));
        }

        [Test]
        public void ShouldPickUpFormatCharacters()
        {
            var hint = new StringOperatorEqualsHint();

            Expression<Func<bool>> x = () => new string('\u202d', 1) + "Hello" == "Hello";// prevent inlining the constant

            string description;
            Assert.IsTrue(hint.TryGetHint(x.Body, out description));
            Assert.IsNotNull(description);

            Assert.IsTrue(description.Contains("format"));
        }
    }
}
