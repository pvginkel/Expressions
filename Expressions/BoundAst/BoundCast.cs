using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.BoundAst
{
    internal class BoundCast : IBoundAstNode
    {
        public Type ResultType { get; private set; }

        public IBoundAstNode Operand { get; private set; }
    }
}
