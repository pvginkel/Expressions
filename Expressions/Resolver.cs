using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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
            return node
                .Accept(new BindingVisitor(this))
                .Accept(new ConstantParsingVisitor())
                .Accept(new ConversionVisitor(this));
        }

        public Expressions.MethodCall ResolveMethod(IExpression operand, string name, IExpression[] arguments)
        {
            bool isStatic = operand is TypeAccess;

            var methods = operand.Type.GetMethods(
                DynamicExpression.Options.AccessBindingFlags |
                (isStatic ? BindingFlags.Static : BindingFlags.Instance)
            );

            var candidates = new List<MethodInfo>();

            foreach (var method in methods)
            {
                if (IdentifiersEqual(method.Name, name))
                    candidates.Add(method);
            }

            if (candidates.Count > 0)
            {
                var method = ResolveMethodGroup(candidates, arguments);

                return new Expressions.MethodCall(operand, method, arguments);
            }

            return null;
        }

        public bool IdentifiersEqual(string a, string b)
        {
            return String.Equals(
                a, b, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
            );
        }

        private MethodInfo ResolveMethodGroup(IList<MethodInfo> methods, IExpression[] arguments)
        {
            var argumentTypes = new Type[arguments == null ? 0 : arguments.Length];
            var argumentsNull = new bool[argumentTypes.Length];

            for (int i = 0; i < argumentTypes.Length; i++)
            {
                argumentTypes[i] = arguments[i].Type;

                var constant = arguments[i] as Expressions.Constant;

                argumentsNull[i] = constant != null && constant.Value == null;
            }

            // Get all methods with the correct number of parameters.

            var candidates = GetCandidates(methods, argumentTypes.Length);

            if (candidates.Count == 0)
                throw new NotSupportedException(String.Format("Cannot resolve method {0}", methods[0].Name));

            if (candidates.Count == 1)
                return candidates[0];

            var matchedCandidates = new List<MethodInfo>();

            foreach (var candidate in candidates)
            {
                // See if we have an exact match.

                var parameters = candidate.GetParameters();

                bool success = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != argumentTypes[i])
                        success = false;
                }

                if (success)
                    return candidate;

                // See if we can find a match with implicit casting.

                success = true;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (
                        parameters[i].ParameterType == typeof(object) ||
                        parameters[i].ParameterType == argumentTypes[i]
                    )
                        continue;

                    var castingTable = TypeUtil.GetImplicitCastingTable(argumentTypes[i]);

                    if (castingTable != null)
                    {
                        bool found = false;

                        for (int j = 0; j < castingTable.Count; j++)
                        {
                            if (castingTable[j] == parameters[i].ParameterType)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            success = false;
                            break;
                        }
                    }
                    else
                    {
                        if (
                            !parameters[i].ParameterType.IsAssignableFrom(argumentTypes[i]) &&
                            !(!parameters[i].ParameterType.IsValueType && argumentsNull[i])
                        )
                        {
                            success = false;
                            break;
                        }
                    }
                }

                if (success)
                    matchedCandidates.Add(candidate);
            }

            if (matchedCandidates.Count != 1)
                throw new NotSupportedException(String.Format("Cannot resolve method {0}", methods[0].Name));

            return matchedCandidates[0];
        }

        private List<MethodInfo> GetCandidates(IEnumerable<MethodInfo> methods, int arguments)
        {
            var candidates = new List<MethodInfo>();

            foreach (var method in methods)
            {
                if (method.GetParameters().Length == arguments)
                    candidates.Add(method);
            }

            return candidates;
        }
    }
}
