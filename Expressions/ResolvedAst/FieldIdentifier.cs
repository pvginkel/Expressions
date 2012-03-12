using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class FieldIdentifier : ITypedResolvedIdentifier
    {
        public bool IsStatic { get { return Field.IsStatic; } }

        public bool IsResolved { get { return true; } }

        public FieldInfo Field { get; private set; }

        public Type Type { get { return Field.FieldType; } }

        public FieldIdentifier(FieldInfo fieldInfo)
        {
            Require.NotNull(fieldInfo, "fieldInfo");

            Field = fieldInfo;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, IsStatic);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as FieldIdentifier;

            return
                other != null &&
                Field == other.Field;
        }
    }
}
