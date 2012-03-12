using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class MethodIdentifier : ITypedResolvedIdentifier
    {
        public bool IsStatic { get { return Method.IsStatic; } }

        public bool IsResolved { get { return true; } }

        public IResolvedIdentifier Operand { get; private set; }

        public MethodInfo Method { get; private set; }

        public Type Type { get { return Method.ReturnType; } }

        public MethodIdentifier(IResolvedIdentifier operand, MethodInfo method)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(method, "method");

            Operand = operand;
            Method = method;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, IsStatic);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as MethodIdentifier;

            return
                other != null &&
                Equals(Operand, other.Operand) &&
                Method == other.Method;
        }
    }
}
