using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using ApprovalTests.Utilities;
using NUnit.Framework;
using PowerAssert;

namespace PowerAssertTests.Approvals
{

    [TestFixture]
    public class EndToEndTest
    {
        [Test]
        public void TestDifferingLists()
        {
            var x = new List<int> { 1, 2, 3, 4, 5, 6 };
            var y = new List<int> { 1, 2, 3, 4, 5, 7 };

            ApproveException(() => x.SequenceEqual(y));
        }

        [Test]
        public void RunComplexExpression()
        {
            int x = 11;
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Utc);
            ApproveException(() => x + 5 == d.Month * y);
        }

        static int field = 11;
        [Test]
        public void RunComplexExpressionWithStaticField()
        {
            int y = 6;
            DateTime d = new DateTime(2010, 3, 1, 0, 0, 0, DateTimeKind.Utc);
            ApproveException(() => field + 5 == d.Month * y);
        }

        [Test]
        public void RunComplexExpression2()
        {
            string x = " lalalaa ";
            int i = 10;
            ApproveException(() => x.Trim().Length == Math.Max(4, new int[] { 5, 4, i / 3, 2 }[0]));
        }

        [Test]
        public void RunComplexExpression3()
        {
            List<int> l = new List<int> { 1, 2, 3 };
            bool b = false;
            ApproveException(() => l[2].ToString() == (b ? "three" : "four"));
        }

        [Test]
        public void RunStringCompare()
        {
            string s = "hello, bobby";
            Tuple<string> t = new Tuple<string>("hello, Bobby");
            ApproveException(() => s == t.Item1);
        }

        [Test]
        public void RunRoundingEdgeCase()
        {
            double d = 3;
            int i = 2;


            ApproveException(() => 4.5 == d + 3 / i);
        }

        [Test]
        public void EnumerablesThatDiffer()
        {
            var s1 = "hello1";
            var s2 = "hello2";

            ApproveException(() => s1.SequenceEqual(s2));
        }

        [Test]
        public void StringsThatDiffer()
        {
            var s1 = "hello1";
            var s2 = "hello2";

            ApproveException(() => s1.Equals(s2));
        }

        [Test]
        public void StringsThatDifferAndAreComparedCaseInsensitively()
        {
            var s1 = "Hello1";
            var s2 = "hello2";

            ApproveException(() => s1.Equals(s2, StringComparison.OrdinalIgnoreCase));
        }


        [Test]
        public void EqualsButNotOperatorEquals()
        {
            var t1 = new Tuple<string>("foo");
            var t2 = new Tuple<string>("foo");

            ApproveException(() => t1 == t2);
        }

        [Test]
        public void SequenceEqualButNotOperatorEquals()
        {
            object list = new List<int> { 1, 2, 3 };
            object array = new[] { 1, 2, 3 };
            ApproveException(() => list == array);
        }

        [Test]
        public void SequenceEqualButNotDotEquals()
        {
            object list = new List<int> { 1, 2, 3 };
            object array = new[] { 1, 2, 3 };
            ApproveException(() => list.Equals(array));
        }

        [Test]
        public void PrintingLinqStatements()
        {
            var list = Enumerable.Range(0, 150);
            ApproveException(() => list.Where(x => x % 2 == 0).Sum() == 0);
        }

        [Test]
        public void PrintingLinqExpressionStatements()
        {
            var list = Enumerable.Range(0, 150);
            ApproveException(() => (from l in list where l % 2 == 0 select l).Sum() == 0);
        }

        [Test]
        public void PrintingComplexLinqExpressionStatements()
        {
            var list = Enumerable.Range(0, 5);
            ApproveException(() => (from x in list from y in list where x > y select x + "," + y).Count() == 0);
        }

        [Test]
        public void PrintingEnumerablesWithNulls()
        {
            var list = new List<int?> { 1, 2, null, 4, 5 };
            ApproveException(() => list.Sum() == null);
        }

        [Test]
        public void PrintingUnaryNot()
        {
            var b = true;
            ApproveException(() => !b);
        }

        [Test]
        public void PrintingUnaryNegate()
        {
            var b = 5;
            ApproveException(() => -b == 0);
        }

        [Test]
        public void PrintingIsTest()
        {
            var b = new object();
            ApproveException(() => b is string);
        }

        [Test]
        public void PrintingNewExpression()
        {
            ApproveException(() => new List<string>() == null);
        }

        [Test]
        public void PrintingDictionary()
        {
            var dictionary = new Dictionary<string, string>
                {
                    {"foo", "bar"},
                    {"foo2", "bar2"}
                };
            ApproveException(() => dictionary == null);
        }

        [Test]
        public void PrintingMethodCall()
        {
            var a = 4;
            ApproveException(() => (a * 5).Equals((a + 5)));
        }

        [Test]
        public void CompareDelegateAndObject()
        {
            var x = 1;
            Func<int> f = () => 1;

            ApproveException(() => Equals(f, x));
        }

        [Test]
        public void CompareTwoCloseFloats()
        {
            double d = 0.1;
            float f = (float)d * 100;
            f /= 100;

            ApproveException(() => d == f);
        }

        [Test]
        public void StringContainsControlChar()
        {
            var l = "hello";
            var r = "hell\u0009o";

            ApproveException(() => l == r);
        }

        [Test]
        public void StringContainsFormatChar()
        {
            var l = "hello";
            var r = "hell\u200Co"; //ZWNJ

            ApproveException(() => l == r);
        }

        class BrokenClass
        {
            public override bool Equals(object obj)
            {
                return false;
            }
        }

        [Test]
        public void ShouldHaveUsedTotal()
        {
            var x = TimeSpan.FromSeconds(100);

            ApproveException(() => x.Seconds == 100);
        }


        [Test]
        public void BrokenEqualityTestInstanceEquals()
        {
            var x = new BrokenClass();

            ApproveException(() => x.Equals(x));
        }



        [Test]
        public void StringContainsMismatchedNewlines()
        {
            //todo: it'd be good for powerassert to escape special characters in strings
            var l = "hell\r\no";
            var r = "hell\no";

            ApproveException(() => l == r);
        }

        [Test]
        public void OneStringIsDecomposedVersionOfOther()
        {
            var l = "hellö".Normalize(NormalizationForm.FormC);
            var r = "hellö".Normalize(NormalizationForm.FormD);

            ApproveException(() => l == r);
        }

        [Test]
        public void NullDereference()
        {
            string l = null;

            ApproveException(() => l.ToUpperInvariant() == "Oops");
        }

        [Test]
        public void Casting()
        {
            int x = 5;
            ApproveException(() => (long)x == 10L);
        }

        [Test]
        public void ArrayLength()
        {
            int[] values = new int[0];
            ApproveException(() => values.Length == 1);
        }

        [Test]
        public void Enum()
        {
            var o = FileOptions.SequentialScan;
            ApproveException(() => o == FileOptions.Encrypted);
        }

        [Test]
        public void EnumBackwards()
        {
            var o = FileOptions.SequentialScan;
            ApproveException(() => FileOptions.Encrypted == o);
        }

        string _expected = "bar";

        [Test]
        public void PrintingTestClassFields()
        {
            ApproveException(() => this._expected == "foo");
        }

        void ApproveException(Expression<Func<bool>> func)
        {
            try
            {
                PAssert.IsTrue(func);
                Assert.Fail("No PowerAssertion");
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e);
                ApprovalTests.Approvals.Verify(DosifyLineEndings(e.ToString()), x => StackTraceScrubber.ScrubPaths(StackTraceScrubber.ScrubLineNumbers(x)));
            }
        }

        static readonly Regex Newline = new Regex(@"\r?\n", RegexOptions.Compiled);

        static string DosifyLineEndings(string text)
        {
            return Newline.Replace(text, "\r\n");
        }
    }
}
