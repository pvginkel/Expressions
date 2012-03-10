using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions.Ast
{
    internal class IdentifierCollection : KeyedCollection<string, Identifier>
    {
        public IdentifierCollection()
        {
        }

        public IdentifierCollection(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        public IdentifierCollection(IEqualityComparer<string> comparer, int dictionaryCreationThreshold)
            : base(comparer, dictionaryCreationThreshold)
        {
        }

        protected override string GetKeyForItem(Identifier item)
        {
            return item.Name;
        }
    }
}
