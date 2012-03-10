using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public interface IExpressionBinder
    {
        Type GetOwnerType();

        Type GetVariableType(string identifier);

        Import[] GetImports();
    }
}
