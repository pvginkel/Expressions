using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class MemberAccess : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public string Member { get; private set; }

        public MemberAccess(IAstNode operand, string member)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(member, "member");

            Operand = operand;
            Member = member;
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            var operand = Operand.Resolve(resolver);

            var member = operand.Resolve(resolver, Member);

            return new ResolvedMemberAccess(operand, member);
        }

        public override string ToString()
        {
            return Operand + "." + Member;
        }
    }
}
