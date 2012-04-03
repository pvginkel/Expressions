using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Describes a variable used in an <see cref="ExpressionContext"/>.
    /// </summary>
    public sealed class Variable
    {
        /// <summary>
        /// Get the name of the variable.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get or set the value of the variable.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class with
        /// the specified name.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public Variable(string name)
        {
            Require.NotNull(name, "name");

            Name = name;
        }
    }
}
