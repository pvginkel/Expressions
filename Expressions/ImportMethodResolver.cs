using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System;

namespace Expressions
{
    internal class ImportMethodResolver : IIdentifierResolver
    {
        public MethodInfo MethodInfo { get; private set; }

        public ImportMethodResolver(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            MethodInfo = methodInfo;
        }
    }
}