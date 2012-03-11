using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

namespace Expressions.BoundAst
{
    internal class BoundUnaryExpression : IBoundAstNode
    {
        public Type ResultType { get; private set; }

        public IBoundAstNode Operand { get; private set; }

        public ExpressionType Type { get; private set; }
    }
}
