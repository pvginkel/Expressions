using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedMethodCall : IResolvedAstNode
    {
        public MethodIdentifier Method { get; private set; }

        public IList<IResolvedAstNode> Arguments { get; private set; }

        public Type Type
        {
            get { return Method.Type; }
        }

        IResolvedIdentifier IResolvedAstNode.Identifier
        {
            get { return Method; }
        }

        public ResolvedMethodCall(MethodIdentifier method, IResolvedAstNode[] arguments)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            Method = method;
            Arguments = new ReadOnlyCollection<IResolvedAstNode>(arguments);
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Method, Type, member, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedMethodCall;

            if (
                other == null ||
                !Equals(Method, other.Method) ||
                 Arguments.Count != other.Arguments.Count
            )
                return false;

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Equals(Arguments[i], other.Arguments[i]))
                    return false;
            }

            return true;
        }
    }
}
