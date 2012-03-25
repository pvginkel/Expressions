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
                .Accept(new ConstantParsingVisitor(this))
                .Accept(new BindingVisitor(this))
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
            var matches = new List<MethodBase>();
            var implicitMatches = new List<MethodBase>();
            var paramsMatches = new List<MethodBase>();
            var implicitParamsMatches = new List<MethodBase>();

            foreach (var method in methods)
            {
                // See if we have an exact match.

                var parameters = method.GetParameters();

                bool paramsMethod =
                    parameters.Length > 0 &&
                    parameters[parameters.Length - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length == 1;

                int mandatoryParameterCount =
                    paramsMethod
                    ? parameters.Length - 1
                    : parameters.Length;

                if (paramsMethod)
                {
                    if (argumentTypes.Count < mandatoryParameterCount)
                        continue;
                }
                else
                {
                    if (argumentTypes.Count != mandatoryParameterCount)
                        continue;
                }

                bool match = true;
                bool implicitMatch = true;

                for (int i = 0; i < mandatoryParameterCount; i++)
                {
                    TestArgumentType(
                        parameters[i].ParameterType,
                        argumentTypes[i],
                        argumentsNull != null && argumentsNull[i],
                        ref match,
                        ref implicitMatch
                    );
                }

                if (paramsMethod)
                {
                    if (argumentTypes.Count > mandatoryParameterCount)
                    {
                        bool nullParamsArgument =
                            argumentTypes.Count == parameters.Length &&
                            argumentsNull != null &&
                            argumentsNull[parameters.Length - 1];

                        if (!nullParamsArgument)
                        {
                            bool matched = false;

                            if (argumentTypes.Count == parameters.Length)
                            {
                                bool paramsMatch = true;
                                bool implicitParamsMatch = true;

                                TestArgumentType(
                                    parameters[parameters.Length - 1].ParameterType,
                                    argumentTypes[argumentTypes.Count - 1],
                                    false,
                                    ref paramsMatch,
                                    ref implicitParamsMatch
                                );

                                if (paramsMatch || implicitParamsMatch)
                                {
                                    match = paramsMatch;
                                    implicitMatch = implicitParamsMatch;
                                    matched = true;
                                }
                            }

                            if (!matched)
                            {
                                var parameterType = parameters[parameters.Length - 1].ParameterType.GetElementType();

                                for (int i = mandatoryParameterCount; i < argumentTypes.Count; i++)
                                {
                                    TestArgumentType(
                                        parameterType,
                                        argumentTypes[i],
                                        argumentsNull != null && argumentsNull[i],
                                        ref match,
                                        ref implicitMatch
                                    );
                                }
                            }
                        }
                    }

                    if (match)
                        paramsMatches.Add(method);
                    if (implicitMatch)
                        implicitParamsMatches.Add(method);
                }
                else
                {
                    if (match)
                        matches.Add(method);
                    if (implicitMatch)
                        implicitMatches.Add(method);
                }
            }

            if (matches.Count == 1)
                return matches[0];
            else if (matches.Count > 1)
                return null;

            if (paramsMatches.Count == 1)
                return paramsMatches[0];
            else if (paramsMatches.Count > 1)
                return null;

            if (implicitMatches.Count == 1)
                return implicitMatches[0];
            else if (implicitMatches.Count > 1)
                return null;

            if (implicitParamsMatches.Count == 1)
                return implicitParamsMatches[0];

            return null;
        }

        private void TestArgumentType(Type parameterType, Type argumentType, bool argumentNull, ref bool match, ref bool implicitMatch)
        {
            if (
                !parameterType.IsValueType &&
                argumentNull
            )
                return;

            if (parameterType != argumentType)
                match = false;

            if (!TypeUtil.CanCastImplicitely(
                argumentType,
                parameterType,
                argumentNull
            ))
                implicitMatch = false;
        }

        internal MethodInfo FindOperatorMethod(string methodName, Type[] sourceTypes, Type returnType, Type[] parameterTypes)
        {
            return FindOperatorMethod(methodName, sourceTypes, returnType, parameterTypes, null);
        }

        internal MethodInfo FindOperatorMethod(string methodName, Type[] sourceTypes, Type returnType, Type[] parameterTypes, bool[] parametersNull)
        {
            var candidates = new List<MethodInfo>();

            foreach (var sourceType in sourceTypes)
            {
                foreach (var method in sourceType.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (method.Name != methodName)
                        continue;

                    var parameters = method.GetParameters();

                    if (parameters.Length != parameterTypes.Length)
                        continue;

                    bool match = true;
                    bool implicitMatch = true;

                    if (returnType != null)
                    {
                        match = returnType == method.ReturnType;
                        implicitMatch = TypeUtil.CanCastImplicitely(
                            method.ReturnType, returnType, false
                        );
                    }

                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (parameters[i].ParameterType != parameterTypes[i])
                            match = false;

                        if (!TypeUtil.CanCastImplicitely(
                            parameterTypes[i],
                            parameters[i].ParameterType,
                            parametersNull != null && parametersNull[i]
                        ))
                            implicitMatch = false;
                    }

                    if (match)
                        return method;
                    if (implicitMatch)
                        candidates.Add(method);
                }
            }

            return candidates.Count == 1 ? candidates[0] : null;
        }
    }
}
