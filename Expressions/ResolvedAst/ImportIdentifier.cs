using System;
using System.Collections.Generic;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class ImportIdentifier : IResolvedIdentifier
    {
        public bool IsStatic { get { return true; } }

        public bool IsResolved { get { return false; } }

        public Import Import { get; private set; }

        public ImportIdentifier(Import import)
        {
            Require.NotNull(import, "import");

            Import = import;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Import.Type, name, true);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ImportIdentifier;

            return
                other != null &&
                Equals(Import, other.Import);
        }
    }
}
