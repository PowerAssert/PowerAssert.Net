﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Infrastructure
{
    internal class ExpressionParser
    {
        public static Node Parse(Expression e)
        {
            if (e.NodeType == ExpressionType.Lambda)
            {
                return Lambda(e);
            }
            try
            {
                return ParseExpression((dynamic)e);
            }
            catch (RuntimeBinderException exception)
            {
                throw new Exception(string.Format("Unable to dispach expression of type {0} with node type of {1}", e.GetType().Name, e.NodeType), exception);
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
                                   Operator = "is",
                                   Right = new ConstantNode() { Text = NameOfType(e.TypeOperand) },
                                   Value = GetValue(e)
                               };
                default:
                    throw new NotImplementedException(string.Format(CultureInfo.CurrentCulture,
                                                                    "Can't handle TypeBinaryExpression of type {0}",
                                                                    e.NodeType));
            }
        }

        static Node Lambda(Expression e)
        {
            return new ConstantNode() { Text = e.ToString() };
        }

        static Node ParseExpression(UnaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Convert:
                    return new UnaryNode { Prefix = "(" + NameOfType(e.Type) + ")(", Operand = Parse(e.Operand), Suffix = ")", PrefixValue = GetValue(e) };
                case ExpressionType.Not:
                    return new UnaryNode { Prefix = "!", Operand = Parse(e.Operand), PrefixValue = GetValue(e) };
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return new UnaryNode { Prefix = "-", Operand = Parse(e.Operand), PrefixValue = GetValue(e) };


            }
            throw new ArgumentOutOfRangeException("e", string.Format("Can't handle UnaryExpression expression of class {0} and type {1}", e.GetType().Name, e.NodeType));

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
                case ExpressionType.NewArrayBounds:
                //todo:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static string NameOfType(Type t)
        {
            return Util.Aliases.ContainsKey(t) ? Util.Aliases[t] : t.Name;
        }

        static Node ArrayIndex(BinaryExpression e)
        {
            return new ArrayIndexNode() { Array = Parse(e.Left), Index = Parse(e.Right), Value = GetValue(e) };
        }

        static Node ParseExpression(ConditionalExpression e)
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

        static Node ParseExpression(MethodCallExpression e)
        {
            var parameters = e.Arguments.Select(Parse);
            if (e.Method.GetCustomAttributes(typeof(ExtensionAttribute), true).Any())
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
                    Container = e.Object == null ? new ConstantNode { Text = e.Method.DeclaringType.Name } : Parse(e.Object),
                    MemberName = e.Method.Name,
                    MemberValue = GetValue(e),
                    Parameters = parameters.ToList(),
                };
        }

        static Node ParseExpression(ConstantExpression e)
        {
            string value = GetValue(e);

            return new ConstantNode
            {
                Text = value
            };
        }

        static Node ParseExpression(MemberExpression e)
        {
            if (IsDisplayClass(e.Expression) || e.Expression == null)
            {
                return new ConstantNode
                {
                    Value = GetValue(e),
                    Text = e.Member.Name
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

        static Node ParseExpression(NewExpression e)
        {
            return new NewObjectNode
                {
                    Type = NameOfType(e.Type), 
                    Parameters = e.Arguments.Select(Parse).ToList(),
                    Value = GetValue(e)
                };
        }
        static Node ParseExpression(MemberInitExpression e)
        {
            return new MemberInitNode
                {
                    Constructor = (NewObjectNode)ParseExpression(e.NewExpression), 
                    Bindings = e.Bindings.Select(ParseExpression).ToList()
                };
        }

        static Node ParseExpression(MemberBinding e)
        {
            if (e is MemberAssignment)
            {
                return new MemberAssignmentNode { MemberName = e.Member.Name, Value = Parse(((MemberAssignment)e).Expression) };
            }
            return new ConstantNode() {Text = e.Member.Name};
        }

        private static Node ParseExpression(InvocationExpression e)
        {
            return new InvocationNode
            {
                Arguments = e.Arguments.Select(Parse),
                Expression = Parse(e.Expression)
            };
        }

        static string GetValue(Expression e)
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
            var s = FormatObject(value);
            return s + GetHints(e, value);
        }

        static string FormatTargetInvocationException(TargetInvocationException exception)
        {
            var i = exception.InnerException;
            return string.Format("{0}: {1}", i.GetType().Name, i.Message);
        }

        private static StringComparer GetComparerFromComparison(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    return StringComparer.CurrentCulture;
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
                default: throw new InvalidDataException("Unexpected StringComparison value.");
            }
        }

        static string GetHints(Expression e, object value)
        {
            if (value is bool && !(bool)value)
            {
                if (e is MethodCallExpression)
                {
                    var methE = (MethodCallExpression)e;
                    if (methE.Arguments.Count > 0)
                    {
                        if (methE.Object == null) // static method
                        {
                            if (methE.Arguments.Count >= 2)
                            {
                                var obj = DynamicInvoke(methE.Arguments[0]);
                                var arg = DynamicInvoke(methE.Arguments[1]);
                                if (methE.Method.Name == "SequenceEqual" && obj is IEnumerable && arg is IEnumerable)
                                {
                                    if (methE.Arguments.Count == 2) // TODO: equality comparer
                                    {
                                        var objEnum = (IEnumerable)obj;
                                        var argEnum = (IEnumerable)arg;

                                        var enumObj = objEnum.GetEnumerator();
                                        var enumArg = argEnum.GetEnumerator();
                                        {
                                            int i = 0;
                                            while (enumObj.MoveNext() && enumArg.MoveNext())
                                            {
                                                if (!Equals(enumObj.Current, enumArg.Current))
                                                {
                                                    return string.Format(", enumerables differ at index {0}, {1} != {2}", i,
                                                            FormatObject(enumObj.Current), FormatObject(enumArg.Current));
                                                }
                                                ++i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else // instance method
                        {
                            var obj = DynamicInvoke(methE.Object);
                            var arg = DynamicInvoke(methE.Arguments.First());
                            if (methE.Method.Name == "Equals")
                            {
                                if (obj is string && arg is string)
                                {
                                    var comparison = (StringComparison)(methE.Arguments.Select(DynamicInvoke).FirstOrDefault(x => x is StringComparison) ??
                                             StringComparison.CurrentCulture);

                                    return GetStringDifferHint((string)obj, (string)arg, GetComparerFromComparison(comparison));
                                }
                                else if (obj is IEnumerable && arg is IEnumerable)
                                {
                                    if (((IEnumerable)obj).Cast<object>().SequenceEqual(((IEnumerable)arg).Cast<object>()))
                                    {
                                        return ", but would have been True with .SequenceEqual()";
                                    }
                                }
                            }
                        }
                    }
                }

                if (e is BinaryExpression && e.NodeType == ExpressionType.Equal)
                {
                    var be = (BinaryExpression)e;
                    object left;
                    object right;
                    try
                    {
                        left = DynamicInvoke(be.Left);
                        right = DynamicInvoke(be.Right);
                    }
                    catch (TargetInvocationException exception)
                    {
                        return FormatTargetInvocationException(exception);
                    }
                    if (Equals(left, right))
                    {
                        return ", but would have been True with Equals()";
                    }
                    if (left is string && right is string)
                    {
                        if (((string)left).Equals((string)right, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return ", but would have been True if case-insensitive";
                        }

                        return GetStringDifferHint((string)left, (string)right, StringComparer.CurrentCulture);
                    }
                    if (left is IEnumerable && right is IEnumerable)
                    {
                        if (((IEnumerable)left).Cast<object>().SequenceEqual(((IEnumerable)right).Cast<object>()))
                        {
                            return ", but would have been True with .SequenceEqual()";
                        }
                    }
                }
            }
            return "";
        }

        static string GetStringDifferHint(string left, string right, StringComparer comparer)
        {
            if (left == null) return ", left is null";
            if (right == null) return ", right is null";

            for (int i = 1; i <= left.Length && i <= right.Length; ++i)
            {
                //TODO: this doesn't know about surrogate pairs or things like esszett
                if (!comparer.Equals(left.Substring(0, i), right.Substring(0, i)))
                    return string.Format(", strings differ at index {0}, '{1}' != '{2}'", i - 1,
                        left[i - 1], right[i - 1]);
            }

            return ""; //err: they don't differ!
        }

        static object DynamicInvoke(Expression e)
        {
            return Expression.Lambda(e).Compile().DynamicInvoke();
        }

        static string FormatObject(object value)
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
            if (value is IEnumerable)
            {
                var enumerable = (IEnumerable)value;
                var values = enumerable.Cast<object>().Select(FormatObject);
                //in case the enumerable is really long, let's cut off the end arbitrarily?
                const int Limit = 5;

                var list = enumerable as IList;
                var knownMax = list != null ? list.Count : default(int?);

                values = values.Take(Limit);
                if (values.Count() == Limit)
                {
                    values = values.Concat(new[] { knownMax.HasValue && knownMax > Limit ? string.Format("... ({0} total)", knownMax) : "..." });
                }
                return "[" + string.Join(", ", values.ToArray()) + "]";
            }
            return value.ToString();
        }
    }
}

