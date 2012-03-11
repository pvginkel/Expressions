using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions.BoundAst
{
    internal class BoundField : IBoundAstNode
    {
        public Type ResultType
        {
            get { return FieldInfo.FieldType; }
        }

        public FieldInfo FieldInfo { get; private set; }

        public IBoundAstNode Object { get; private set; }
    }
}
