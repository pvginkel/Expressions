using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Represents the context of an expression.
    /// </summary>
    public interface IExpressionContext : IBindingContext, IExecutionContext
    {
    }
}
