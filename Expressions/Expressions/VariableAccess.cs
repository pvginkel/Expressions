using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class VariableAccess : IExpression
    {
        public Type Type { get; private set; }

        public int ParameterIndex { get; private set; }

        public VariableAccess(Type type, int parameterIndex)
        {
            Require.NotNull(type, "type");

            Type = type;
            ParameterIndex = parameterIndex;
        }
    }
}
