using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class BinaryExpression : IAstNode
    {
        public IAstNode Left { get; private set; }

        public IAstNode Right { get; private set; }

        public ExpressionType Type { get; private set; }

        public BinaryExpression(IAstNode left, IAstNode right, ExpressionType type)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");

            Left = left;
            Right = right;
            Type = type;
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.BinaryExpression(this);
        }

        public override string ToString()
        {
            return String.Format("({0} {1} {2})", Left, Type, Right);
        }
    }
}
