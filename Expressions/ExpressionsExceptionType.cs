using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Type of an ExpressionsException.
    /// </summary>
    public enum ExpressionsExceptionType
    {
        /// <summary>
        /// Exception is because of a syntax error.
        /// </summary>
        SyntaxError,
        /// <summary>
        /// Exception is because of a constant overflow during parsing.
        /// </summary>
        ConstantOverflow,
        /// <summary>
        /// Exception is because of a type mismatch in a method call.
        /// </summary>
        TypeMismatch,
        /// <summary>
        /// Exception is because of an undefined name.
        /// </summary>
        UndefinedName,
        /// <summary>
        /// Exception is because a function was used that did not have a return value.
        /// </summary>
        FunctionHasNoReturnValue,
        /// <summary>
        /// Exception is because an invalid cast was made.
        /// </summary>
        InvalidExplicitCast,
        /// <summary>
        /// Exception is because of a method overload could not be matched unambiguously.
        /// </summary>
        AmbiguousMatch,
        /// <summary>
        /// Exception is because of access is denied.
        /// </summary>
        AccessDenied,
        /// <summary>
        /// Exception is because of an invalid format.
        /// </summary>
        InvalidFormat,
        /// <summary>
        /// Exception is because a method could not be resolved.
        /// </summary>
        UnresolvedMethod
    }
}
