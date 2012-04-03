using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Provides a default implementation of the <see cref="IExpressionContext"/>
    /// interface.
    /// </summary>
    public class ExpressionContext : IExpressionContext
    {
        /// <summary>
        /// Get the owner of the expression or null when the expression
        /// does not have an owner.
        /// </summary>
        public object Owner { get; set; }

        /// <summary>
        /// Get the imports of the expression.
        /// </summary>
        public IList<Import> Imports { get; private set; }

        /// <summary>
        /// Get whether to ignore case.
        /// </summary>
        public bool IgnoreCase { get; private set; }

        /// <summary>
        /// Get the variables associated with the expression context.
        /// </summary>
        public VariableCollection Variables { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionContext"/>
        /// class.
        /// </summary>
        public ExpressionContext()
            : this(null, null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionContext"/>
        /// class with the specified imports.
        /// </summary>
        /// <param name="imports">The imports of the expression context.</param>
        public ExpressionContext(IEnumerable<Import> imports)
            : this(imports, null, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionContext"/>
        /// class with the specified imports and owner.
        /// </summary>
        /// <param name="imports">The imports of the expression context.</param>
        /// <param name="owner">The owner of the expression context.</param>
        public ExpressionContext(IEnumerable<Import> imports, object owner)
            : this(imports, owner, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionContext"/>
        /// class with the specified imports, owner and whether to ignore case.
        /// </summary>
        /// <param name="imports">The imports of the expression context.</param>
        /// <param name="owner">The owner of the expression context.</param>
        /// <param name="ignoreCase">True to ignore case; otherwise false.</param>
        public ExpressionContext(IEnumerable<Import> imports, object owner, bool ignoreCase)
        {
            Owner = owner;
            IgnoreCase = ignoreCase;

            Variables = new VariableCollection(
                ignoreCase
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal
            );

            if (imports != null)
                Imports = new List<Import>(imports);
            else
                Imports = new List<Import>();
        }

        Type IBindingContext.OwnerType
        {
            get { return Owner == null ? null : Owner.GetType(); }
        }

        Type IBindingContext.GetVariableType(string variable, bool ignoreCase)
        {
            if (Variables.Contains(variable))
            {
                object value = ((IExecutionContext)this).GetVariableValue(variable, ignoreCase);

                return value == null ? typeof(object) : value.GetType();
            }
            else
            {
                return null;
            }
        }

        object IExecutionContext.GetVariableValue(string variable, bool ignoreCase)
        {
            if (Variables.Contains(variable))
                return Variables[variable].Value;
            else
                return null;
        }
    }
}
