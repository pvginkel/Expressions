using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public interface IExecutionContext
    {
        object Owner { get; }

        object GetVariableValue(string variable, bool ignoreCase);
    }
}
