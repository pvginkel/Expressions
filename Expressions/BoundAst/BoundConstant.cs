using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.BoundAst
{
    internal class BoundConstant : IBoundAstNode
    {
        public object Value { get; private set; }

        public Type ResultType
        {
            get
            {
                if (Value == null)
                    return typeof(object);
                else
                    return Value.GetType();
            }
        }
    }
}
