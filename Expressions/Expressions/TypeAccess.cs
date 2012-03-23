using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class TypeAccess : IExpression
    {
        public Type Type { get; private set; }

        public TypeAccess(Type type)
        {
            Require.NotNull(type, "type");

            Type = type;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.TypeAccess(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.TypeAccess(this);
        }
    }
}
