using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Represents the execution context supplied when executing a bound expression.
    /// </summary>
    public interface IExecutionContext
    {
        /// <summary>
        /// Get the owner of the expression or null when the expression
        /// does not have an owner.
        /// </summary>
        object Owner { get; }

        /// <summary>
        /// Get the value of a variable of the expression or null when
        /// the variable does not exist.
        /// </summary>
        /// <param name="variable">The name of the variable to get the value for.</param>
        /// <param name="ignoreCase">True to ignore case when resolving the variable; otherwise false.</param>
        /// <returns></returns>
        object GetVariableValue(string variable, bool ignoreCase);
    }
}
