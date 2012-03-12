using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class VariableIdentifier : ITypedResolvedIdentifier
    {
        public Type Type { get; private set; }

        public int ParameterIndex { get; private set; }

        public bool IsStatic { get { return false; } }

        public bool IsResolved { get { return true; } }

        public VariableIdentifier(Type type, int parameterIndex)
        {
            Require.NotNull(type, "type");

            Type = type;
            ParameterIndex = parameterIndex;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as VariableIdentifier;

            return
                other != null &&
                Type == other.Type &&
                ParameterIndex == other.ParameterIndex;
        }
    }
}
