using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.BoundAst
{
    internal class Binder
    {
        private readonly DynamicExpression _dynamicExpression;
        private readonly Type _ownerType;
        private readonly Import[] _imports;
        private readonly Type[] _identifierTypes;
        private readonly int[] _parameterMap;
        private readonly bool _ignoreCase;

        public Binder(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes, int[] parameterMap)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (imports == null)
                throw new ArgumentNullException("imports");
            if (identifierTypes == null)
                throw new ArgumentNullException("identifierTypes");
            if (parameterMap == null)
                throw new ArgumentNullException("parameterMap");

            _dynamicExpression = dynamicExpression;
            _ownerType = ownerType;
            _imports = imports;
            _identifierTypes = identifierTypes;
            _parameterMap = parameterMap;

            _ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);
        }
    }
}
