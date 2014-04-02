using System;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssertTests
{
    [TestFixture]
    public class NodeFormatterTest
    {
        [Test]
        public void FormatConstant()
        {
            string[] s = NodeFormatter.Format(new ConstantNode {Value = 5});
            AssertLines(new[] { "5" }, s);
        }

        [Test]
        public void FormatOperator()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
                                              {
                                                  Value = false,
                                                  Left = new ConstantNode {Value = 5},
                                                  Right = new ConstantNode {Value = 6},
                                                  Type = ExpressionType.Equal
                                              });

            string[] expected = {
                                    "5 == 6",
                                    "  __",
                                    "  False"
                                };

            AssertLines(expected, s);
        }

        [Test]
        public void FormatTwoOperators()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
                                              {
                                                  Type = ExpressionType.Equal,
                                                  Value = "false",
                                                  Left = new ConstantNode {Value = "31"},
                                                  Right = new BinaryNode
                                                  {
                                                      Type = ExpressionType.Multiply,
                                                      Value = "30",
                                                      Left = new ConstantNode { Value = "5" },
                                                      Right = new ConstantNode { Value = "6" }
                                                  }
                                              });

            string[] expected = {
                                    "31 == 5 * 6",
                                    "   __   .",
                                    "   |    .",
                                    "   |    30",
                                    "   false"
                                };

            AssertLines(expected, s);
        }

        [Test]
        public void FormatInvocationNode()
        {
            Func<int, double, double> f = (i, i2) => i + i2;
            int a = 3;
            double x = 2.423;

            string[] s = NodeFormatter.Format(BinaryNode.Create((BinaryExpression)((Expression<Func<bool>>) (() => f(a, x) == 314.4)).Body));

            string[] expected = {
                                    "f(a, x) == 314.4",
                                    ". .  .  __",
                                    ". .  .  |",
                                    "| .  .  |",
                                    "| |  |  False",
                                    "| |  2.423",
                                    "| 3",
                                    "delegate Func<int, double, double>, type: double (int, double)"
                                };

            AssertLines(expected, s);
        }

        static void AssertLines(string[] expected, string[] actual)
        {
            Assert.AreEqual(string.Join(Environment.NewLine, expected), string.Join(Environment.NewLine, actual));
        }
    }
}