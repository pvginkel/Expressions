using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Expressions.Ast;
using Expressions.Expressions;

namespace Expressions
{
    internal class Resolver
    {
        public DynamicExpression DynamicExpression { get; private set; }

        public Type[] IdentifierTypes { get; private set; }

        public int[] IdentifierIndexes { get; private set; }

        public Type OwnerType { get; private set; }

        public Import[] Imports { get; private set; }

        public bool IgnoreCase { get; private set; }

        public Resolver(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes, int[] parameterMap)
        {
            Require.NotNull(dynamicExpression, "dynamicExpression");
            Require.NotNull(imports, "imports");
            Require.NotNull(identifierTypes, "identifierTypes");
            Require.NotNull(parameterMap, "parameterMap");

            DynamicExpression = dynamicExpression;
            OwnerType = ownerType;
            Imports = imports;
            IdentifierTypes = identifierTypes;

            // Inverse the parameter map.

            IdentifierIndexes = new int[IdentifierTypes.Length];

            for (int i = 0; i < IdentifierTypes.Length; i++)
            {
                IdentifierIndexes[i] = -1;

                if (IdentifierTypes[i] != null)
                {
                    for (int j = 0; j < parameterMap.Length; j++)
                    {
                        if (parameterMap[j] == i)
                        {
                            IdentifierIndexes[i] = j;
                            break;
                        }
                    }

                    Debug.Assert(IdentifierIndexes[i] != -1);
                }
            }

            IgnoreCase = !DynamicExpression.IsLanguageCaseSensitive(DynamicExpression.Language);
        }

        public IExpression Resolve(IAstNode node)
        {
            return node.Accept(new BindingVisitor(this));
        }
    }
}
