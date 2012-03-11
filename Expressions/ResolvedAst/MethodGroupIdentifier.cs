using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class MethodGroupIdentifier : IResolvedIdentifier
    {
        public IResolvedIdentifier Operand { get; private set; }

        public bool IsStatic
        {
            get { throw new NotSupportedException(); }
        }

        public bool IsResolved { get { return false; } }

        public IList<MethodInfo> Methods { get; private set; }

        public MethodGroupIdentifier(IResolvedIdentifier operand, MethodInfo[] methods)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");
            if (methods == null)
                throw new ArgumentNullException("methods");

            Operand = operand;
            Methods = new ReadOnlyCollection<MethodInfo>(methods);
        }

        public MethodIdentifier Resolve(IResolvedAstNode[] arguments)
        {
            var methodInfo = Resolver.ResolveMethodGroup(Methods, arguments);

            return new MethodIdentifier(Operand, methodInfo);
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            throw new NotSupportedException();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as MethodGroupIdentifier;

            if (
                other == null ||
                !Equals(Operand, other.Operand) ||
                Methods.Count != other.Methods.Count
            )
                return false;

            for (int i = 0; i < Methods.Count; i++)
            {
                if (Methods[i] != other.Methods[i])
                    return false;
            }

            return true;
        }
    }
}
