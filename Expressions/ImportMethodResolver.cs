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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as ImportMethodResolver;

            return
                other != null &&
                    MethodInfo == other.MethodInfo;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ObjectUtil.CombineHashCodes(
                    GetType().GetHashCode(),
                    MethodInfo.GetHashCode()
                    );
            }
        }
    }
}