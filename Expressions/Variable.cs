using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public sealed class Variable
    {
        public string Name { get; private set; }

        public object Value { get; set; }

        public Variable(string name)
        {
            Require.NotNull(name, "name");

            Name = name;
        }
    }
}
