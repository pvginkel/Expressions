using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Expressions
{
    public sealed class BoundExpression
    {
        private readonly DynamicExpression _dynamicExpression;
        private readonly Type _ownerType;
        private readonly Import[] _import;
        private readonly Type[] _identifierTypes;
        private readonly int[] _parameterMap;
        private readonly DynamicSignature _compiledMethod;

        internal BoundExpression(DynamicExpression dynamicExpression, Type ownerType, Import[] import, Type[] identifierTypes)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (ownerType == null)
                throw new ArgumentNullException("ownerType");
            if (import == null)
                throw new ArgumentNullException("import");
            if (identifierTypes == null)
                throw new ArgumentNullException("identifierTypes");

            _dynamicExpression = dynamicExpression;
            _ownerType = ownerType;
            _import = import;
            _identifierTypes = identifierTypes;

            _compiledMethod = CompileExpression();
        }

        private DynamicSignature CompileExpression()
        {
            var method = new DynamicMethod(
                "DynamicMethod",
                typeof(object),
                new[] { typeof(object[]) },
                true
            );

            var il = method.GetILGenerator();

            return (DynamicSignature)method.CreateDelegate(typeof(DynamicSignature));
        }

        public object Invoke(IExecutionContext executionContext)
        {
            if (executionContext == null)
                throw new ArgumentNullException("executionContext");

            var parameters = new object[_parameterMap.Length];

            bool ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);
            var identifiers = _dynamicExpression.ParseResult.Identifiers;

            for (int i = 0; i < parameters.Length; i++)
            {
                int index = _parameterMap[i];

                if (index == -1)
                {
                    parameters[i] = executionContext.Owner;
                }
                else
                {
                    parameters[i] = executionContext.GetVariableValue(
                        identifiers[index].Name, ignoreCase
                    );
                }
            }

            return _compiledMethod(parameters);
        }

        private delegate object DynamicSignature(object[] parameters);
    }
}
