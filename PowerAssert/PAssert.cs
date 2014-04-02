using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using PowerAssert.Hints;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert
{
    public class Configuration
    {
        public Configuration()
        {
            SetDefaultHints();
        }

        void SetDefaultHints()
        {
            SetHints(new MethodEqualsInsteadOfOperatorEqualsHint()
                , new StringOperatorEqualsHint()
                , new EnumerableOperatorEqualsHint()
                , new SequenceEqualHint()
                , new DelegateShouldHaveBeenInvokedEqualsHint()
                , new StringEqualsHint()
                , new EnumerableEqualsHint()
                , new FloatEqualityHint()
                , new BrokenEqualityHint()
                , new TimeSpanTotalMistakesHint()
                );
        }

        internal IHint Hinter { get; private set; }
        public void SetHints(params IHint[] hints)
        {
            if (hints.Length == 1) Hinter = hints[0];
            Hinter = new MultiHint(hints);
        }

        public void SetHints(IHint hint)
        {
            Hinter = hint;
        }
    }

    public static class PAssert
    {
        static PAssert()
        {
            Config = new Configuration();
        }

        public static Configuration Config { get; set; }

        public static TException Throws<TException>(Action a) where TException : Exception
        {
            try
            {
                a();
            }
            catch (TException exception)
            {
                return exception;
            }

            throw new Exception("An exception of type " + typeof(TException).Name + " was expected, but no exception occured");
        }

        public static void IsTrue(Expression<Func<bool>> expression)
        {
            // TODO: see if we can simplify the expression to False

            Func<bool> func = expression.Compile();
            if (!func())
            {
                var method = new StackFrame(1, false).GetMethod();
                _currentTestClass = method != null ? method.DeclaringType : null;
                throw CreateException(expression, "IsTrue failed");
            }
        }

        static Exception CreateException(Expression<Func<bool>> expression, string message)
        {
            Node constantNode = Node.Parse(expression.Body);

            var successful = new List<Alternative>();
            foreach (var alternative in constantNode.EnumerateAlternatives(Config.Hinter))
            {
                if ((bool) Expression.Lambda(alternative.Expression).Compile().DynamicInvoke())
                {
                    successful.Add(alternative);
                }
            }

            string[] lines = NodeFormatter.Format(constantNode);
            string nl = Environment.NewLine;
            string exceptionMessage = string.Format("{0}, expression was:{1}{1}{2}", message, nl, String.Join(nl, lines));

            if (successful.Any())
            {
                exceptionMessage += string.Format("{0}{0}The following {1} alternatives would have made the assertion true:", nl, successful.Count);
                for (int index = 0; index < successful.Count; index++)
                {
                    var alternative = successful[index];
                    var node = Node.Parse(alternative.Expression);
                    string[] furtherlines = NodeFormatter.Format(node);
                    exceptionMessage += string.Format("{0}{0}{1}) {3}{0}{0}  {2}", nl, (1 + index), string.Join(nl + "  ", furtherlines), alternative.Alteration);
                }
            }

            return new Exception(exceptionMessage);
        }

        [ThreadStatic]
        private static Type _currentTestClass;

        internal static Type CurrentTestClass
        {
            get { return _currentTestClass; }
        }
    }
}
