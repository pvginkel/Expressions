using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions.Expressions
{
    internal class Index : IExpression
    {
        public IExpression Operand { get; private set; }

        public IExpression Argument { get; private set; }

        public Type Type { get; private set; }

        public Index(IExpression operand, IExpression argument, Type type)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(argument, "argument");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;

            Argument = argument;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Index(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Index(this);
        }
    }
}
