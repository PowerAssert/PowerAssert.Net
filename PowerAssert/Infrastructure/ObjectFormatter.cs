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
#if NETCOREAPP1_1
            var isGenericType = new Func<Type, bool>(t => t.GetTypeInfo().IsGenericType);
#else
            var isGenericType = new Func<Type, bool>(t => t.IsGenericType);
#endif
            if (isGenericType(type) && type.GetGenericTypeDefinition() == typeof (KeyValuePair<,>))
            {
                var k = type.GetProperty("Key").GetValue(value, null);
                var v = type.GetProperty("Value").GetValue(value, null);
                return String.Format("{{{0}:{1}}}", FormatObject(k), FormatObject(v));
            }
            if (type.GetInterfaces()
                .Where(isGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IGrouping<,>)))
            {
                var k = type.GetProperty("Key").GetValue(value, null);
                return String.Format("{{{0}:{1}}}", FormatObject(k), FormatEnumerable(value));
            }
            if (value is Type)
            {
                return "typeof(" + ExpressionParser.NameOfType((Type) value) + ")";
            }
            if (value is Delegate)
            {
                var del = (Delegate) value;

#if NETCOREAPP1_1
                var method = RuntimeReflectionExtensions.GetMethodInfo(del);
#else
                var method = del.Method;
#endif

                return String.Format("delegate {0}, type: {2} ({1})", ExpressionParser.NameOfType(del.GetType()), String.Join(", ", method.GetParameters().Select(x => ExpressionParser.NameOfType(x.ParameterType))), ExpressionParser.NameOfType(method.ReturnType));
            }
            if (value is IEnumerable)
            {
                return FormatEnumerable(value);
            }
            return value.ToString();
        }

        internal static string FormatEnumerable(object value)
        {
            var enumerable = ((IEnumerable)value).Cast<object>();

            // In case the enumerable is really long, let's cut off the end arbitrarily?
            const int Limit = 5;
            const int LargeLimit = 1001;

            var list = enumerable as IList;
            var values = enumerable
                .Take(LargeLimit)
                .ToList();

            var knownMax = list != null
                ? list.Count
                : (values.Count == LargeLimit)
                    ? default(int?)
                    : values.Count;

            if (values.Count > Limit) {
                var suffixItem = knownMax.HasValue
                    ? (knownMax.Value == LargeLimit)
                        ? String.Format("... (at least {0})", knownMax.Value - 1)
                        : String.Format("... ({0} total)", knownMax.Value)
                    : "...";

                values = values
                    .Take(Limit)
                    .Concat(new[] {suffixItem})
                    .ToList();
            }

            return "[" + String.Join(", ", values.Select(FormatObject)) + "]";
        }
    }
}
