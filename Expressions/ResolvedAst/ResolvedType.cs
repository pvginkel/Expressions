using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedType : IResolvedAstNode
    {
        public Type Type { get; private set; }

        public IResolvedIdentifier Identifier { get; private set; }

        public ResolvedType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;

            Identifier = new TypeIdentifier(Type);
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Identifier, Type, member, true);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedType;

            return
                other != null &&
                Type == other.Type &&
                Equals(Identifier, other.Identifier);
        }
    }
}
