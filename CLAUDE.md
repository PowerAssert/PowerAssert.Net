# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Is

PowerAssert.Net is a .NET port of Groovy's PowerAssert. When a test assertion fails, instead of just showing "expected X but got Y", it renders a visual decomposition of the entire expression tree with evaluated values at each sub-expression — making test failures self-diagnosing.

## Build and Test Commands

```powershell
# Build the solution
dotnet build PowerAssert.sln

# Run all tests
dotnet test PowerAssertTests/PowerAssertTests.csproj

# Run a single test (by name filter)
dotnet test PowerAssertTests/PowerAssertTests.csproj --filter "FullyQualifiedName~TestName"

# Run tests in a specific class
dotnet test PowerAssertTests/PowerAssertTests.csproj --filter "ClassName=PowerAssertTests.SomeTestClass"
```

The test project targets `net45`; the library targets `netcoreapp1.1`, `net40`, and `netstandard2.0`.

Tests use **NUnit 3** and **ApprovalTests** for snapshot-based output verification. When an approval test fails, it produces a `.received.txt` file alongside the `.approved.txt` file — diff and update the `.approved.txt` when the output change is intentional.

## Architecture

The core pipeline is: **Expression → ExpressionParser → Node Tree → NodeFormatter → ASCII art output**

### Entry Point: `PAssert` (`PowerAssert/PAssert.cs`)

Public API. `IsTrue(Expression<Func<bool>> expression)` is the main method. On failure it calls `RenderExpression()` → `ExpressionParser` → `NodeFormatter` to build the exception message.

Also exposes:
- `IsTrue<T>(T target, Expression<Func<T, bool>> expression)` — assertion with an explicit parameter
- `Throws<TException>(...)` — exception assertions
- `Poly()` — returns a `PolyAssert` for collecting multiple failures before throwing

### Expression Parser: `ExpressionParser` (`PowerAssert/Infrastructure/ExpressionParser.cs`)

Walks the C# expression tree (a `System.Linq.Expressions.Expression`) and builds a Node tree. Uses **C# dynamic dispatch** via overloaded `ParseExpression(dynamic e)` methods — one overload per expression node type (Binary, Member, MethodCall, etc.). Each overload evaluates the sub-expression's runtime value and wraps it in the appropriate Node subclass.

### Node Tree: `PowerAssert/Infrastructure/Nodes/`

Intermediate representation between expression tree and rendered output. Key types:
- `ConstantNode`, `BinaryNode`, `UnaryNode`, `MemberAccessNode`, `MethodCallNode`, `ArrayIndexNode`, `ConditionalNode`, `InvocationNode`, `NewObjectNode`, `NewArrayNode`, `MemberInitNode`, `ListInitNode`

Each node exposes a `Walk()` visitor method used by the formatter. Nodes carry both their source-text representation and their evaluated runtime value.

### Formatter: `NodeFormatter` (`PowerAssert/Infrastructure/NodeFormatter.cs`)

Converts the Node tree into the multi-line ASCII art output. It:
1. Lays out the expression as a single text line
2. Generates "stalk" connector lines (pipes, horizontal bars) linking each sub-expression position to its evaluated value
3. Reverses and filters the lines to produce final readable output

### Object Formatter: `ObjectFormatter` (`PowerAssert/Infrastructure/ObjectFormatter.cs`)

Converts runtime values to display strings. Handles nulls, primitives, strings, collections (first 5 items + count), exceptions, delegates, and types.

### Hint System: `PowerAssert/Hints/`

A plugin layer over the rendered output that provides actionable suggestions. Implements `IHint`; the `MultiHint` chains them. Examples:
- `StringEqualsHint` — highlights character-level string differences
- `SequenceEqualHint` — shows differences between sequences
- `FloatEqualityHint` — warns about float precision
- `EnumerableOperatorEqualsHint` — suggests `.SequenceEqual()` instead of `==`

### Multiple Assertions: `PolyAssert` (`PowerAssert/MultipleAssertions/PolyAssert.cs`)

`IDisposable`-based collector: wrap in a `using` block, call `.IsTrue()` / `.Try()` multiple times, and all failures are thrown together when the block exits. `StopIfErrorsHaveOccurred()` can force early exit.

## Adding a New Expression Type

1. Add a `ParseExpression(SomeExpressionType e)` overload in `ExpressionParser.cs`
2. Create a corresponding `Node` subclass in `Nodes/` if needed
3. Add approval tests in `PowerAssertTests/` to lock in the rendered output
