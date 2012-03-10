using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System;

namespace Expressions
{
    internal class OwnerPropertyResolver : IIdentifierResolver
    {
        public PropertyInfo PropertyInfo { get; private set; }

        public OwnerPropertyResolver(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            PropertyInfo = propertyInfo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as OwnerPropertyResolver;

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