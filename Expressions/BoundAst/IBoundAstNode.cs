using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.BoundAst
{
    internal interface IBoundAstNode
    {
        Type ResultType { get; }
    }
}
