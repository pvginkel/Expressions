using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class Constant : IExpression
    {
        public Type Type { get; private set; }

        public object Value { get; private set; }

        public Constant(object value)
        {
            Value = value;

            if (value == null)
                Type = typeof(object);
            else if (value is UnparsedNumber)
                Type = ((UnparsedNumber)value).Type;
            else
                Type = value.GetType();
        }

        public void Accept(IExpressionVisitor visitor)
        {
            visitor.Constant(this);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Constant(this);
        }
    }
}
