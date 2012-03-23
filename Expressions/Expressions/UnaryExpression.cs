using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

namespace Expressions.Expressions
{
    internal class UnaryExpression : IExpression
    {
        public IExpression Operand { get; private set; }

        public Type Type { get; private set; }

        public ExpressionType ExpressionType { get; private set; }

        public UnaryExpression(IExpression operand, Type type, ExpressionType expressionType)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
            ExpressionType = expressionType;
        }
    }
}
