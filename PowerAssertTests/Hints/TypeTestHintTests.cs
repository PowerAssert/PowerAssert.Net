using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;

namespace PowerAssertTests.Hints
{
    [TestFixture]
    public class TypeTestHintTests
    {
        [Test]
        public void ShouldPickUpTypeComparisonOperator()
        {
            var hint = new TypeTestHint();

            var a = new TypeA();

            Expression<Func<bool>> x = () => a is TypeB;
            var p = new ExpressionParser(x.Body);

            string description;
            Assert.IsTrue(hint.TryGetHint(p, x.Body, out description));
            Assert.IsNotNull(description);
        }

        class TypeA { }
        class TypeB { }
    }
}
