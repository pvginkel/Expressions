using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

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

        public override string ToString()
        {
            return String.Format("({0} {1} {2})", Left, Type, Right);
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            var left = Left.Resolve(resolver);
            var right = Right.Resolve(resolver);
            var type = resolver.ResolveExpressionType(left.Type, right.Type, Type);

            return new ResolvedBinaryExpression(left, right, type, Type);
        }
    }
}
