using System;
using System.Reflection;

namespace PowerAssert
{
	internal static class ReflectionShim
	{
		public static bool IsEnum(Type type)
		{
#if (NET40)
			return type.IsEnum;
#else
			return type.GetTypeInfo().IsEnum;
#endif
		}

		public static bool IsGenericType(Type type)
		{
#if (NET40)
			return type.IsGenericType;
#else
			return type.GetTypeInfo().IsGenericType;
#endif
		}

		public static Assembly Assembly(Type type)
		{
#if (NET40)
			return type.Assembly;
#else
			return type.GetTypeInfo().Assembly;
#endif
		}

		public static PropertyInfo[] GetProperties(Type type)
		{
#if (NET40)
			return type.GetProperties();
#else
			return type.GetTypeInfo().GetProperties();
#endif
		}

		public static PropertyInfo GetProperty(Type type, string name)
		{
#if (NET40)
			return type.GetProperty(name);
#else
			return type.GetTypeInfo().GetProperty(name);
#endif
		}

		public static Type[] GetInterfaces(Type type)
		{
#if (NET40)
			return type.GetInterfaces();
#else
			return type.GetTypeInfo().GetInterfaces();
#endif
		}

		public static Type GetGenericTypeDefinition(Type type)
		{
#if (NET40)
			return type.GetGenericTypeDefinition();
#else
			return type.GetTypeInfo().GetGenericTypeDefinition();
#endif
		}

		public static ParameterInfo[] GetMethodParameters(Delegate method)
		{
#if (NET40)
			return method.Method.GetParameters();
#else
			return method.GetMethodInfo().GetParameters();
#endif
		}

		public static Type MethodReturnType(Delegate method)
		{
#if (NET40)
			return method.Method.ReturnType;
#else
			return method.GetMethodInfo().ReturnType;
#endif
		}
	}
}
