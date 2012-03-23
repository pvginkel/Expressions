using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions.Expressions
{
    internal class FieldAccess : IExpression
    {
        public Type Type
        {
            get { return FieldInfo.FieldType; }
        }

        public IExpression Operand { get; private set; }

        public FieldInfo FieldInfo { get; private set; }

        public FieldAccess(IExpression operand, FieldInfo fieldInfo)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(fieldInfo, "fieldInfo");

            Operand = operand;
            FieldInfo = fieldInfo;
        }
    }
}
