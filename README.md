#PowerAssert.NET

[![Build status](https://ci.appveyor.com/api/projects/status/pcj7c0cvh2xxbobg)](https://ci.appveyor.com/project/robfe/powerassert-net)

*Readable, Writable Test Assertions for .NET*

A .NET port of [Groovy's PowerAssert](http://dontmindthelanguage.wordpress.com/2009/12/11/groovy-1-7-power-assert/). It prints an easy-to-understand decomposition of your assertion
s expression tree (with values) whenever an assertion fails. 

## Examples:

### Seeing actual values

Given the following unit test:


    [Test]
    public void RunComplexExpression()
    {
       int x = 11;
       int y = 6;
       DateTime d = new DateTime(2010, 3, 1);
       PAssert.IsTrue(() => x + 5 == d.Month * y);
    }

PowerAssert will cause a failure with the following message:


    System.Exception : IsTrue failed, expression was:

    x + 5 == d.Month * y
    . .   __ . .   . . .
    . .   |  . .   . . .
    . |   |  . \_ _/ | .
    | |   |  .   |   | |
    | |   |  |   |   | 6
    | |   |  |   |   18
    | |   |  |   3
    | |   |  1/03/2010 12:00:00 a.m.
    | |   False
    | 16
    11

### Looking into collections
PowerAssert gives you insights into the contents of your collections under assertion:

Given the following unit test:

    [Test]
    [Ignore("This test will fail for demo purposes")]
    public void PrintingLinqExpressionStatements()
    {
        var list = Enumerable.Range(0, 150);
        PAssert.IsTrue(() => (from l in list where l % 2 == 0 select l).Sum() == 0);
    }

PowerAssert will cause a failure with the following message:

    System.Exception : IsTrue failed, expression was:

    list.Where(l => ((l % 2) == 0)).Sum() == 0
    .  . .   .                      . .   __
    .  . .   .                      \ /   |
    .  . \_ _/                       |    |
    \ _/   |                         |    |
     |     |                         |    False
     |     |                         5550
     |     [0, 2, 4, 6, 8, ...]
     [0, 1, 2, 3, 4, ...]


### Equals versus ==
Given the following unit test:

    [Test]
    public void EqualsButNotOperatorEquals()
    {
        var t1 = new Tuple<string>("foo");
        var t2 = new Tuple<string>("foo");

        PAssert.IsTrue(() => t1 == t2);
    }


Power Assert will cause a failure with the following message:


    System.Exception : IsTrue failed, expression was:

    t1 == t2
    .. __ ..
    __ |  __
    |  |  (foo)
    |  False, but would have been True with Equals()
    (foo)

### Equals versus SequenceEquals

Given the following unit test:

    [Test]
    [Ignore("This test will fail for demo purposes")]
    public void SequenceEqualButNotOperatorEquals()
    {
        object list = new List<int> { 1, 2, 3 };
        object array = new[] { 1, 2, 3 };
        PAssert.IsTrue(() => list == array);
    }

Power Assert will cause a failure with the following message:

    System.Exception : IsTrue failed, expression was:

    list == array
    .  . __ .   .
    \ _/ |  \_ _/
     |   |    [1, 2, 3]
     |   False, but would have been True with .SequenceEqual()
     [1, 2, 3]


## Contributing

Feature requests and PRs are welcomed!

Forked from http://powerassert.codeplex.com/SourceControl/changeset/8e1d4d6874e1
