using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public enum ExpressionsExceptionType
    {
        SyntaxError,
        ConstantOverflow,
        TypeMismatch,
        UndefinedName,
        FunctionHasNoReturnValue,
        InvalidExplicitCast,
        AmbiguousMatch,
        AccessDenied,
        InvalidFormat,
        UnresolvedMethod
    }
}
