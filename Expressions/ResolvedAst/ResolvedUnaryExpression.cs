using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedUnaryExpression : IResolvedAstNode
    {
        public IResolvedAstNode Operand { get; private set; }

        public Type Type { get; private set; }

        public ExpressionType ExpressionType { get; private set; }

        public IResolvedIdentifier Identifier
        {
            get { return Operand.Identifier; }
        }

        public ResolvedUnaryExpression(IResolvedAstNode operand, Type type, ExpressionType expressionType)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
            ExpressionType = expressionType;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Operand.Identifier, Type, member, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedUnaryExpression;

            return
                other != null &&
                Equals(Operand, other.Operand) &&
                Type == other.Type &&
                ExpressionType == other.ExpressionType;
        }
    }
}
