using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System;

namespace Expressions
{
    internal class ImportPropertyResolver : IIdentifierResolver
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public ImportPropertyResolver(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            PropertyInfo = propertyInfo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ImportPropertyResolver;

            return
                other != null &&
                    PropertyInfo == other.PropertyInfo;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ObjectUtil.CombineHashCodes(
                    GetType().GetHashCode(),
                    PropertyInfo.GetHashCode()
                    );
            }
        }
    }
}