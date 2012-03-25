using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal enum ExpressionType
    {
        And,
        AndBoth,
        Or,
        OrBoth,
        Xor,
        Equals,
        Compares,
        NotEquals,
        NotCompares,
        Greater,
        GreaterOrEquals,
        Less,
        LessOrEquals,
        In,
        ShiftLeft,
        ShiftRight,
        Add,
        Subtract,
        Power,
        Multiply,
        Divide,
        Modulo,
        Plus,
        Minus,
        Not,
        Group,
        LogicalAnd,
        LogicalOr,
        LogicalNot,
        BitwiseAnd,
        BitwiseOr,
        BitwiseNot,
        Concat
    }
}
