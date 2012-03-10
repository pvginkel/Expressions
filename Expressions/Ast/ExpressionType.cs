using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal enum ExpressionType
    {
        And,
        Or,
        Xor,
        Equals,
        NotEquals,
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
        Not
    }
}
