using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Expressions
{
    internal class Compiler
    {
        private readonly DynamicExpression _dynamicExpression;
        private readonly Type _ownerType;
        private readonly Import[] _imports;
        private readonly Type[] _identifierTypes;
        private readonly int[] _parameterMap;
        private readonly bool _ignoreCase;

        public ILGenerator Il { get; private set; }

        public Compiler(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes, int[] parameterMap, ILGenerator il)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (imports == null)
                throw new ArgumentNullException("imports");
            if (identifierTypes == null)
                throw new ArgumentNullException("identifierTypes");
            if (parameterMap == null)
                throw new ArgumentNullException("parameterMap");
            if (il == null)
                throw new ArgumentNullException("il");

            _dynamicExpression = dynamicExpression;
            _ownerType = ownerType;
            _imports = imports;
            _identifierTypes = identifierTypes;
            _parameterMap = parameterMap;

            _ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);

            Il = il;
        }

        public void Compile()
        {
            //_dynamicExpression.ParseResult.RootNode.Compile(this);
        }

        public void PushOwner()
        {
            Debug.Assert(_ownerType != null);

            PushParameter(0);

            Il.Emit(OpCodes.Castclass, _ownerType);
        }

        public void PushParameter(int index)
        {
            Debug.Assert(index < _parameterMap.Length);

            Il.Emit(OpCodes.Ldarg_0);

            switch (index)
            {
                case 0: Il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: Il.Emit(OpCodes.Ldc_I4_1); break;
                case 2: Il.Emit(OpCodes.Ldc_I4_2); break;
                case 3: Il.Emit(OpCodes.Ldc_I4_3); break;
                case 4: Il.Emit(OpCodes.Ldc_I4_4); break;
                case 5: Il.Emit(OpCodes.Ldc_I4_5); break;
                case 6: Il.Emit(OpCodes.Ldc_I4_6); break;
                case 7: Il.Emit(OpCodes.Ldc_I4_7); break;
                case 8: Il.Emit(OpCodes.Ldc_I4_8); break;
                default: Il.Emit(OpCodes.Ldc_I4_S, index); break;
            }

            Il.Emit(OpCodes.Ldelem_Ref);
        }

        private bool LanguageEquals(string a, string b)
        {
            return String.Equals(a, b, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
    }
}
