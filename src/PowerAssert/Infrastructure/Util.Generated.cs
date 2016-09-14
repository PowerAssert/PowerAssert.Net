using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace PowerAssert.Infrastructure
{
    internal class Util
    {
	    internal static readonly Dictionary<Type, string> Aliases = new Dictionary<Type, string>()
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(string), "string" },
            { typeof(bool), "bool" },
            { typeof(void), "void" },
        };

        internal static Dictionary<ExpressionType, string> BinaryOperators = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.AndAlso, "&&"},
            {ExpressionType.OrElse, "||"},
            {ExpressionType.Add, "+"},
            {ExpressionType.AddChecked, "+"},
            {ExpressionType.And, "&"},
			// AndAlso: The binary operator AndAlso is not defined for the types 'System.String' and 'System.String'.
			// ArrayLength: Unhandled binary: ArrayLength
			// ArrayIndex: Argument must be array
			// Call: Unhandled binary: Call
            {ExpressionType.Coalesce, "??"},
			// Conditional: Unhandled binary: Conditional
			// Constant: Unhandled binary: Constant
			// Convert: Unhandled binary: Convert
			// ConvertChecked: Unhandled binary: ConvertChecked
            {ExpressionType.Divide, "/"},
            {ExpressionType.Equal, "=="},
            {ExpressionType.ExclusiveOr, "^"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="},
			// Invoke: Unhandled binary: Invoke
			// Lambda: Unhandled binary: Lambda
            {ExpressionType.LeftShift, "<<"},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
			// ListInit: Unhandled binary: ListInit
			// MemberAccess: Unhandled binary: MemberAccess
			// MemberInit: Unhandled binary: MemberInit
            {ExpressionType.Modulo, "%"},
            {ExpressionType.Multiply, "*"},
            {ExpressionType.MultiplyChecked, "*"},
			// Negate: Unhandled binary: Negate
			// UnaryPlus: Unhandled binary: UnaryPlus
			// NegateChecked: Unhandled binary: NegateChecked
			// New: Unhandled binary: New
			// NewArrayInit: Unhandled binary: NewArrayInit
			// NewArrayBounds: Unhandled binary: NewArrayBounds
			// Not: Unhandled binary: Not
            {ExpressionType.NotEqual, "!="},
            {ExpressionType.Or, "|"},
			// OrElse: The binary operator OrElse is not defined for the types 'System.String' and 'System.String'.
			// Parameter: Unhandled binary: Parameter
            {ExpressionType.Power, "^"},
			// Quote: Unhandled binary: Quote
            {ExpressionType.RightShift, ">>"},
            {ExpressionType.Subtract, "-"},
            {ExpressionType.SubtractChecked, "-"},
			// TypeAs: Unhandled binary: TypeAs
			// TypeIs: Unhandled binary: TypeIs
            {ExpressionType.Assign, "="},
			// Block: Unhandled binary: Block
			// DebugInfo: Unhandled binary: DebugInfo
			// Decrement: Unhandled binary: Decrement
			// Dynamic: Unhandled binary: Dynamic
			// Default: Unhandled binary: Default
			// Extension: Unhandled binary: Extension
			// Goto: Unhandled binary: Goto
			// Increment: Unhandled binary: Increment
			// Index: Unhandled binary: Index
			// Label: Unhandled binary: Label
			// RuntimeVariables: Unhandled binary: RuntimeVariables
			// Loop: Unhandled binary: Loop
			// Switch: Unhandled binary: Switch
			// Throw: Unhandled binary: Throw
			// Try: Unhandled binary: Try
			// Unbox: Unhandled binary: Unbox
            {ExpressionType.AddAssign, "+="},
            {ExpressionType.AndAssign, "&="},
            {ExpressionType.DivideAssign, "/="},
            {ExpressionType.ExclusiveOrAssign, "^="},
            {ExpressionType.LeftShiftAssign, "<<="},
            {ExpressionType.ModuloAssign, "%="},
            {ExpressionType.MultiplyAssign, "*="},
            {ExpressionType.OrAssign, "|="},
            {ExpressionType.PowerAssign, "**="},
            {ExpressionType.RightShiftAssign, ">>="},
            {ExpressionType.SubtractAssign, "-="},
            {ExpressionType.AddAssignChecked, "+="},
            {ExpressionType.MultiplyAssignChecked, "*="},
            {ExpressionType.SubtractAssignChecked, "-="},
			// PreIncrementAssign: Unhandled binary: PreIncrementAssign
			// PreDecrementAssign: Unhandled binary: PreDecrementAssign
			// PostIncrementAssign: Unhandled binary: PostIncrementAssign
			// PostDecrementAssign: Unhandled binary: PostDecrementAssign
			// TypeEqual: Unhandled binary: TypeEqual
			// OnesComplement: Unhandled binary: OnesComplement
			// IsTrue: Unhandled binary: IsTrue
			// IsFalse: Unhandled binary: IsFalse
        };

		internal static Dictionary<ExpressionType, string> UnaryOperators = new Dictionary<ExpressionType, string>
        {
			// Add: Unhandled unary: Add
			// AddChecked: Unhandled unary: AddChecked
			// And: Unhandled unary: And
			// AndAlso: Unhandled unary: AndAlso
			// ArrayLength: Argument must be array
			// ArrayIndex: Unhandled unary: ArrayIndex
			// Call: Unhandled unary: Call
			// Coalesce: Unhandled unary: Coalesce
			// Conditional: Unhandled unary: Conditional
			// Constant: Unhandled unary: Constant
            {ExpressionType.Convert, "Convert"},
            {ExpressionType.ConvertChecked, "ConvertChecked"},
			// Divide: Unhandled unary: Divide
			// Equal: Unhandled unary: Equal
			// ExclusiveOr: Unhandled unary: ExclusiveOr
			// GreaterThan: Unhandled unary: GreaterThan
			// GreaterThanOrEqual: Unhandled unary: GreaterThanOrEqual
			// Invoke: Unhandled unary: Invoke
			// Lambda: Unhandled unary: Lambda
			// LeftShift: Unhandled unary: LeftShift
			// LessThan: Unhandled unary: LessThan
			// LessThanOrEqual: Unhandled unary: LessThanOrEqual
			// ListInit: Unhandled unary: ListInit
			// MemberAccess: Unhandled unary: MemberAccess
			// MemberInit: Unhandled unary: MemberInit
			// Modulo: Unhandled unary: Modulo
			// Multiply: Unhandled unary: Multiply
			// MultiplyChecked: Unhandled unary: MultiplyChecked
            {ExpressionType.Negate, "-"},
            {ExpressionType.UnaryPlus, "+"},
            {ExpressionType.NegateChecked, "-"},
			// New: Unhandled unary: New
			// NewArrayInit: Unhandled unary: NewArrayInit
			// NewArrayBounds: Unhandled unary: NewArrayBounds
			// Not: The unary operator Not is not defined for the type 'System.Double'.
			// NotEqual: Unhandled unary: NotEqual
			// Or: Unhandled unary: Or
			// OrElse: Unhandled unary: OrElse
			// Parameter: Unhandled unary: Parameter
			// Power: Unhandled unary: Power
			// Quote: Quoted expression must be a lambda
			// RightShift: Unhandled unary: RightShift
			// Subtract: Unhandled unary: Subtract
			// SubtractChecked: Unhandled unary: SubtractChecked
			// TypeAs: The type used in TypeAs Expression must be of reference or nullable type, System.Double is neither
			// TypeIs: Unhandled unary: TypeIs
			// Assign: Unhandled unary: Assign
			// Block: Unhandled unary: Block
			// DebugInfo: Unhandled unary: DebugInfo
            {ExpressionType.Decrement, "Decrement"},
			// Dynamic: Unhandled unary: Dynamic
			// Default: Unhandled unary: Default
			// Extension: Unhandled unary: Extension
			// Goto: Unhandled unary: Goto
            {ExpressionType.Increment, "Increment"},
			// Index: Unhandled unary: Index
			// Label: Unhandled unary: Label
			// RuntimeVariables: Unhandled unary: RuntimeVariables
			// Loop: Unhandled unary: Loop
			// Switch: Unhandled unary: Switch
			// Throw: Argument must not have a value type.
			// Try: Unhandled unary: Try
			// Unbox: Can only unbox from an object or interface type to a value type.
			// AddAssign: Unhandled unary: AddAssign
			// AndAssign: Unhandled unary: AndAssign
			// DivideAssign: Unhandled unary: DivideAssign
			// ExclusiveOrAssign: Unhandled unary: ExclusiveOrAssign
			// LeftShiftAssign: Unhandled unary: LeftShiftAssign
			// ModuloAssign: Unhandled unary: ModuloAssign
			// MultiplyAssign: Unhandled unary: MultiplyAssign
			// OrAssign: Unhandled unary: OrAssign
			// PowerAssign: Unhandled unary: PowerAssign
			// RightShiftAssign: Unhandled unary: RightShiftAssign
			// SubtractAssign: Unhandled unary: SubtractAssign
			// AddAssignChecked: Unhandled unary: AddAssignChecked
			// MultiplyAssignChecked: Unhandled unary: MultiplyAssignChecked
			// SubtractAssignChecked: Unhandled unary: SubtractAssignChecked
            {ExpressionType.PreIncrementAssign, "++"},
            {ExpressionType.PreDecrementAssign, "--"},
            {ExpressionType.PostIncrementAssign, "PostIncrementAssign(x++"},
            {ExpressionType.PostDecrementAssign, "PostDecrementAssign(x--"},
			// TypeEqual: Unhandled unary: TypeEqual
			// OnesComplement: The unary operator OnesComplement is not defined for the type 'System.Double'.
			// IsTrue: The unary operator IsTrue is not defined for the type 'System.Double'.
			// IsFalse: The unary operator IsFalse is not defined for the type 'System.Double'.
        };
    }
}

