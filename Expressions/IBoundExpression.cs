using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public interface IBoundExpression
    {
        object Invoke();

        object Invoke(IExecutionContext executionContext);
    }

    public interface IBoundExpression<out T> : IBoundExpression
    {
        new T Invoke();

        new T Invoke(IExecutionContext executionContext);
    }
}
