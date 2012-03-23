using System;
using System.Collections.Generic;
using System.Text;

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

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.MemberAccess(this);
        }

        public override string ToString()
        {
            return Operand + "." + Member;
        }
    }
}
