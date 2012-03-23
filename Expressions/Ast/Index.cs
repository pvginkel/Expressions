using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class Index : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public AstNodeCollection Arguments { get; private set; }

        public Index(IAstNode operand, AstNodeCollection arguments)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(arguments, "arguments");

            Operand = operand;
            Arguments = arguments;
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Index(this);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Operand);
            sb.Append('[');

            for (int i = 0; i < Arguments.Nodes.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                sb.Append(Arguments.Nodes[i]);
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
