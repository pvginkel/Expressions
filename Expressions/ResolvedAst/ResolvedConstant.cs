using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedConstant : IResolvedAstNode
    {
        public object Value { get; private set; }

        public IResolvedIdentifier Identifier { get; private set; }

        public ResolvedConstant(object value)
        {
            Value = value;

            Identifier = new ConstantIdentifier(value);
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Identifier, Type, member, false);
        }

        public Type Type
        {
            get
            {
                if (Value == null)
                    return typeof(object);
                else
                    return Value.GetType();
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedConstant;

            return
                other != null &&
                Equals(Value, other.Value) &&
                Equals(Identifier, other.Identifier);
        }
    }
}
