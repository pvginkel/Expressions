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
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }
    }
}
