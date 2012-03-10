using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public interface IBindingContext
    {
        Type OwnerType { get; }

        IList<Import> Imports { get; }

        Type GetVariableType(string variable, bool ignoreCase);
    }
}
