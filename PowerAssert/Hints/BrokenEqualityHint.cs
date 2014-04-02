using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Hints
{
    class BrokenEqualityHint : IHint
    {
        private static readonly MethodInfo ObjectEqualsMethodInfo = typeof(object).GetMethod("Equals", BindingFlags.Instance | BindingFlags.Public);

        public bool TryGetHint(Expression expression, out string hint)
        {
            object left = null;
            object right = null;
            Type declaringType = null;

            var methE = expression as MethodCallExpression;
            if (methE != null)
            {
                if (methE.Method == ObjectEqualsMethodInfo)
                {
                    left = Node.DynamicInvoke(methE.Object);
                    right = Node.DynamicInvoke(methE.Arguments[0]);

                    // methE.Method.DeclaringType doesn't work here, it will
                    // always return the base class method
                    declaringType = GetDerivedMethodInfo(methE.Object.Type, methE.Method).DeclaringType;
                }
            }

            var binE = expression as BinaryExpression;
            if (binE != null && binE.NodeType == ExpressionType.Equal)
            {
                left = Node.DynamicInvoke(binE.Left);
                right = Node.DynamicInvoke(binE.Right);

                if (binE.Method != null)
                    declaringType = binE.Method.DeclaringType;
                else 
                    declaringType = typeof (object); // this should never happen - the hints are only called when the assert fails
            }

            if (left != null && left == right)
            {
                hint = ", type " + Node.NameOfType(declaringType) + " has a broken equality implementation (both sides are the same object)";
                return true;
            }

            hint = null;
            return false;
        }

        // this can be improved
        private static MethodInfo GetDerivedMethodInfo(Type derivedType, MethodInfo baseMethod)
        {
            return derivedType.GetMethod(baseMethod.Name,
                baseMethod.GetParameters().Select(x => x.ParameterType).ToArray());
        }
    }
}