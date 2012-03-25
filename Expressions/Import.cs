using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    public sealed class Import
    {
        private static readonly Import[] EmptyImports = new Import[0];

        private int? _hashCode;

        public Type Type { get; private set; }

        public string Namespace { get; private set; }

        public IList<Import> Imports { get; private set; }

        public Import(Type type)
            : this(null, type)
        {
        }

        public Import(string ns, Type type)
            : this(ns, type, null)
        {
            Require.NotNull(type, "type");
        }

        public Import(string ns, params Import[] imports)
            : this(ns, null, imports)
        {
            Require.NotNull(imports, "imports");
            Require.That(imports.Length > 0, "At least one import is required in a namespace", "imports");
        }

        private Import(string ns, Type type, params Import[] imports)
        {
            Type = type;
            Namespace = ns;

            if (imports == null || imports.Length == 0)
                Imports = EmptyImports;
            else
                Imports = new ReadOnlyCollection<Import>(imports);

            foreach (var import in Imports)
            {
                if (import == null)
                    throw new ArgumentException("Item of imports cannot be null", "imports");
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as Import;

            if (
                other == null ||
                Type != other.Type ||
                Namespace != other.Namespace ||
                Imports.Count != other.Imports.Count
            )
                return false;

            for (int i = 0; i < Imports.Count; i++)
            {
                if (!Imports[i].Equals(other.Imports[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                unchecked
                {
                    int hashCode = ObjectUtil.CombineHashCodes(
                        Type == null ? 0 : Type.GetHashCode(),
                        Namespace == null ? 0 : Namespace.GetHashCode()
                    );

                    foreach (var import in Imports)
                    {
                        hashCode = ObjectUtil.CombineHashCodes(
                            hashCode, import.GetHashCode()
                        );
                    }

                    _hashCode = hashCode;
                }
            }

            return _hashCode.Value;
        }

        public override string ToString()
        {
            return Type.FullName + " in " + (Namespace ?? Type.Name);
        }
    }
}
