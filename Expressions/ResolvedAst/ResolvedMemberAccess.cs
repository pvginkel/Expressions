using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedMemberAccess : IResolvedAstNode
    {
        public IResolvedAstNode Operand { get; private set; }

        public IResolvedIdentifier Identifier { get; private set; }

        public ResolvedMemberAccess(IResolvedAstNode operand, IResolvedIdentifier identifier)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");
            if (identifier == null)
                throw new ArgumentNullException("identifier");

            Operand = operand;
            Identifier = identifier;
        }

        public Type Type
        {
            get
            {
                var typedIdentifier = Identifier as ITypedResolvedIdentifier;

                if (typedIdentifier == null)
                    throw new NotSupportedException();

                return typedIdentifier.Type;
            }
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return Identifier.Resolve(resolver, member);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedMemberAccess;

            return
                other != null &&
                Equals(Operand, other.Operand) &&
                Equals(Identifier, other.Identifier);
        }
    }
}
