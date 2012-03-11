using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions
{
    internal class OwnerFieldResolver : IIdentifierResolver
    {
        public FieldInfo FieldInfo { get; private set; }

        public OwnerFieldResolver(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo");

            FieldInfo = fieldInfo;
        }
    }
}
