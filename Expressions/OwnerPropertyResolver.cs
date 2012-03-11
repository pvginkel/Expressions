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
    }
}