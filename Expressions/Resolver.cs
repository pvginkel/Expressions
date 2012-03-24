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

        public BoundExpressionOptions Options { get; private set; }

        public int[] IdentifierIndexes { get; private set; }

        public Type OwnerType { get; private set; }

        public Import[] Imports { get; private set; }

        public bool IgnoreCase { get; private set; }

        public Resolver(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes, int[] parameterMap, BoundExpressionOptions options)
        {
            Require.NotNull(dynamicExpression, "dynamicExpression");
            Require.NotNull(imports, "imports");
            Require.NotNull(identifierTypes, "identifierTypes");
            Require.NotNull(parameterMap, "parameterMap");

            DynamicExpression = dynamicExpression;
            OwnerType = ownerType;
            Imports = imports;
            IdentifierTypes = identifierTypes;
            Options = options;

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
                Options.AccessBindingFlags |
                (isStatic ? BindingFlags.Static : BindingFlags.Instance)
            );

            var candidates = new List<MethodBase>();

            foreach (var method in methods)
            {
                if (IdentifiersEqual(method.Name, name))
                    candidates.Add(method);
            }

            if (candidates.Count > 0)
            {
                var method = ResolveMethodGroup(candidates, arguments);

                if (method == null)
                    throw new NotSupportedException(String.Format("Cannot resolve method {0}", candidates[0].Name));

                return new Expressions.MethodCall(operand, (MethodInfo)method, arguments);
            }

            return null;
        }

        public bool IdentifiersEqual(string a, string b)
        {
            return String.Equals(
                a, b, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
            );
        }

        public MethodBase ResolveMethodGroup(IList<MethodBase> methods, IExpression[] arguments)
        {
            var argumentTypes = new Type[arguments == null ? 0 : arguments.Length];
            var argumentsNull = new bool[argumentTypes.Length];

            for (int i = 0; i < argumentTypes.Length; i++)
            {
                argumentTypes[i] = arguments[i].Type;

                var constant = arguments[i] as Expressions.Constant;

                argumentsNull[i] = constant != null && constant.Value == null;
            }

            return ResolveMethodGroup(methods, argumentTypes, argumentsNull);
        }

        public MethodBase ResolveMethodGroup(IList<MethodBase> methods, IList<Type> argumentTypes, IList<bool> argumentsNull)
        {
            // Get all methods with the correct number of parameters.

            var candidates = GetCandidates(methods, argumentTypes.Count);

            if (candidates.Count == 0)
                return null;

            if (candidates.Count == 1)
                return candidates[0];

            var matchedCandidates = new List<MethodBase>();

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
                    success = success && TypeUtil.CanCastImplicitely(
                        argumentTypes[i],
                        parameters[i].ParameterType,
                        argumentsNull[i]
                    );
                }

                if (success)
                    matchedCandidates.Add(candidate);
            }

            if (matchedCandidates.Count != 1)
                return null;

            return matchedCandidates[0];
        }

        private List<MethodBase> GetCandidates(IEnumerable<MethodBase> methods, int arguments)
        {
            var candidates = new List<MethodBase>();

            foreach (var method in methods)
            {
                if (method.GetParameters().Length == arguments)
                    candidates.Add(method);
            }

            return candidates;
        }
    }
}
