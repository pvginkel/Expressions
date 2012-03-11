using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedCast : IResolvedAstNode
    {
        public IResolvedAstNode Operand { get; private set; }

        public Type Type { get; private set; }

        public IResolvedIdentifier Identifier
        {
            get { return Operand.Identifier; }
        }

        public ResolvedCast(IResolvedAstNode operand, Type type)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");
            if (type == null)
                throw new ArgumentNullException("type");

            Operand = operand;
            Type = type;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Operand.Identifier, Type, member, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedCast;

            return
                other != null &&
                Equals(Operand, other.Operand) &&
                Type == other.Type;
        }
    }
}
