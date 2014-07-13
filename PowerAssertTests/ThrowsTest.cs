using System.Threading.Tasks;

namespace PowerAssertTests
{
    using System;
    using NUnit.Framework;
    using PowerAssert;

    [TestFixture]
    public class ThrowsTest
    {
        [Test]
        public void Should_fail_when_expression_does_not_throw_an_exception()
        {
            try
            {
                PAssert.Throws<Exception>(() => MethodThatDoesNotThrowAnException());
            }
            catch
            {
                return;
            }

            throw new Exception("Expected throws assertion to fail.");
        }

        [Test]
        public void Should_succeed_when_expression_does_throw_an_exception()
        {
            PAssert.Throws<Exception>(() => MethodThatDoesThrow(new Exception()));
        }

        [Test]
        public void Should_return_the_thrown_exception()
        {
            var expectedException = new Exception();

            var actualException = PAssert.Throws<Exception>(() => MethodThatDoesThrow(expectedException));

            Assert.That(actualException, Is.EqualTo(expectedException));
        }

        [Test]
        public void Should_assert_on_the_thrown_exception()
        {
            var expectedException = new Exception("broken");

            var actualException = PAssert.Throws<Exception>(() => MethodThatDoesThrow(expectedException), x => x.Message == "broken");

            Assert.That(actualException, Is.EqualTo(expectedException));
        }

        [Test]
        public async Task Should_succeed_when_async_expression_does_throw_exception()
        {
            await PAssert.Throws<Exception>(async () => await AsyncMethodThatDoesThrow(new Exception()));
        }

        [Test]
        public async Task Should_return_the_thrown_exception_when_async()
        {
            var expectedException = new Exception();

            var actualException = await PAssert.Throws<Exception>(async () => await AsyncMethodThatDoesThrow(expectedException));

            Assert.That(actualException, Is.EqualTo(expectedException));
        }

        [Test]
        public async Task Should_assert_on_the_thrown_exception_when_async()
        {
            var expectedException = new Exception("broken");

            var actualException = await PAssert.Throws<Exception>(async () => await AsyncMethodThatDoesThrow(expectedException), x => x.Message == "broken");

            Assert.That(actualException, Is.EqualTo(expectedException));
        }

        [Test]
        public void Should_fail_if_thrown_exception_is_of_wrong_type()
        {
            try
            {
                PAssert.Throws<ArgumentException>(() => MethodThatDoesThrow(new NullReferenceException()));
            }
            catch
            {
                return;
            }

            throw new Exception("Expected throws assertion to fail.");
        }

        private static void MethodThatDoesNotThrowAnException()
        {
            //NOP
        }

        private static void MethodThatDoesThrow(Exception ex)
        {
            throw ex;
        }

        private static async Task AsyncMethodThatDoesThrow(Exception ex)
        {
            throw ex;
        }
    }
}
