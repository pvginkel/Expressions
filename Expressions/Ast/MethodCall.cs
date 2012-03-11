using System;
using System.Collections.Generic;
using System.Text;
using Expressions.BoundAst;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class MethodCall : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public AstNodeCollection Arguments { get; private set; }

        public MethodCall(IAstNode operand)
            : this(operand, null)
        {
        }

        public MethodCall(IAstNode operand, AstNodeCollection arguments)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");

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

            MethodIdentifier method;

            if (operand.Identifier is MethodGroupIdentifier)
                method = ((MethodGroupIdentifier)operand.Identifier).Resolve(arguments);
            else if (operand.Identifier is MethodIdentifier)
                method = (MethodIdentifier)operand.Identifier;
            else
                throw new NotImplementedException();

            return new ResolvedMethodCall(operand, method, arguments);
        }

        public override string ToString()
        {
            return Operand + (Arguments == null ? "()" : Arguments.ToString());
        }
    }
}
