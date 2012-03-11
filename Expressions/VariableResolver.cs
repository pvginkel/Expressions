using System.Text;
using System.Collections.Generic;
using System;

namespace Expressions
{
    internal class VariableResolver : IIdentifierResolver
    {
        public Type Type { get; private set; }

        public int ParameterIndex { get; private set; }

        public VariableResolver(Type type, int parameterIndex)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
            ParameterIndex = parameterIndex;
        }
    }
}