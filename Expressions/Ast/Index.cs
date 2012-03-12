using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class Index : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public AstNodeCollection Arguments { get; private set; }

        public Index(IAstNode operand, AstNodeCollection arguments)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(arguments, "arguments");

            Operand = operand;
            Arguments = arguments;
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            var operand = Operand.Resolve(resolver);

            var arguments = new IResolvedAstNode[Arguments == null ? 0 : Arguments.Nodes.Count];

            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = Arguments.Nodes[i].Resolve(resolver);
            }

            PropertyIdentifier property;

            if (operand.Identifier is PropertyGroupIdentifier)
                property = ((PropertyGroupIdentifier)operand.Identifier).Resolve(arguments);
            else if (operand.Identifier is PropertyIdentifier)
                property = (PropertyIdentifier)operand.Identifier;
            else
                throw new NotImplementedException();

            return new ResolvedIndex(operand, property, arguments);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Operand);
            sb.Append('[');

            for (int i = 0; i < Arguments.Nodes.Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                sb.Append(Arguments.Nodes[i]);
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
