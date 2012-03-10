using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public sealed class Import
    {
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
    }
}
