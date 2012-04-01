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

        public Type CommonType { get; private set; }

        public BinaryExpression(IExpression left, IExpression right, ExpressionType expressionType, Type type)
            : this(left, right, expressionType, type, type)
        {
        }

        public BinaryExpression(IExpression left, IExpression right, ExpressionType expressionType, Type type, Type commonType)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");
            Require.NotNull(type, "type");
            Require.NotNull(commonType, "commonType");

            Left = left;
            Right = right;
            ExpressionType = expressionType;
            Type = type;
            CommonType = commonType;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.BinaryExpression(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.BinaryExpression(this);
        }
    }
}
