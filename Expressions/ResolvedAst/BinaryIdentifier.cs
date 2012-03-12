using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class BinaryIdentifier : ITypedResolvedIdentifier
    {
        public IResolvedIdentifier Left { get; private set; }

        public IResolvedIdentifier Right { get; private set; }

        public Type Type { get; private set; }

        public bool IsStatic { get; private set; }

        public bool IsResolved { get { return true; } }

        public BinaryIdentifier(IResolvedIdentifier left, IResolvedIdentifier right, Type type)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");
            Require.NotNull(type, "type");

            Debug.Assert(left.IsStatic == right.IsStatic);

            Left = left;
            Right = right;

            IsStatic = left.IsStatic;
            Type = type;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as BinaryIdentifier;

            return
                other != null &&
                Equals(Left, other.Left) &&
                Equals(Right, other.Right) &&
                Type == other.Type;
        }
    }
}
