using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    public class ExpressionContext : IExpressionContext
    {
        public object Owner { get; set; }

        public IList<Import> Imports { get; private set; }

        public bool IgnoreCase { get; set; }

        public VariableCollection Variables { get; private set; }

        public ExpressionContext()
            : this(null, null, true)
        {
        }

        public ExpressionContext(IEnumerable<Import> imports)
            : this(imports, null, true)
        {
        }

        public ExpressionContext(IEnumerable<Import> imports, object owner)
            : this(imports, owner, true)
        {
        }

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
