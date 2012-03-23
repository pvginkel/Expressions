using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class Cast : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public TypeIdentifier Type { get; private set; }

        public Cast(IAstNode operand, TypeIdentifier type)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Cast(this);
        }

        public override string ToString()
        {
            return String.Format("Cast({0}, {1})", Operand, Type);
        }
    }
}
