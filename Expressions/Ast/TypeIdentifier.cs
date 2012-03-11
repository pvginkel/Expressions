using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class TypeIdentifier : IAstNode
    {
        public string Name { get; private set; }

        public int ArrayIndex { get; private set; }

        public TypeIdentifier(string name, int arrayIndex)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
            ArrayIndex = arrayIndex;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Name);

            if (ArrayIndex > 0)
            {
                sb.Append('[');

                if (ArrayIndex > 1)
                    sb.Append(new String(',', ArrayIndex - 1));

                sb.Append(']');
            }

            return sb.ToString();
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            return new ResolvedType(resolver.ResolveType(Name, ArrayIndex));
        }
    }
}
