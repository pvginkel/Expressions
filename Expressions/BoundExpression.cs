using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public sealed class BoundExpression
    {
        private DynamicExpression _dynamicExpression;
        private IIdentifierResolver[] _resolvedIdentifiers;

        internal BoundExpression(DynamicExpression dynamicExpression, IIdentifierResolver[] resolvedIdentifiers)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (resolvedIdentifiers == null)
                throw new ArgumentNullException("resolvedIdentifiers");

            _dynamicExpression = dynamicExpression;
            _resolvedIdentifiers = resolvedIdentifiers;
        }
    }
}
