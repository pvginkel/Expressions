using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class Cast : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public TypeIdentifier Type { get; private set; }

        public Cast(IAstNode operand, TypeIdentifier type)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;
        }

        public override string ToString()
        {
            return String.Format("Cast({0}, {1})", Operand, Type);
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            var operand = Operand.Resolve(resolver);

            return new ResolvedCast(operand, ((ResolvedType)Type.Resolve(resolver)).Type);
        }
    }
}
