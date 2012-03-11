using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class TypeIdentifier : ITypedResolvedIdentifier
    {
        public bool IsStatic { get { return true; } }

        public bool IsResolved { get { return false; } }

        public Type Type { get; private set; }

        public TypeIdentifier(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, IsStatic);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as TypeIdentifier;

            return
                other != null &&
                Type == other.Type;
        }
    }
}
