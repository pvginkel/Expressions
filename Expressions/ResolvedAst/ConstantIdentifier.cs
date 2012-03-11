using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ConstantIdentifier : ITypedResolvedIdentifier
    {
        public bool IsStatic { get { return false; } }

        public bool IsResolved { get { return false; } }

        public object Value { get; private set; }

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

        public ConstantIdentifier(object value)
        {
            Value = value;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ConstantIdentifier;

            return
                other != null &&
                Equals(Value, other.Value);
        }
    }
}
