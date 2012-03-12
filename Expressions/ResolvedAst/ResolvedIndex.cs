using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ResolvedIndex : IResolvedAstNode
    {
        public IResolvedAstNode Operand { get; private set; }

        public PropertyIdentifier Property { get; private set; }

        public IList<IResolvedAstNode> Arguments { get; private set; }

        public Type Type
        {
            get { return Property.Type; }
        }

        public IResolvedIdentifier Identifier
        {
            get { return Operand.Identifier; }
        }

        public ResolvedIndex(IResolvedAstNode operand, PropertyIdentifier property, IResolvedAstNode[] arguments)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(property, "property");
            Require.NotNull(arguments, "arguments");

            Operand = operand;
            Property = property;
            Arguments = new ReadOnlyCollection<IResolvedAstNode>(arguments);
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string member)
        {
            return resolver.Resolve(Operand.Identifier, Type, member, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ResolvedIndex;

            if (
                other == null ||
                !Equals(Operand, other.Operand) ||
                !Equals(Property, other.Property) ||
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
