using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal interface IExpression
    {
        Type Type { get; }

        T Accept<T>(IExpressionVisitor<T> visitor);
    }
}
