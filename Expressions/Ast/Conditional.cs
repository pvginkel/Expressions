using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class Conditional : IAstNode
    {
        public IAstNode Condition { get; private set; }

        public IAstNode Then { get; private set; }

        public IAstNode Else { get; private set; }

        public Conditional(IAstNode condition, IAstNode then, IAstNode @else)
        {
            Require.NotNull(condition, "condition");
            Require.NotNull(then, "then");
            Require.NotNull(@else, "else");

            Condition = condition;
            Then = then;
            Else = @else;
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.Conditional(this);
        }

        public override string ToString()
        {
            return String.Format("Conditional({0}, {1}, {2})", Condition, Then, Else);
        }
    }
}
