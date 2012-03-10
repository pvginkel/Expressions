using System.Text;
using System.Collections.Generic;
using System;

namespace Expressions
{
    internal class VariableResolver : IIdentifierResolver
    {
        public Type Type { get; private set; }

        public VariableResolver(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as VariableResolver;

            return
                other != null &&
                    Type == other.Type;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ObjectUtil.CombineHashCodes(
                    GetType().GetHashCode(),
                    Type.GetHashCode()
                    );
            }
        }
    }
}