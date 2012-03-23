using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class UnaryExpression : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public ExpressionType Type { get; private set; }

        public UnaryExpression(IAstNode operand, ExpressionType type)
        {
            Require.NotNull(operand, "operand");

            Operand = operand;
            Type = type;
        }

        public override string ToString()
        {
            return String.Format("({0} {1})", Type, Operand);
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.UnaryExpression(this);
        }
    }
}
