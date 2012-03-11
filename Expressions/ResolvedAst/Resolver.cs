using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Expressions.Ast;
using System.Reflection;

namespace Expressions.ResolvedAst
{
    internal class Resolver
    {
        private readonly DynamicExpression _dynamicExpression;
        private readonly Type _ownerType;
        private readonly Import[] _imports;
        private readonly Type[] _identifierTypes;
        private readonly int[] _identifierIndexes;
        private readonly bool _ignoreCase;

        public Resolver(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes, int[] parameterMap)
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

            // Inverse the parameter map.

            _identifierIndexes = new int[_identifierTypes.Length];

            for (int i = 0; i < _identifierTypes.Length; i++)
            {
                _identifierIndexes[i] = -1;

                if (_identifierTypes[i] != null)
                {
                    for (int j = 0; j < parameterMap.Length; j++)
                    {
                        if (parameterMap[j] == i)
                        {
                            _identifierIndexes[i] = j;
                            break;
                        }
                    }

                    Debug.Assert(_identifierIndexes[i] != -1);
                }
            }

            _ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);
        }

        public Type ResolveType(string type, int arrayIndex)
        {
            string builtInType = type;

            if (_ignoreCase)
                builtInType = builtInType.ToLowerInvariant();

            var result = TypeUtil.GetBuiltInType(builtInType);

            if (result == null)
            {
                result = Type.GetType(type, false);

                if (result == null)
                    throw new NotSupportedException(String.Format("Unknown type '{0}'", type));
            }

            if (arrayIndex > 0)
                result = result.MakeArrayType(arrayIndex);

            return result;
        }

        public IResolvedIdentifier Resolve(IResolvedIdentifier operand, Type type, string member, bool isStatic)
        {
            return Resolve(operand, type, member, isStatic, true);
        }

        private IResolvedIdentifier Resolve(IResolvedIdentifier operand, Type type, string member, bool isStatic, bool throwOnError)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (member == null)
                throw new ArgumentNullException("member");

            // First try properties and methods.

            var methods = type.GetMethods(
                (isStatic ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.Public
            );

            var propertyMethods = new List<MethodInfo>();
            var normalMethods = new List<MethodInfo>();
            string propertyMember = "get_" + member;

            foreach (var method in methods)
            {
                // We do not check for IsSpecialName. NHibernate is a known
                // bug where generated properties do not have IsSpecialName set,
                // and it shouldn't be an issue.

                if (IdentifiersEqual(method.Name, propertyMember))
                    propertyMethods.Add(method);
                else if (IdentifiersEqual(method.Name, member))
                    normalMethods.Add(method);
            }

            if (propertyMethods.Count == 1)
                return new PropertyIdentifier(operand, propertyMethods[0]);
            else if (propertyMethods.Count > 1)
                return new PropertyGroupIdentifier(operand, propertyMethods.ToArray());
            else if (normalMethods.Count == 1)
                return new MethodIdentifier(operand, normalMethods[0]);
            else if (normalMethods.Count > 1)
                return new MethodGroupIdentifier(operand, normalMethods.ToArray());

            // Next, try for a field.

            var field = type.GetField(
                member,
                (_ignoreCase ? BindingFlags.IgnoreCase : 0) |
                (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                BindingFlags.Public
            );

            if (field != null)
                return new FieldIdentifier(field);

            // Nothing matched.

            if (throwOnError)
                throw new NotSupportedException(String.Format("Cannot resolve '{0}'", member));

            return null;
        }

        public IResolvedIdentifier ResolveGlobal(string member)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            // First try variables.

            var identifiers = _dynamicExpression.ParseResult.Identifiers;

            for (int i = 0; i < _identifierTypes.Length; i++)
            {
                if (
                    _identifierTypes[i] != null &&
                    IdentifiersEqual(identifiers[i].Name, member)
                )
                    return new VariableIdentifier(_identifierTypes[i], _identifierIndexes[i]);
            }

            // Next, we go through the owner type.

            if (_ownerType != null)
            {
                var result = Resolve(new VariableIdentifier(_ownerType, 0), _ownerType, member, false, false);

                if (result == null)
                    Resolve(new TypeIdentifier(_ownerType), _ownerType, member, true, false);

                if (result != null)
                    return result;
            }

            // Next, imports. Namespaces have precedence.

            foreach (var import in _imports)
            {
                if (import.Namespace != null && IdentifiersEqual(import.Namespace, member))
                    return new ImportIdentifier(import);
            }

            // And last, members of the imports.

            foreach (var import in _imports)
            {
                if (import.Namespace == null)
                {
                    var result = Resolve(new TypeIdentifier(import.Type), import.Type, member, true, false);

                    if (result != null)
                        return result;
                }
            }

            // Not resolving a global identifier is not an error and will simply
            // result in a null constant.

            return NullIdentifier.Instance;
        }

        private bool IdentifiersEqual(string a, string b)
        {
            return String.Equals(
                a, b, _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal
            );
        }

        public Type ResolveExpressionType(Type left, Type right, ExpressionType type)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            if (left == right)
                return left;

            // TODO: Implicit/explicit operators and operators for the expression type.

            // Special cast for adding strings.

            if (type == ExpressionType.Add && (left == typeof(string) || right == typeof(string)))
                return typeof(string);

            // Can we cast implicitly?

            var leftTable = TypeUtil.GetImplicitCastingTable(left);
            var rightTable = TypeUtil.GetImplicitCastingTable(right);

            if (leftTable != null && rightTable != null)
            {
                for (int i = 0; i < leftTable.Count; i++)
                {
                    if (leftTable[i] == right)
                        return leftTable[i];

                    for (int j = 0; j < rightTable.Count; j++)
                    {
                        if (leftTable[i] == rightTable[j])
                            return leftTable[i];
                    }
                }
            }

            // We can't cast implicitly.

            throw new NotSupportedException(String.Format("Cannot implicitly cast {0} and {1}", left, right));
        }

        internal static MethodInfo ResolveMethodGroup(IList<MethodInfo> methods, IResolvedAstNode[] arguments)
        {
            var argumentTypes = new Type[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                argumentTypes[i] = arguments[i].Type;
            }

            // Get all methods with the correct number of parameters.

            var candidates = GetCandidates(methods, argumentTypes.Length);

            if (candidates.Count == 0)
                throw new NotSupportedException(String.Format("Cannot resolve method {0}", methods[0].Name));

            if (candidates.Count == 1)
                return candidates[0];

            // See if we have an exact match.

            foreach (var candidate in candidates)
            {
                var parameters = candidate.GetParameters();

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == argumentTypes[i])
                        return candidate;
                }
            }

            // See if we can find a match with implicit casting.

            foreach (var candidate in candidates)
            {
                var parameters = candidate.GetParameters();
                bool success = true;

                for (int i = 0; i < parameters.Length; i++)
                {
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
                        if (!parameters[i].ParameterType.IsAssignableFrom(argumentTypes[i]))
                        {
                            success = false;
                            break;
                        }
                    }
                }

                if (success)
                    return candidate;
            }

            throw new NotSupportedException(String.Format("Cannot resolve method {0}", methods[0].Name));
        }

        private static List<MethodInfo> GetCandidates(IEnumerable<MethodInfo> methods, int arguments)
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
