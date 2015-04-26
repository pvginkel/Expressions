using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public class ResolveVariableValueEventArgs : EventArgs
    {
        public Object Result { get; set; }

        public string Variable { get; private set; }

        public bool IgnoreCase { get; private set; }

        public ResolveVariableValueEventArgs(string variable, bool ignoreCase)
        {
            Variable = variable;
            IgnoreCase = ignoreCase;
        }
    }

    /// <summary>
    /// Delegate for the ResolveVariableValue event.
    /// </summary>
    /// <param name="variable">The name of the variable.</param>
    /// <param name="ignoreCase">True to ignore case when resolving the variable; otherwise false.</param>
    /// <returns></returns>
    public delegate void ResolveVariableValueEventHandler(object sender, ResolveVariableValueEventArgs e);
}
