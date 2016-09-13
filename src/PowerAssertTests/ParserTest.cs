using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssertTests
{
    [TestFixture]
    public class ParserTest
    {
        static int field = 5;

        [Test]
        public void ParsePrimitiveConstant()
        {
            Expression<Func<int>> f = () => 5;
            var p = new ExpressionParser(f.Body);
            ConstantNode constantNode = p.Parse() as ConstantNode;
            Assert.AreEqual("5", constantNode.Text);
        }

        [Test]
        public void ParsePrimitiveStaticField()
        {
            Expression<Func<int>> f = () => field;
            var p = new ExpressionParser(f.Body);
            ConstantNode constantNode = p.Parse() as ConstantNode;
            Assert.AreEqual("field", constantNode.Text);
            Assert.AreEqual("5", constantNode.Value);
        }

        [Test]
        public void ParseStringConstant()
        {
            Expression<Func<string>> f = () => "foo";
            var p = new ExpressionParser(f.Body);
            ConstantNode constantNode = p.Parse() as ConstantNode;
            Assert.AreEqual("\"foo\"", constantNode.Text);
        }

        [Test]
        public void ParseMember()
        {
            int x = 5;
            Expression<Func<int>> f = () => x;
            var p = new ExpressionParser(f.Body);
            Node constantNode = p.Parse();
            Assert.AreEqual(new ConstantNode {Text = "x", Value = "5"}, constantNode);
        }

        [Test]
        public void ParseMemberAccess()
        {
            DateTime d = new DateTime(2010, 12, 25);
            Expression<Func<int>> f = () => d.Day;
            var p = new ExpressionParser(f.Body);
            MemberAccessNode node = (MemberAccessNode) p.Parse();

            MemberAccessNode expected = new MemberAccessNode
            {
                Container = new ConstantNode {Text = "d", Value = d.ToString()},
                MemberName = "Day",
                MemberValue = "25"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseMethodAccess()
        {
            string s = "hello";
            Expression<Func<string>> f = () => s.Substring(1);
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new MethodCallNode
            {
                Container = new ConstantNode {Text = "s", Value = @"""hello"""},
                MemberName = "Substring",
                MemberValue = @"""ello""",
                Parameters = new List<Node>() {new ConstantNode {Text = "1"}}
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseMethodWithException()
        {
            Expression<Func<int>> f = () => ThrowException();
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new MethodCallNode
            {
                Container = new ConstantNode {Text = "PowerAssertTests.ParserTest"},
                MemberName = "ThrowException",
                MemberValue = @"(threw DivideByZeroException)",
            };

            Assert.AreEqual(expected, node);
        }

        int ThrowException()
        {
            var d = 0;
            return 1/d;
        }

        [Test]
        public void ParseConditional()
        {
            bool b = false;
            Expression<Func<int>> f = () => b ? 1 : 2;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new ConditionalNode
            {
                Condition = new ConstantNode {Text = "b", Value = "False"},
                TrueNode = new ConstantNode {Text = "1"},
                TrueValue = "1",
                FalseNode = new ConstantNode {Text = "2"},
                FalseValue = "2",
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseArrayCreateAndIndex()
        {
            Expression<Func<int>> f = () => new int[] {1, 2, 3}[1];
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new ArrayIndexNode
            {
                Array = new NewArrayNode
                {
                    Type = "int",
                    Items = new List<Node>
                    {
                        new ConstantNode {Text = "1"},
                        new ConstantNode {Text = "2"},
                        new ConstantNode {Text = "3"},
                    }
                },
                Index = new ConstantNode {Text = "1"},
                Value = "2"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseCast()
        {
            double x = 5.1;
            Expression<Func<int>> f = () => (int) x;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new UnaryNode
            {
                Prefix = "(int)",
                PrefixValue = "5",
                Operand = new ConstantNode() {Text = "x", Value = 5.1M.ToString()},
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseIsOperator()
        {
            object x = "xValue";
            Expression<Func<bool>> f = () => x is string;
            var p = new ExpressionParser(f.Body);

            var node = p.Parse();
            var expected = new BinaryNode()
            {
                Left = new ConstantNode() {Text = "x", Value = "\"xValue\""},
                Operator = "is",
                Right = new ConstantNode() {Text = "string"},
                Value = "True"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNot()
        {
            var v = true;
            Expression<Func<bool>> f = () => !v;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new UnaryNode
            {
                Prefix = "!",
                PrefixValue = "False",
                Operand = new ConstantNode() {Text = "v", Value = "True"},
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNotAsPartOfBinaryOperation()
        {
            var v = true;
            Expression<Func<bool>> f = () => !v == false;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new BinaryNode
            {
                Left = new UnaryNode
                {
                    Prefix = "!",
                    PrefixValue = "False",
                    Operand = new ConstantNode() {Text = "v", Value = "True"},
                },
                Right = new ConstantNode {Text = "False", Value = null},
                Operator = "==",
                Value = "True"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNotWrappingBinaryOperation()
        {
            var v = true;
            Expression<Func<bool>> f = () => !(v == false);
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new UnaryNode
            {
                Prefix = "!",
                PrefixValue = "True",
                Operand = new BinaryNode
                {
                    Left = new ConstantNode {Text = "v", Value = "True"},
                    Right = new ConstantNode {Text = "False", Value = null},
                    Operator = "==",
                    Value = "False"
                }
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNegate()
        {
            var v = 5;
            Expression<Func<int>> f = () => -v;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new UnaryNode
            {
                Prefix = "-",
                PrefixValue = "-5",
                Operand = new ConstantNode() {Text = "v", Value = "5"},
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseEnumerableWithNulls()
        {
            var v = new List<int?> {1, 2, null, 4};
            Expression<Func<List<int?>>> f = () => v;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();

            var expected = new ConstantNode()
            {
                Text = "v",
                Value = "[1, 2, null, 4]"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseBinaryWithNullLeftHandSide()
        {
            string leftHandSide = null;
            Expression<Func<bool>> f = () => leftHandSide == "foo";
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();
            var expected = new BinaryNode
            {
                Left = new ConstantNode {Text = "leftHandSide", Value = "null"},
                Right = new ConstantNode {Text = "\"foo\""},
                Operator = "==",
                Value = "False"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseBinaryWithNullRightHandSide()
        {
            string rightHandSide = null;
            Expression<Func<bool>> f = () => "foo" == rightHandSide;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();
            var expected = new BinaryNode
            {
                Left = new ConstantNode {Text = "\"foo\""},
                Right = new ConstantNode {Text = "rightHandSide", Value = "null"},
                Operator = "==",
                Value = "False"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseCoalesce()
        {
            string x = null;
            Expression<Func<string>> f = () => x ?? "foo";
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();
            var expected = new BinaryNode
            {
                Left = new ConstantNode { Text = "x", Value = "null" },
                Right = new ConstantNode { Text = "\"foo\"" },
                Operator = "??",
                Value = "\"foo\""
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseNewAnonymousType()
        {
            int x = 1;
            Expression<Func<int>> f = () => new { Value = x, x }.Value;
            var p = new ExpressionParser(f.Body);
            var node = p.Parse();
            var expected = new MemberAccessNode
            {
                Container = new NewAnonymousTypeNode
                {
                    Parameters = new[]
                    {
                        new MemberAssignmentNode
                        {
                            MemberName = "Value",
                            Value = new ConstantNode {Text = "x", Value="1" }
                        },
                        new MemberAssignmentNode
                        {
                            MemberName = "x",
                            Value = new ConstantNode {Text = "x", Value="1" }
                        }
                    }.ToList(),
                    Value = "{ Value = 1, x = 1 }"
                },
                MemberName = "Value",
                MemberValue = "1"
            };
            Assert.AreEqual(expected, node);
        }
    }
}