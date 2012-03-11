using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class PropertyGroupIdentifier : IResolvedIdentifier
    {
        public IResolvedIdentifier Operand { get; private set; }

        public bool IsStatic
        {
            get { throw new NotSupportedException(); }
        }

        public bool IsResolved { get { return false; } }

        public IList<MethodInfo> Properties { get; private set; }

        public PropertyGroupIdentifier(IResolvedIdentifier operand, MethodInfo[] properties)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");
            if (properties == null)
                throw new ArgumentNullException("properties");

            Operand = operand;
            Properties = new ReadOnlyCollection<MethodInfo>(properties);
        }

        public PropertyIdentifier Resolve(IResolvedAstNode[] arguments)
        {
            var methodInfo = Resolver.ResolveMethodGroup(Properties, arguments);

            return new PropertyIdentifier(Operand, methodInfo);
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
                Properties.Count != other.Methods.Count
            )
                return false;

            for (int i = 0; i < Properties.Count; i++)
            {
                if (Properties[i] != other.Methods[i])
                    return false;
            }

            return true;
        }
    }
}
