using System;
using System.Linq;
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
            string[] s = NodeFormatter.Format(new ConstantNode {Text = "5", Value = null});
            AssertLines(new[] { "5" }, s);
        }

        [Test]
        public void FormatOperator()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
                                              {
                                                  Operator = "==",
                                                  Value = "false",
                                                  Left = new ConstantNode {Text = "5"},
                                                  Right = new ConstantNode {Text = "6"}
                                              });

            string[] expected = {
                                    "5 == 6",
                                    "  __",
                                    "  false"
                                };

            AssertLines(expected, s);
        }

        [Test]
        public void FormatTwoOperators()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
                                              {
                                                  Operator = "==",
                                                  Value = "false",
                                                  Left = new ConstantNode {Text = "31"},
                                                  Right = new BinaryNode
                                                  {
                                                      Operator = "*",
                                                      Value = "30",
                                                      Left = new ConstantNode { Text = "5" },
                                                      Right = new ConstantNode { Text = "6" }
                                                  }
                                              });

            string[] expected = {
                                    "31 == 5 * 6",
                                    "   __   .",
                                    "   |    _",
                                    "   |    30",
                                    "   false"
                                };

            AssertLines(expected, s);
        }

        [Test]
        public void FormatInvocationNode()
        {
            string[] s = NodeFormatter.Format(new BinaryNode
            {
                Left = new InvocationNode
                {
                    Arguments = new Node[] 
                                       {
                                            new ConstantNode { Text = "a", Value = "3" },
                                            new ConstantNode { Text = "x", Value = "2.423" }
                                       },
                    Expression = new ConstantNode { Text = "f", Value = "System.Func`3[System.Int32,System.Double,System.Double]" }
                },
                Operator = "==",
                Right = new ConstantNode { Text = "314.4" },
                Value = "False"
            });

            string[] expected = {
                                    "f(a, x) == 314.4",
                                    ". .  .  __",
                                    "_ .  .  |",
                                    "| _  _  |",
                                    "| |  |  False",
                                    "| |  2.423",
                                    "| 3",
                                    "System.Func`3[System.Int32,System.Double,System.Double]"
                                };

            AssertLines(expected, s);
        }

        static void AssertLines(string[] expected, string[] actual)
        {
            Assert.AreEqual(string.Join(Environment.NewLine, expected), string.Join(Environment.NewLine, actual));
        }
    }
}