using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public class ResolveVariableTypeEventArgs : EventArgs
    {
        public Type Result { get; set; }

        public string Variable { get; private set; }

        public bool IgnoreCase { get; private set; }

        public ResolveVariableTypeEventArgs(string variable, bool ignoreCase)
        {
            Variable = variable;
            IgnoreCase = ignoreCase;
        }
    }

    /// <summary>
    /// Delegate for the ResolveVariableType event.
    /// </summary>
    /// <param name="variable">The name of the variable.</param>
    /// <param name="ignoreCase">True to ignore case when resolving the variable; otherwise false.</param>
    /// <returns></returns>
    public delegate void ResolveVariableTypeEventHandler(object sender, ResolveVariableTypeEventArgs e);
}
