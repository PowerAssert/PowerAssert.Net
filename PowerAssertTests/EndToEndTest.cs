using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using PowerAssert;
using PowerAssert.Infrastructure;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssertTests
{

    [TestFixture]
    public class EndToEndTest
    {
        [Test]
        public void PrintResults()
        {
            int x = 11;
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1);
            Expression<Func<bool>> expression = () => x + 5 == d.Month * y;
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }

        [Test]
        public void PrintResults_for_a_MethodInvokeExpression()
        {
            Func<int, double, double> f = (A, X) =>
                A * Math.Log(X);

            int a = 11;
            double x = 1.234;
            Expression<Func<bool>> expression = () => f(a, x) == 5.678;
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }

        [Test]
        public void PrintResults_for_a_MethodInvokeExpression_on_an_Expression()
        {
            Expression<Func<int, double, double>> f = (A, X) =>
                A * Math.Log(X);

            int a = 11;
            double x = 1.234;
            Expression<Func<bool>> expression = () => f.Compile()(a, x) == 5.678;
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }
        
        [Test]
        public void PrintResultsForAction()
        {
            Func<Action<string>, bool> foo = act => { act("s"); return false; };
            Action<string> x = k => {};
            Expression<Func<bool>> expression = () => foo(x);
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }


        [Test]
        public void PrintResultsForNewObject()
        {

            Expression<Func<bool>> expression = () => new List<string>(5).Count == 0;
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }
        [Test]
        public void PrintResultsForNewObjectWithInitialiser()
        {
            var ten = 10;
            Expression<Func<bool>> expression = () => new List<string>(5) { Capacity = ten }.Capacity == 10;
            Node constantNode = ExpressionParser.Parse(expression.Body);
            string[] strings = NodeFormatter.Format(constantNode);
            string s = string.Join(Environment.NewLine, strings);
            Console.Out.WriteLine(s);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void TestDifferingLists()
        {
            var x = new List<int> { 1, 2, 3, 4, 5, 6 };
            var y = new List<int> { 1, 2, 3, 4, 5, 7 };

            PAssert.IsTrue(() => x.SequenceEqual(y));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpression()
        {
            int x = 11;
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1);
            PAssert.IsTrue(() => x + 5 == d.Month * y);
        }

        static int field = 11;
        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpressionWithStaticField()
        {
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1);
            PAssert.IsTrue(() => field + 5 == d.Month * y);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpression2()
        {
            string x = " lalalaa ";
            int i = 10;
            PAssert.IsTrue(() => x.Trim().Length == Math.Max(4, new int[] { 5, 4, i / 3, 2 }[0]));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunComplexExpression3()
        {
            List<int> l = new List<int> { 1, 2, 3 };
            bool b = false;
            PAssert.IsTrue(() => l[2].ToString() == (b ? "three" : "four"));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunStringCompare()
        {
            string s = "hello, bobby";
            Tuple<string> t = new Tuple<string>("hello, Bobby");
            PAssert.IsTrue(() => s == t.Item1);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void RunRoundingEdgeCase()
        {
            double d = 3;
            int i = 2;


            PAssert.IsTrue(() => 4.5 == d + 3 / i);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void EnumerablesThatDiffer()
        {
            var s1 = "hello1";
            var s2 = "hello2";

            PAssert.IsTrue(() => s1.SequenceEqual(s2));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void StringsThatDiffer()
        {
            var s1 = "hello1";
            var s2 = "hello2";

            PAssert.IsTrue(() => s1.Equals(s2));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void StringsThatDifferAndAreComparedCaseInsensitively()
        {
            var s1 = "Hello1";
            var s2 = "hello2";

            PAssert.IsTrue(() => s1.Equals(s2, StringComparison.OrdinalIgnoreCase));
        }


        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void EqualsButNotOperatorEquals()
        {
            var t1 = new Tuple<string>("foo");
            var t2 = new Tuple<string>("foo");

            PAssert.IsTrue(() => t1 == t2);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void SequenceEqualButNotOperatorEquals()
        {
            object list = new List<int> { 1, 2, 3 };
            object array = new[] { 1, 2, 3 };
            PAssert.IsTrue(() => list == array);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void SequenceEqualButNotDotEquals()
        {
            object list = new List<int> { 1, 2, 3 };
            object array = new[] { 1, 2, 3 };
            PAssert.IsTrue(() => list.Equals(array));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingLinqStatements()
        {
            var list = Enumerable.Range(0, 150);
            PAssert.IsTrue(() => list.Where(x => x % 2 == 0).Sum() == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingLinqExpressionStatements()
        {
            var list = Enumerable.Range(0, 150);
            PAssert.IsTrue(() => (from l in list where l % 2 == 0 select l).Sum() == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingComplexLinqExpressionStatements()
        {
            var list = Enumerable.Range(0, 5);
            PAssert.IsTrue(() => (from x in list from y in list where x > y select x + "," + y).Count() == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingEnumerablesWithNulls()
        {
            var list = new List<int?> { 1, 2, null, 4, 5 };
            PAssert.IsTrue(() => list.Sum() == null);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingUnaryNot()
        {
            var b = true;
            PAssert.IsTrue(() => !b);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingUnaryNegate()
        {
            var b = 5;
            PAssert.IsTrue(() => -b == 0);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingIsTest()
        {
            var b = new object();
            PAssert.IsTrue(() => b is string);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingNewExpression()
        {
            PAssert.IsTrue(() => new List<string>() == null);
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingDictionary()
        {
            var dictionary = new Dictionary<string, string>
                {
                    {"foo", "bar"},
                    {"foo2", "bar2"}
                };
            PAssert.IsTrue(() => dictionary == null);
        }
        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingMethodCall()
        {
            var a = 4;
            PAssert.IsTrue(() => (a * 5).Equals((a + 5)));
        }

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void CompareDelegateAndObject()
        {
            var x = 1;
            Func<int> f = () => 1;

            PAssert.IsTrue(() => Equals(f, x));
        }

        string _expected = "bar";

        [Test]
        [Ignore("This test will fail for demo purposes")]
        public void PrintingTestClassFields()
        {
            PAssert.IsTrue(() => this._expected == "foo");
        }

    }
}
