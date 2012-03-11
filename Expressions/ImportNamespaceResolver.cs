using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal class ImportNamespaceResolver : IIdentifierResolver
    {
        public Import Import { get; private set; }

        public ImportNamespaceResolver(Import import)
        {
            if (import == null)
                throw new ArgumentNullException("import");

            Import = import;
        }
    }
}
