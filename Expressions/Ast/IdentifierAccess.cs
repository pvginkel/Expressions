using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class IdentifierAccess : IAstNode
    {
        public string Name { get; private set; }

        public IdentifierAccess(string name)
        {
            Require.NotNull(name, "name");

            Name = name;
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.IdentifierAccess(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
