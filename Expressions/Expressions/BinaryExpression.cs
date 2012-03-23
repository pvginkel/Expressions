using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

namespace Expressions.Expressions
{
    internal class BinaryExpression : IExpression
    {
        public IExpression Left { get; private set; }

        public IExpression Right { get; private set; }

        public ExpressionType ExpressionType { get; private set; }

        public Type Type { get; private set; }

        public BinaryExpression(IExpression left, IExpression right, ExpressionType expressionType, Type type)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");
            Require.NotNull(type, "type");

            Left = left;
            Right = right;
            ExpressionType = expressionType;
            Type = type;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.BinaryExpression(this);
        }
    }
}
