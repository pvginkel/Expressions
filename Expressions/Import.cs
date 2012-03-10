using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public sealed class Import
    {
        private int? _hashCode;

        public Type Type { get; private set; }

        public string Namespace { get; private set; }

        public Import(Type type)
            : this(type, null)
        {
        }

        public Import(Type type, string ns)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Type = type;
            Namespace = ns;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as Import;

            return
                other != null &&
                Type == other.Type &&
                Namespace == other.Namespace;
        }

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                unchecked
                {
                    _hashCode = ObjectUtil.CombineHashCodes(
                        Type.GetHashCode(),
                        Namespace == null ? 0 : Namespace.GetHashCode()
                    );
                }
            }

            return _hashCode.Value;
        }
    }
}
