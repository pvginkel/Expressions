using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedBinaryExpression : IResolvedAstNode
    {
        public IResolvedAstNode Left { get; private set; }

        public IResolvedAstNode Right { get; private set; }

        public Type Type { get; private set; }

        public ExpressionType ExpressionType { get; private set; }

        public IResolvedIdentifier Identifier { get; private set; }

        public ResolvedBinaryExpression(IResolvedAstNode left, IResolvedAstNode right, Type type, ExpressionType expressionType)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            if (type == null)
                throw new ArgumentNullException("type");

            Left = left;
            Right = right;
            Type = type;
            ExpressionType = expressionType;

            Identifier = new BinaryIdentifier(Left.Identifier, Right.Identifier, Type);
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Identifier, Type, member, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedBinaryExpression;

            return
                other != null &&
                Equals(Left, other.Left) &&
                Equals(Right, other.Right) &&
                Type == other.Type &&
                ExpressionType == other.ExpressionType &&
                Equals(Identifier, other.Identifier);
        }
    }
}
