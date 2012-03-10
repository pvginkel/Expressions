using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    public class ExpressionContext : IExpressionContext
    {
        private static readonly Import[] EmptyImports = new Import[0];

        public object Owner { get; set; }

        public IList<Import> Imports { get; private set; }

        public bool IgnoreCase { get; private set; }

        public VariableCollection Variables { get; private set; }

        public ExpressionContext()
            : this(null)
        {
        }

        public ExpressionContext(IList<Import> imports)
            : this(imports, null)
        {
        }

        public ExpressionContext(IList<Import> imports, object owner)
            : this(imports, owner, true)
        {
        }

        public ExpressionContext(IList<Import> imports, object owner, bool ignoreCase)
        {
            Owner = owner;
            Imports = new ReadOnlyCollection<Import>(imports ?? EmptyImports);
            IgnoreCase = ignoreCase;

            Variables = new VariableCollection(
                ignoreCase
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal
            );
        }

        Type IBindingContext.OwnerType
        {
            get { return Owner == null ? null : Owner.GetType(); }
        }

        Type IBindingContext.GetVariableType(string variable, bool ignoreCase)
        {
            object value = ((IExecutionContext)this).GetVariableValue(variable, ignoreCase);

            return value == null ? null : value.GetType();
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
