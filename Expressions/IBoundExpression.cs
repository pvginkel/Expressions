using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Represents a bound expression.
    /// </summary>
    public interface IBoundExpression
    {
        /// <summary>
        /// Invokes the expression.
        /// </summary>
        /// <returns>The result of the expression.</returns>
        object Invoke();

        /// <summary>
        /// Invokes the expression.
        /// </summary>
        /// <param name="executionContext">The execution context used to
        /// execute the expression.</param>
        /// <returns>The result of the expression.</returns>
        object Invoke(IExecutionContext executionContext);
    }

    /// <summary>
    /// Represents a bound expression.
    /// </summary>
    /// <typeparam name="T">The type of the result of the expression.</typeparam>
    public interface IBoundExpression<out T> : IBoundExpression
    {
        /// <summary>
        /// Invokes the expression.
        /// </summary>
        /// <returns>The result of the expression.</returns>
        new T Invoke();

        /// <summary>
        /// Invokes the expression.
        /// </summary>
        /// <param name="executionContext">The execution context used to
        /// execute the expression.</param>
        /// <returns>The result of the expression.</returns>
        new T Invoke(IExecutionContext executionContext);
    }
}
