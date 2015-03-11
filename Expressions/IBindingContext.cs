﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Represents the binding context supplied when binding an expression.
    /// </summary>
    public interface IBindingContext
    {
        /// <summary>
        /// Get the type of the owner of the expression or null when
        /// the expression does not have an owner.
        /// </summary>
        Type OwnerType { get; }

        /// <summary>
        /// Get the imports of the expression.
        /// </summary>
        IList<Import> Imports { get; }

        /// <summary>
        /// Get the type of a variable of the expression or null when the
        /// variable doesn't exist.
        /// </summary>
        /// <param name="variable">The name of the variable.</param>
        /// <param name="ignoreCase">True to ignore case when resolving the variable; otherwise false.</param>
        /// <returns></returns>
        Type GetVariableType(string variable, bool ignoreCase);

        /// <summary>
        /// Event which will be raised in case a variable type cannot be defined from the binding context
        /// </summary>
        event ResolveVariableTypeHandler ResolveVariableType;
    }

    /// <summary>
    /// Delegate for the ResolveVariableType event.
    /// </summary>
    /// <param name="variable">The name of the variable.</param>
    /// <param name="ignoreCase">True to ignore case when resolving the variable; otherwise false.</param>
    /// <returns></returns>
    public delegate Type ResolveVariableTypeHandler(string variable, bool ignoreCase);

}
