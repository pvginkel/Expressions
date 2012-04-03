using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Describes an import definition of an expression.
    /// </summary>
    public sealed class Import
    {
        private static readonly Import[] EmptyImports = new Import[0];

        private int? _hashCode;

        /// <summary>
        /// Get the type to be imported.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Get the namespace the type must be imported at.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Get the imports that are imported as nested namespaces.
        /// </summary>
        public IList<Import> Imports { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class with
        /// the specified type.
        /// </summary>
        /// <param name="type">The type to be imported.</param>
        public Import(Type type)
            : this(null, type)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class with
        /// the specified type and namespace.
        /// </summary>
        /// <param name="ns">The namespace the type must be imported at.</param>
        /// <param name="type">The type to be imported.</param>
        public Import(string ns, Type type)
            : this(ns, type, null)
        {
            Require.NotNull(type, "type");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class with
        /// the specified namespace and nested imports.
        /// </summary>
        /// <param name="ns">The namespace the type must be imported at.</param>
        /// <param name="imports">The imports that are nested under this import.</param>
        public Import(string ns, params Import[] imports)
            : this(ns, null, imports)
        {
            Require.NotNull(imports, "imports");
            Require.That(imports.Length > 0, "At least one import is required in a namespace", "imports");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Import"/> class with
        /// the specified type, namespace and nested imports.
        /// </summary>
        /// <param name="ns">The namespace the type must be imported at.</param>
        /// <param name="type">The type to be imported.</param>
        /// <param name="imports">The imports that are nested under this import.</param>
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

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
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

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
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

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Type.FullName + " in " + (Namespace ?? Type.Name);
        }
    }
}
