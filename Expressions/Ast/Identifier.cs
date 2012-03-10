using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class Identifier : IAstNode
    {
        public string Name { get; private set; }

        public Identifier(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
