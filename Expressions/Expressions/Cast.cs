using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class Cast : IExpression
    {
        public IExpression Operand { get; private set; }

        public Type Type { get; private set; }

        public Cast(IExpression operand, Type type)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Cast(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Cast(this);
        }
    }
}
