using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PowerAssert.Infrastructure
{
    public class ObjectFormatter
    {
        internal static string FormatTargetInvocationException(TargetInvocationException exception)
        {
            var i = exception.InnerException;
            return String.Format("(threw {0})", i.GetType().Name);
        }

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

            var exception = value as Exception;
            if (exception != null)
            {
                return "{" + exception.GetType().Name + "}";
            }

            var type = value.GetType();
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof (KeyValuePair<,>))
            {
                var k = type.GetTypeInfo().GetProperty("Key").GetValue(value, null);
                var v = type.GetTypeInfo().GetProperty("Value").GetValue(value, null);
                return String.Format("{{{0}:{1}}}", FormatObject(k), FormatObject(v));
            }
            if (type.GetTypeInfo().GetInterfaces()
                .Where(i => i.GetTypeInfo().IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IGrouping<,>)))
            {
                var k = type.GetTypeInfo().GetProperty("Key").GetValue(value, null);
                return String.Format("{{{0}:{1}}}", FormatObject(k), FormatEnumerable(value));
            }
            if (value is Type)
            {
                return "typeof(" + ExpressionParser.NameOfType((Type) value) + ")";
            }
            if (value is Delegate)
            {
                var del = (Delegate) value;

                return String.Format("delegate {0}, type: {2} ({1})", ExpressionParser.NameOfType(del.GetType()), String.Join(", ", del.GetMethodInfo().GetParameters().Select(x => ExpressionParser.NameOfType(x.ParameterType))), ExpressionParser.NameOfType(del.GetMethodInfo().ReturnType));
            }
            if (value is IEnumerable)
            {
                return FormatEnumerable(value);
            }
            return value.ToString();
        }

        internal static string FormatEnumerable(object value)
        {
            var enumerable = (IEnumerable) value;
            var values = enumerable.Cast<object>().Select(FormatObject);
            //in case the enumerable is really long, let's cut off the end arbitrarily?
            const int Limit = 5;

            var list = enumerable as IList;
            var knownMax = list != null ? list.Count : default(int?);

            values = values.Take(Limit);
            if (values.Count() == Limit)
            {
                values = values.Concat(new[] {knownMax.HasValue && knownMax > Limit ? String.Format("... ({0} total)", knownMax) : "..."});
            }
            return "[" + String.Join(", ", values.ToArray()) + "]";
        }
    }
}