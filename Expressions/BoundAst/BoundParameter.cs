using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.BoundAst
{
    internal class BoundParameter : IBoundAstNode
    {
        public Type ParameterType { get; private set; }

        public int ParameterIndex { get; private set; }

        Type IBoundAstNode.ResultType
        {
            get { return ParameterType; }
        }
    }
}
