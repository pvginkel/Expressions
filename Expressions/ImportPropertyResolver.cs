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
    }
}