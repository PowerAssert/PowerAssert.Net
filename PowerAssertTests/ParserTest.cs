using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
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
            ConstantNode constantNode = Node.Parse(f.Body) as ConstantNode;
            Assert.AreEqual(5, constantNode.Value);
        }

        [Test]
        public void ParsePrimitiveStaticField()
        {
            Expression<Func<int>> f = () => field;
            NamedNode constantNode = Node.Parse(f.Body) as NamedNode;
            Assert.AreEqual("field", constantNode.Name);
            Assert.AreEqual("5", constantNode.Value);
        }

        [Test]
        public void ParseStringConstant()
        {
            Expression<Func<string>> f = () => "foo";
            ConstantNode constantNode = Node.Parse(f.Body) as ConstantNode;
            Assert.AreEqual("foo", constantNode.Value);
        }

        [Test]
        public void ParseMember()
        {
            int x = 5;
            Expression<Func<int>> f = () => x;
            Node constantNode = Node.Parse(f.Body);
            Assert.AreEqual(new NamedNode { Name = "x", Value = "5" }, constantNode);

        }

        [Test]
        public void ParseMemberAccess()
        {
            DateTime d = new DateTime(2010, 12, 25);
            Expression<Func<int>> f = () => d.Day;
            MemberAccessNode node = (MemberAccessNode)Node.Parse(f.Body);

            MemberAccessNode expected = new MemberAccessNode
                                        {
                                            Container = new NamedNode { Name = "d", Value = d.ToString() },
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
            var node = Node.Parse(f.Body);

            var expected = new MethodCallNode
                                        {
                                            Container = new NamedNode { Name = "s", Value = @"""hello""" },
                                            MemberName = "Substring",
                                            MemberValue = @"""ello""",
                                            Parameters = new List<Node> { new ConstantNode { Value = "1" } }

                                        };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseMethodWithException()
        {
            Expression<Func<int>> f = () => ThrowException();
            var node = Node.Parse(f.Body);

            var expected = new MethodCallNode
                                        {
                                            Container = new ConstantNode { Value = "PowerAssertTests.ParserTest"},
                                            MemberName = "ThrowException",
                                            MemberValue = @"DivideByZeroException: Attempted to divide by zero.",

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
            var node = Node.Parse(f.Body);

            var expected = new ConditionalNode
                                        {
                                            Condition = new NamedNode { Name = "b", Value = "False" },
                                            TrueNode = new ConstantNode { Value = "1" },
                                            TrueValue = "1",
                                            FalseNode = new ConstantNode { Value = "2" },
                                            FalseValue = "2",
                                        };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseArrayCreateAndIndex()
        {
            Expression<Func<int>> f = () => new[] { 1, 2, 3 }[1];
            var node = Node.Parse(f.Body);

            var expected = new ArrayIndexNode
                               {
                                   Left = new NewArrayNode
                                   {
                                       Type = "int",
                                       Items = new List<Node>
                                       {
                                           new ConstantNode{Value= "1"},
                                           new ConstantNode{Value= "2"},
                                           new ConstantNode{Value= "3"},
                                       }
                                   },
                                   Right = new ConstantNode { Value = "1"},
                                   Value = "2"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseCast()
        {
            double x = 5.1;
            Expression<Func<int>> f = () => (int)x;
            var node = Node.Parse(f.Body);

            var expected = new UnaryNode
                               {
                                   Prefix = "(int)(",
                                   PrefixValue = "5",
                                   Operand = new NamedNode {Name = "x", Value = 5.1M.ToString()},
                                   Suffix = ")"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseIsOperator()
        {
            object x = "xValue";
            Expression<Func<bool>> f = () => x is string;

            var node = Node.Parse(f.Body);
            var expected = new BinaryNode
                               {
                                   Left = new NamedNode {Name = "x", Value = "\"xValue\""},
                                   Type = ExpressionType.TypeIs,
                                   Right = new ConstantNode { Value = "string" },
                                   Value = "True"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNot()
        {
            var v = true;
            Expression<Func<bool>> f = () => !v;
            var node = Node.Parse(f.Body);

            var expected = new UnaryNode
                                {
                                    Prefix = "!",
                                    PrefixValue = false,
                                    Operand = new NamedNode { Name = "v", Value = true },
                                };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNotAsPartOfBinaryOperation()
        {
            var v = true;
            Expression<Func<bool>> f = () => !v == false;
            var node = Node.Parse(f.Body);

            var expected = new BinaryNode
                            {
                                Left = new UnaryNode
                                        {
                                            Prefix = "!",
                                            PrefixValue = false,
                                            Operand = new NamedNode { Name = "v", Value = true },
                                        },
                                Right = new ConstantNode { Value = false },
                                Type = ExpressionType.Equal,
                                Value = true
                            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNotWrappingBinaryOperation()
        {
            var v = true;
            Expression<Func<bool>> f = () => !(v == false);
            var node = Node.Parse(f.Body);

            var expected = new UnaryNode
                            {
                                Prefix = "!(",
                                PrefixValue = true,
                                Operand = new BinaryNode
                                            {
                                                Left = new NamedNode { Name = "v", Value = true },
                                                Right = new ConstantNode { Value = false },
                                                Type = ExpressionType.Equal,
                                                Value = false
                                            },
                                Suffix = ")"
                            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseUnaryNegate()
        {
            var v = 5;
            Expression<Func<int>> f = () => -v;
            var node = Node.Parse(f.Body);

            var expected = new UnaryNode
                               {
                                   Prefix = "-",
                                   PrefixValue = "-5",
                                   Operand = new NamedNode {Name = "v", Value = "5"},
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseEnumerableWithNulls()
        {
            var v = new List<int?>{1,2,null,4};
            Expression<Func<List<int?>>> f = () => v;
            var node = Node.Parse(f.Body);

            var expected = new NamedNode
                               {
                                   Name = "v",
                                   Value = "[1, 2, null, 4]"
                               };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseBinaryWithNullLeftHandSide()
        {
            string leftHandSide = null;
            Expression<Func<bool>> f = () => leftHandSide == "foo";
            var node = Node.Parse(f.Body);
            var expected = new BinaryNode
            {
                Left = new NamedNode { Name = "leftHandSide", Value = "null" },
                Right = new ConstantNode { Value = "\"foo\"" },
                Type = ExpressionType.Equal,
                Value = "False"
            };

            Assert.AreEqual(expected, node);
        }

        [Test]
        public void ParseBinaryWithNullRightHandSide()
        {
            string rightHandSide = null;
            Expression<Func<bool>> f = () => "foo" == rightHandSide;
            var node = Node.Parse(f.Body);
            var expected = new BinaryNode
            {
                Left = new ConstantNode { Value = "\"foo\"" },
                Right = new NamedNode { Name = "rightHandSide", Value = "null" },
                Type = ExpressionType.Equal,
                Value = "False"
            };

            Assert.AreEqual(expected, node);
        }
    }
}