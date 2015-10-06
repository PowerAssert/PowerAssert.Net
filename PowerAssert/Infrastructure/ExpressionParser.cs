using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using PowerAssert.Hints;
using PowerAssert.Infrastructure.Nodes;
using System.Diagnostics;

namespace PowerAssert.Infrastructure
{
    class ExpressionParser
    {
        static Assembly MyAssembly = typeof(ExpressionParser).Assembly;

        public Expression RootExpression { get; private set; }
        public Type TestClass { get; private set; }
        public bool TextOnly { get; private set; }

        public ExpressionParser(Expression expression, bool textOnly = false, Type testClass = null)
        {
            RootExpression = expression;
            TestClass = testClass ?? GetTestClass();
            TextOnly = textOnly;
        }

        static Type GetTestClass()
        {
            var st = new StackTrace(1, false);
            return st.GetFrames().Select(f => f.GetMethod().DeclaringType)
                     .FirstOrDefault(t => t != null && t.Assembly != MyAssembly);
        }

        public Node Parse()
        {
            return Parse(RootExpression);
        }

        public Node Parse(Expression e)
        {
            try
            {
                return ParseExpression((dynamic) e);
            }
            catch (RuntimeBinderException exception)
            {
                throw new Exception(string.Format("Unable to dispatch expression of type {0} with node type of {1}", e.GetType().Name, e.NodeType), exception);
            }
        }

        Node ParseExpression(TypeBinaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.TypeIs:
                    return new BinaryNode
                    {
                        Left = Parse(e.Expression),
                        Operator = "is",
                        Right = new ConstantNode() {Text = NameOfType(e.TypeOperand)},
                        Value = GetValue(e)
                    };
                default:
                    throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture,
                        "Can't handle TypeBinaryExpression of type {0}",
                        e.NodeType));
            }
        }

        Node ParseExpression(LambdaExpression e)
        {
            string parameters;
            if (e.Parameters.Count == 0)
            {
                parameters = "()";
            }
            else if (e.Parameters.Count == 1)
            {
                parameters = e.Parameters[0].Name;
            }
            else
            {
                parameters = "(" + string.Join(", ", e.Parameters.Select(p => p.Name)) + ")";
            }
            var parser = new ExpressionParser(e.Body, true, TestClass);
            return new BinaryNode
            {
                Operator = "=>",
                Left = new ConstantNode { Text = parameters },
                Right = parser.Parse(),
            };
        }

        Node ParseExpression(ParameterExpression e)
        {
            return new ConstantNode
            {
                Text = e.Name,
                Value = GetValue(e)
            };
        }

        Node ParseExpression(UnaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Convert:
                    return new UnaryNode {Prefix = "(" + NameOfType(e.Type) + ")", Operand = Parse(e.Operand), PrefixValue = GetValue(e)};
                case ExpressionType.Not:
                    return new UnaryNode {Prefix = "!", Operand = Parse(e.Operand), PrefixValue = GetValue(e)};
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return new UnaryNode {Prefix = "-", Operand = Parse(e.Operand), PrefixValue = GetValue(e)};
                case ExpressionType.ArrayLength:
                    return new MemberAccessNode {Container = Parse(e.Operand), MemberName = "Length", MemberValue = GetValue(e)};
            }
            throw new ArgumentOutOfRangeException("e", string.Format("Can't handle UnaryExpression expression of class {0} and type {1}", e.GetType().Name, e.NodeType));
        }

        Node ParseExpression(NewArrayExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.NewArrayInit:
                    Type t = e.Type.GetElementType();
                    return new NewArrayNode
                    {
                        Items = e.Expressions.Select(Parse).ToList(),
                        Type = NameOfType(t)
                    };
                case ExpressionType.NewArrayBounds:
                    //todo:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static string NameOfType(Type t)
        {
            if (t.IsGenericType)
            {
                var typeArgs = t.GetGenericArguments().Select(NameOfType);
                return string.Format("{0}<{1}>", t.Name.Split('`')[0], string.Join(", ", typeArgs));
            }
            else
            {
                return Util.Aliases.ContainsKey(t) ? Util.Aliases[t] : t.Name;
            }
        }

        Node ArrayIndex(BinaryExpression e)
        {
            return new ArrayIndexNode() {Array = Parse(e.Left), Index = Parse(e.Right), Value = GetValue(e)};
        }

        Node ParseExpression(ConditionalExpression e)
        {
            return new ConditionalNode
            {
                Condition = Parse(e.Test),
                TestValue = bool.Parse(GetValue(e.Test)),
                FalseNode = Parse(e.IfFalse),
                FalseValue = GetValue(e.IfFalse),
                TrueNode = Parse(e.IfTrue),
                TrueValue = GetValue(e.IfTrue)
            };
        }

        Node ParseExpression(MethodCallExpression e)
        {
            var parameters = e.Arguments.Select(Parse).ToList();
            if (e.Method.GetCustomAttributes(typeof (ExtensionAttribute), true).Any())
            {
                return new MethodCallNode
                {
                    Container = parameters.First(),
                    MemberName = e.Method.Name,
                    MemberValue = GetValue(e),
                    Parameters = parameters.Skip(1).ToList(),
                };
            }
            return new MethodCallNode
            {
                Container = e.Object == null ? new ConstantNode {Text = e.Method.DeclaringType.Name} : Parse(e.Object),
                MemberName = e.Method.Name,
                MemberValue = GetValue(e),
                Parameters = parameters.ToList(),
            };
        }

        Node ParseExpression(ConstantExpression e)
        {
            string value = GetValue(e);

            return new ConstantNode
            {
                Text = value
            };
        }

        Node ParseExpression(MemberExpression e)
        {
            if (IsDisplayClass(e.Expression) || e.Expression == null)
            {
                return new ConstantNode
                {
                    Value = GetValue(e),
                    Text = GetDisplayText(e.Member)
                };
            }
            return new MemberAccessNode
            {
                Container = Parse(e.Expression),
                MemberValue = GetValue(e),
                MemberName = GetDisplayText(e.Member)
            };
        }


        bool IsDisplayClass(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                var typeName = expression.Type.Name;
                return typeName.StartsWith("<") || typeName.StartsWith("_Closure$__") || expression.Type == TestClass;
            }
            return false;
        }

        Node ParseExpression(BinaryExpression e)
        {
            return e.NodeType == ExpressionType.ArrayIndex
                ? ArrayIndex(e)
                : new BinaryNode
                {
                    Operator = Util.BinaryOperators[e.NodeType],
                    Value = GetValue(e),
                    Left = Parse(e.Left),
                    Right = Parse(e.Right),
                };
        }

        Node ParseExpression(NewExpression e)
        {
            return new NewObjectNode
            {
                Type = NameOfType(e.Type),
                Parameters = e.Arguments.Select(Parse).ToList(),
                Value = GetValue(e)
            };
        }

        Node ParseExpression(MemberInitExpression e)
        {
            return new MemberInitNode
            {
                Constructor = new NewObjectNode
                {
                    Type = NameOfType(e.NewExpression.Type),
                    Parameters = e.NewExpression.Arguments.Select(Parse).ToList(),
                    Value = GetValue(e)
                },
                Bindings = e.Bindings.Select(ParseExpression).ToList()
            };
        }

        Node ParseExpression(MemberBinding e)
        {
            if (e is MemberAssignment)
            {
                return new MemberAssignmentNode {MemberName = GetDisplayText(e.Member), Value = Parse(((MemberAssignment) e).Expression)};
            }
            return new ConstantNode() {Text = GetDisplayText(e.Member)};
        }

        Node ParseExpression(ListInitExpression e)
        {
            var items = e.Initializers.SelectMany(x => x.Arguments);
            return new ListInitNode
            {
                Constructor = new NewObjectNode
                {
                    Type = NameOfType(e.NewExpression.Type),
                    Parameters = e.NewExpression.Arguments.Select(Parse).ToList(),
                    Value = GetValue(e)
                },
                Items = items.Select(x => (Node)ParseExpression((dynamic)x)).ToList()
            };
        }

	    Node ParseExpression(InvocationExpression e)
        {
            return new InvocationNode
            {
                Arguments = e.Arguments.Select(Parse).ToList(),
                Expression = Parse(e.Expression)
            };
        }

        string GetValue(Expression e)
        {
            object value;
            try
            {
                if (e is ConstantExpression)
                {
                    value = ((ConstantExpression)e).Value;
                }
                else if (TextOnly)
                {
                    return null; // Return immediately (Don't apply ObjectFormatter)
                }
                else
                {
                    value = DynamicInvoke(e);
                }
            }
            catch (TargetInvocationException exception)
            {
                return ObjectFormatter.FormatTargetInvocationException(exception);
            }
            var s = ObjectFormatter.FormatObject(value);
            return s + GetHints(e, value);
        }

        const string VBLOCAL_PREFIX = "$VB$Local_";
        string GetDisplayText(MemberInfo member)
        {
            return member.Name.StartsWith(VBLOCAL_PREFIX) ? member.Name.Substring(VBLOCAL_PREFIX.Length) : member.Name;
        }

        static readonly IHint Hinter = new MultiHint(
            new MethodEqualsInsteadOfOperatorEqualsHint(),
            new StringOperatorEqualsHint(),
            new EnumerableOperatorEqualsHint(),
            new SequenceEqualHint(),
            new DelegateShouldHaveBeenInvokedEqualsHint(),
            new StringEqualsHint(),
            new EnumerableEqualsHint(),
            new FloatEqualityHint(),
            new BrokenEqualityHint(),
            new TimeSpanTotalMistakesHint(),
            new EnumComparisonShowValuesHint()
            );

        string GetHints(Expression e, object value)
        {
            if (value is bool && !(bool) value)
            {
                string hint;
                if (Hinter.TryGetHint(this, e, out hint))
                {
                    return hint;
                }
            }
            return "";
        }


        internal object DynamicInvoke(Expression e)
        {
            if (TextOnly)
                return null;
            return Expression.Lambda(e).Compile().DynamicInvoke();
        }
    }
}
