using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using PowerAssert.Hints;

namespace PowerAssert.Infrastructure.Nodes
{
    public class Alternative
    {
        public string Alteration { get; set; }
        public Expression Expression { get; set; }
    }

    internal abstract class Node
    {
        internal abstract void Walk(NodeWalker walker, int depth);
        internal delegate void NodeWalker(string text, string value = null, int depth = 0);

        public virtual IEnumerable<Alternative> EnumerateAlternatives(IHint hinter)
        {
            throw new NotImplementedException();
        }

        public static Node Parse(Expression e)
        {
            var lambda = e as LambdaExpression;
            if (lambda != null)
            {
                return Lambda(lambda);
            }
            try
            {
                return ParseExpression((dynamic)e);
            }
            catch (RuntimeBinderException exception)
            {
                throw new Exception(string.Format("Unable to dispatch expression of type {0} with node type of {1}", e.GetType().Name, e.NodeType), exception);
            }
        }
        static Node ParseExpression(TypeBinaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.TypeIs:
                    return new BinaryNode
                    {
                        Left = Parse(e.Expression),
                        Right = new ConstantNode { Value = e.TypeOperand },
                        Value = GetValue(e),
                        Type = e.NodeType
                    };
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Can't handle TypeBinaryExpression of type {0}", e.NodeType));
            }
        }

        internal static string FormatTargetInvocationException(TargetInvocationException exception)
        {
            var i = exception.InnerException;
            return string.Format("{0}: {1}", i.GetType().Name, i.Message);
        }

        protected static object GetValue(Expression e)
        {
            object value;
            try
            {
                value = DynamicInvoke(e);
            }
            catch (TargetInvocationException exception)
            {
                return FormatTargetInvocationException(exception);
            }
            return value;
            //var s = FormatObject(value);
            //return s + GetHints(e, value);
        }

        /*
        static string GetHints(Expression e, object value)
        {
            if (value is bool && !(bool)value)
            {
                string hint;
                if (Hinter.TryGetHint(e, out hint))
                    return hint;
            }
            return "";
        }*/

        internal static string FormatObject(object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is string)
            {
                return "\"" + value + "\"";
            }
            if (value is char)
            {
                return "'" + value + "'";
            }
            var type = value.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                var k = type.GetProperty("Key").GetValue(value, null);
                var v = type.GetProperty("Value").GetValue(value, null);
                return string.Format("{{{0}:{1}}}", FormatObject(k), FormatObject(v));
            }
            if (value is Type)
            {
                return "typeof(" + NameOfType((Type)value) + ")";
            }
            if (value is Delegate)
            {
                var del = (Delegate)value;

                return string.Format("delegate {0}, type: {2} ({1})", NameOfType(del.GetType()), string.Join(", ", del.Method.GetParameters().Select(x => NameOfType(x.ParameterType))), NameOfType(del.Method.ReturnType));
            }
            if (value is IEnumerable)
            {
                var enumerable = (IEnumerable)value;
                var values = enumerable.Cast<object>().Select(FormatObject);
                //in case the enumerable is really long, let's cut off the end arbitrarily?
                const int limit = 5;

                var list = enumerable as IList;
                var knownMax = list != null ? list.Count : default(int?);

                values = values.Take(limit);
                if (values.Count() == limit)
                {
                    values = values.Concat(new[] { knownMax.HasValue && knownMax > limit ? string.Format("... ({0} total)", knownMax) : "..." });
                }
                return "[" + string.Join(", ", values.ToArray()) + "]";
            }
            return value.ToString();
        }

        internal static object DynamicInvoke(Expression e)
        {
            return Expression.Lambda(e).Compile().DynamicInvoke();
        }
        static Node Lambda(LambdaExpression e)
        {
            return new NamedNode { Name = e.ToString(), Value = e.Compile()};
        }

        static Node ParseExpression(UnaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Convert:
                    return new UnaryNode { Prefix = "(" + NameOfType(e.Type) + ")(", Operand = Parse(e.Operand), Suffix = ")", PrefixValue = GetValue(e) };
                case ExpressionType.Not:
                    return ParseUnaryNot(e);
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return new UnaryNode { Prefix = "-", Operand = Parse(e.Operand), PrefixValue = GetValue(e) };
            }

            throw new ArgumentOutOfRangeException("e", string.Format("Can't handle UnaryExpression expression of class {0} and type {1}", e.GetType().Name, e.NodeType));
        }

        static Node ParseUnaryNot(UnaryExpression e)
        {
            string suffix = e.Operand is BinaryExpression ? ")" : null;
            string prefix = e.Operand is BinaryExpression ? "!(" : "!";

            return new UnaryNode { Prefix = prefix, Operand = Parse(e.Operand), PrefixValue = GetValue(e), Suffix = suffix };
        }

        static Node ParseExpression(NewArrayExpression e)
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
                default: // TODO
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

            return Util.Aliases.ContainsKey(t) ? Util.Aliases[t] : t.Name;
        }

        static Node ParseExpression(ConditionalExpression e)
        {
            return new ConditionalNode
            {
                Condition = Parse(e.Test),
                TestValue = (bool)GetValue(e.Test),
                FalseNode = Parse(e.IfFalse),
                FalseValue = GetValue(e.IfFalse),
                TrueNode = Parse(e.IfTrue),
                TrueValue = GetValue(e.IfTrue)
            };
        }

        static Node ParseExpression(MethodCallExpression e)
        {
            var parameters = e.Arguments.Select(Parse).ToArray();
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
                Container = e.Object == null ? new NameOnlyNode{Name=NameOfType(e.Method.DeclaringType)} : Parse(e.Object),
                MemberName = e.Method.Name,
                MemberValue = GetValue(e),
                Parameters = parameters.ToList(),
            };
        }

        static Node ParseExpression(ConstantExpression e)
        {
            var value = GetValue(e);

            return new ConstantNode
            {
                Value = value,
            };
        }

        static Node ParseExpression(MemberExpression e)
        {
            if (IsDisplayClass(e.Expression) || e.Expression == null)
            {
                return new NamedNode
                {
                    Expression = e,
                    Value = GetValue(e),
                    Name = e.Member.Name
                };
            }
            return new MemberAccessNode
            {
                Container = Parse(e.Expression),
                MemberValue = GetValue(e),
                MemberName = e.Member.Name
            };
        }

        static bool IsDisplayClass(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                return expression.Type.Name.StartsWith("<") || expression.Type == PAssert.CurrentTestClass;
            }
            return false;
        }

        static Node ParseExpression(BinaryExpression e)
        {
            return BinaryNode.Create(e);
        }

        static Node ParseExpression(NewExpression e)
        {
            return NewObjectNode.Create(e);
        }
        static Node ParseExpression(MemberInitExpression e)
        {
            return MemberInitNode.Create(e);
        }

        protected static Node ParseExpression(MemberBinding e)
        {
            var assignment = e as MemberAssignment;
            if (assignment != null)
            {
                return MemberAssignmentNode.Create(assignment);
            }

            return new NamedNode { Name = e.Member.Name };
        }

        private static Node ParseExpression(InvocationExpression e)
        {
            return InvocationNode.Create(e);
        }
        public override bool Equals(object obj)
        {
            if(obj.GetType() != GetType())
            {
                return false;
            }

            var allPropertiesMatch = from info in GetType().GetProperties()
                                     let mine = info.GetValue(this, null)
                                     let theirs = info.GetValue(obj, null)
                                     select ObjectsOrEnumerablesEqual(mine, theirs);

            return allPropertiesMatch.All(b => b);
        }

        static bool ObjectsOrEnumerablesEqual(object mine, object theirs)
        {
            if(mine == theirs)
            {
                return true;
            }
            if(mine == null || theirs == null)
            {
                return false;
            }
            return mine is IEnumerable ? ((IEnumerable) mine).Cast<object>().SequenceEqual(((IEnumerable) theirs).Cast<object>()) : mine.Equals(theirs);
        }

        public override int GetHashCode()
        {
            var v = from info in GetType().GetProperties()
                    let value = info.GetValue(this, null)
                    select value == null ? 0 : value.GetHashCode();

            return v.Aggregate((x, y) => x ^ y * 397);
        }

        public override string ToString()
        {
            var strings = NodeFormatter.Format(this);
            return string.Join(Environment.NewLine, strings);
        }
    }
}