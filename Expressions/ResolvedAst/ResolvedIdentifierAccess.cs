using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedIdentifierAccess : IResolvedAstNode
    {
        public IResolvedIdentifier Identifier { get; private set; }

        public ResolvedIdentifierAccess(IResolvedIdentifier identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");

            Identifier = identifier;
        }

        public Type Type
        {
            get
            {
                var typedIdentifier = Identifier as ITypedResolvedIdentifier;

                return typedIdentifier == null ? null : typedIdentifier.Type;
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

            var other = obj as ResolvedIdentifierAccess;

            return
                other != null &&
                Equals(Identifier, other.Identifier);
        }
    }
}
