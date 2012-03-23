using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class Constant : IExpression
    {
        public Type Type
        {
            get
            {
                if (Value == null)
                    return typeof(object);
                else
                    return Value.GetType();
            }
        }

        public object Value { get; private set; }

        public Constant(object value)
        {
            Value = value;
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Constant(this);
        }
    }
}
