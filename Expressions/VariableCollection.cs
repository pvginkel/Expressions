using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// A collection of <see cref="Variable"/>'s.
    /// </summary>
    public sealed class VariableCollection : KeyedCollection<string, Variable>
    {
        internal VariableCollection(IEqualityComparer<string> comparer)
            : base(comparer)
        {
        }

        /// <summary>
        /// When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <returns>
        /// The key for the specified element.
        /// </returns>
        /// <param name="item">The element from which to extract the key.
        ///                 </param>
        protected override string GetKeyForItem(Variable item)
        {
            return item.Name;
        }

        /// <summary>
        /// Add a new <see cref="Variable"/> to the collection with the specified
        /// name and value.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public void Add(string name, object value)
        {
            Add(new Variable(name) { Value = value });
        }
    }
}
