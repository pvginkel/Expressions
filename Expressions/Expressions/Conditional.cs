using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class Conditional : IExpression
    {
        public IExpression Condition { get; private set; }

        public IExpression Then { get; private set; }

        public IExpression Else { get; private set; }

        public Type Type { get; private set; }

        public Conditional(IExpression condition, IExpression then, IExpression @else, Type type)
        {
            Require.NotNull(condition, "condition");
            Require.NotNull(then, "then");
            Require.NotNull(@else, "else");
            Require.NotNull(type, "type");

            Condition = condition;
            Then = then;
            Else = @else;
            Type = type;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Conditional(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Conditional(this);
        }
    }
}
