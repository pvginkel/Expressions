using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

namespace Expressions.BoundAst
{
    internal class BoundBinaryExpression : IBoundAstNode
    {
        public Type ResultType { get; private set; }

        public IBoundAstNode Left { get; private set; }

        public IBoundAstNode Right { get; private set; }

        public ExpressionType Type { get; private set; }
    }
}
