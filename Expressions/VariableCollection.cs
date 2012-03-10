using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    public sealed class VariableCollection : KeyedCollection<string, Variable>
    {
        public VariableCollection()
        {
        }

        public VariableCollection(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        public VariableCollection(IEqualityComparer<string> comparer, int dictionaryCreationThreshold)
            : base(comparer, dictionaryCreationThreshold)
        {
        }

        protected override string GetKeyForItem(Variable item)
        {
            return item.Name;
        }

        public void Add(string name, object value)
        {
            Add(new Variable(name) { Value = value });
        }
    }
}
