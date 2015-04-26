using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Expressions.Ast;
using Expressions.Expressions;
using BinaryExpression = Expressions.Ast.BinaryExpression;

namespace Expressions
{
    internal class BindingVisitor : IAstVisitor<IExpression>
    {
        private readonly Resolver _resolver;

        public BindingVisitor(Resolver resolver)
        {
            Require.NotNull(resolver, "resolver");

            _resolver = resolver;
        }

        private static string GetOperatorName(Type left, Type right, ExpressionType type)
        {
            string operatorName = null;

            switch (type)
            {
                case ExpressionType.Add: operatorName = "op_Addition"; break;
                case ExpressionType.Divide: operatorName = "op_Division"; break;
                case ExpressionType.Multiply: operatorName = "op_Multiply"; break;
                case ExpressionType.Subtract: operatorName = "op_Subtraction"; break;
                case ExpressionType.Equals: operatorName = "op_Equality"; break;
                case ExpressionType.NotEquals: operatorName = "op_Inequality"; break;
                case ExpressionType.Greater: operatorName = "op_GreaterThan"; break;
                case ExpressionType.GreaterOrEquals: operatorName = "op_GreaterThanOrEqual"; break;
                case ExpressionType.Less: operatorName = "op_LessThan"; break;
                case ExpressionType.LessOrEquals: operatorName = "op_LessThanOrEqual"; break;
                case ExpressionType.ShiftLeft: operatorName = "op_LeftShift"; break;
                case ExpressionType.ShiftRight: operatorName = "op_RightShift"; break;
                case ExpressionType.Modulo: operatorName = "op_Modulus"; break;
                case ExpressionType.LogicalAnd: operatorName = "op_LogicalAnd"; break;
                case ExpressionType.AndBoth: operatorName = "op_LogicalAnd"; break;
                case ExpressionType.LogicalOr: operatorName = "op_LogicalOr"; break;
                case ExpressionType.OrBoth: operatorName = "op_LogicalOr"; break;
                case ExpressionType.BitwiseAnd: operatorName = "op_BitwiseAnd"; break;
                case ExpressionType.BitwiseOr: operatorName = "op_BitwiseOr"; break;

                case ExpressionType.And:
                    if (left == typeof(bool) && right == typeof(bool))
                        operatorName = "op_LogicalAnd";
                    else
                        operatorName = "op_BitwiseAnd";
                    break;

                case ExpressionType.Or:
                    if (left == typeof(bool) && right == typeof(bool))
                        operatorName = "op_LogicalOr";
                    else
                        operatorName = "op_BitwiseOr";
                    break;

                case ExpressionType.Xor:
                    operatorName = "op_ExclusiveOr";
                    break;
            }

            return operatorName;
        }

        public IExpression BinaryExpression(Ast.BinaryExpression binaryExpression)
        {
            // In expressions are converted to method calls.

            if (binaryExpression.Type == ExpressionType.In)
                return BinaryInExpression(binaryExpression);

            // We need to do this here and not in the conversion phase because
            // we need the return type of the method to fully bind the tree.

            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            string operatorName = GetOperatorName(left.Type, right.Type, binaryExpression.Type);

            Type commonType = FindCommonType(left.Type, right.Type);

            var types = new List<Type>();
            if (commonType != null)
                types.Add(commonType);
            types.AddRange(GetTypeHierarchy(left.Type));
            types.AddRange(GetTypeHierarchy(right.Type));
            types = ListExtension<Type>.Distinct(types);
            
            if (operatorName != null)
            {
                var method = _resolver.FindOperatorMethod(
                    operatorName,
                    types.ToArray(),
                    null,
                    new[] { left.Type, right.Type },
                    new[] { left is Expressions.Constant && ((Expressions.Constant)left).Value == null, right is Expressions.Constant && ((Expressions.Constant)right).Value == null }
                );

                if (method != null)
                {
                    return new Expressions.MethodCall(
                        new TypeAccess(typeof(string)),
                        method,
                        new[]
                        {
                            left,
                            right
                        }
                    );
                }
            }

            switch (binaryExpression.Type)
            {
                case ExpressionType.ShiftLeft:
                case ExpressionType.ShiftRight:
                    if (!TypeUtil.IsInteger(left.Type))
                        throw new ExpressionsException("Left operand of shift operations must be of an integer type", ExpressionsExceptionType.TypeMismatch);
                    if (right.Type != typeof(int) && right.Type != typeof(byte))
                        throw new ExpressionsException("Right operand of shift operations must be integer or byte type", ExpressionsExceptionType.TypeMismatch);
                    commonType = left.Type;
                    break;

                case ExpressionType.Power:
                    commonType = typeof(double);
                    break;

                case ExpressionType.Greater:
                case ExpressionType.GreaterOrEquals:
                case ExpressionType.Less:
                case ExpressionType.LessOrEquals:
                    commonType = ResolveExpressionCommonType(left.Type, right.Type, false, true, false);
                    break;

                case ExpressionType.Equals:
                case ExpressionType.NotEquals:
                case ExpressionType.Compares:
                case ExpressionType.NotCompares:
                    if (left.Type.IsValueType != right.Type.IsValueType)
                        throw new ExpressionsException("Cannot resolve expression type", ExpressionsExceptionType.TypeMismatch);

                    commonType = ResolveExpressionCommonType(left.Type, right.Type, false, false, true);
                    break;

                default:
                    commonType = ResolveExpressionCommonType(left.Type, right.Type, binaryExpression.Type == ExpressionType.Add, false, false);
                    break;
            }
            
            var type = ResolveExpressionType(left.Type, right.Type, commonType, binaryExpression.Type);

            var expressionType = binaryExpression.Type;

            if (type != typeof(bool))
            {
                if (expressionType == ExpressionType.AndBoth)
                    expressionType = ExpressionType.BitwiseAnd;
                else if (expressionType == ExpressionType.OrBoth)
                    expressionType = ExpressionType.BitwiseOr;
            }

            return new Expressions.BinaryExpression(left, right, expressionType, type, commonType);
        }

        private IExpression BinaryInExpression(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Right is AstNodeCollection)
            {
                var arguments = new List<IExpression>();

                arguments.Add(binaryExpression.Left.Accept(this));

                foreach (var argument in ((AstNodeCollection)binaryExpression.Right).Nodes)
                {
                    arguments.Add(argument.Accept(this));
                }

                return new Expressions.MethodCall(
                    new TypeAccess(typeof(CompilerUtil)),
                    typeof(CompilerUtil).GetMethod("InSet"),
                    arguments
                );
            }
            else
            {
                var set = binaryExpression.Right.Accept(this);

                if (typeof(IEnumerable).IsAssignableFrom(set.Type))
                {
                    return new Expressions.MethodCall(
                        new TypeAccess(typeof(CompilerUtil)),
                        typeof(CompilerUtil).GetMethod("InEnumerable"),
                        new[]
                        {
                            binaryExpression.Left.Accept(this),
                            set
                        }
                    );
                }
                else
                {
                    throw new ExpressionsException(
                        "Right operand of set must be an enumerable",
                         ExpressionsExceptionType.TypeMismatch
                        );
                }
            }
        }

        public IExpression Cast(Ast.Cast cast)
        {
            return new Expressions.Cast(
                cast.Operand.Accept(this),
                ResolveType(cast.Type.Name, cast.Type.ArrayIndex),
                cast.CastType
            );
        }

        public IExpression Constant(Ast.Constant constant)
        {
            return new Expressions.Constant(constant.Value);
        }

        public IExpression IdentifierAccess(IdentifierAccess identifierAccess)
        {
            // First try variables.

            var identifiers = _resolver.DynamicExpression.ParseResult.Identifiers;

            for (int i = 0; i < _resolver.IdentifierTypes.Length; i++)
            {
                if (
                    _resolver.IdentifierTypes[i] != null &&
                    _resolver.IdentifiersEqual(identifiers[i].Name, identifierAccess.Name)
                )
                    return VariableAccess(_resolver.IdentifierIndexes[i], _resolver.IdentifierTypes[i]);
            }

            // Next, we go through the owner type.

            if (_resolver.OwnerType != null)
            {
                var result = Resolve(new VariableAccess(_resolver.OwnerType, 0), identifierAccess.Name);

                if (result == null)
                    result = Resolve(new TypeAccess(_resolver.OwnerType), identifierAccess.Name);

                if (result != null)
                    return result;
            }

            // Next, imports. Namespaces have precedence.

            foreach (var import in _resolver.Imports)
            {
                if (
                    import.Namespace != null &&
                    _resolver.IdentifiersEqual(import.Namespace, identifierAccess.Name)
                ) {
                    if (import.Type != null)
                        return new TypeAccess(import.Type);
                    else
                        return new ImportAccess(import);
                }
            }

            // Next, members of the imports.

            foreach (var import in _resolver.Imports)
            {
                if (import.Namespace == null)
                {
                    var result = Resolve(new TypeAccess(import.Type), identifierAccess.Name);

                    if (result != null)
                        return result;
                }
            }

            // Last, see whether the identifier is a built in type.

            var type = TypeUtil.GetBuiltInType(identifierAccess.Name, _resolver.DynamicExpression.Language);

            if (type != null)
                return new TypeAccess(type);

            // Call out to handler if not null
            if (_resolver.TypeResolver != null)
                type = _resolver.TypeResolver.GetVariableType(identifierAccess.Name, _resolver.IgnoreCase);

            if (type != null)
                return new TypeAccess(type);

            throw new ExpressionsException(
                String.Format("Unresolved identifier '{0}'", identifierAccess.Name),
                ExpressionsExceptionType.UndefinedName
            );
        }

        private static IExpression VariableAccess(int identifierIndex, Type identifierType)
        {
            var variableAccess = new VariableAccess(identifierType, identifierIndex);

            if (typeof(DynamicExpression).IsAssignableFrom(identifierType))
            {
                var methodCall = new Expressions.MethodCall(
                    variableAccess,
                    typeof(DynamicExpression).GetMethod("Invoke", new Type[0]),
                    null
                );

                if (variableAccess.Type.IsGenericType)
                {
                    var resultType = identifierType.GetGenericArguments()[0];

                    return new Expressions.Cast(methodCall, resultType);
                }
                else
                {
                    return methodCall;
                }
            }
            else if (typeof(IBoundExpression).IsAssignableFrom(identifierType))
            {
                var methodCall = new Expressions.MethodCall(
                    variableAccess,
                    typeof(IBoundExpression).GetMethod("Invoke", new Type[0]),
                    null
                );

                foreach (var @interface in identifierType.GetInterfaces())
                {
                    if (
                        @interface.IsGenericType &&
                        @interface.GetGenericTypeDefinition() == typeof(IBoundExpression<>)
                    ) {
                        var resultType = identifierType.GetGenericArguments()[0];

                        return new Expressions.Cast(methodCall, resultType);
                    }
                }

                return methodCall;
            }
            else
            {
                return variableAccess;
            }
        }

        private IExpression Resolve(IExpression operand, string member)
        {
            bool isStatic = operand is TypeAccess;

            // First try properties and methods.

            var methods = operand.Type.GetMethods(
                _resolver.Options.AccessBindingFlags |
                (isStatic ? BindingFlags.Static : BindingFlags.Instance)
            );

            var propertyMethods = new List<MethodInfo>();
            string propertyMember = "get_" + member;

            foreach (var method in methods)
            {
                // We do not check for IsSpecialName. NHibernate is a known
                // bug where generated properties do not have IsSpecialName set,
                // and it shouldn't be an issue.

                if (_resolver.IdentifiersEqual(method.Name, propertyMember))
                    propertyMethods.Add(method);
            }

            if (propertyMethods.Count == 1)
                return new Expressions.MethodCall(operand, propertyMethods[0], null);
            else if (propertyMethods.Count > 1)
                return null;

            // Next, try for a field.

            var field = operand.Type.GetField(
                member,
                (_resolver.IgnoreCase ? BindingFlags.IgnoreCase : 0) |
                (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                _resolver.Options.AccessBindingFlags
            );

            if (field != null)
            {
                if (field.IsLiteral)
                    return new Expressions.Constant(field.GetValue(null));
                else
                    return new FieldAccess(operand, field);
            }

            // Nothing matched.

            return null;
        }

        public IExpression Index(Ast.Index index)
        {
            return Index(index.Operand, index.Arguments);
        }

        private IExpression Index(IAstNode operand, AstNodeCollection arguments)
        {
            var resolvedArguments = ResolveArguments(arguments);

            var resolvedOperand = operand.Accept(this);

            if (resolvedOperand.Type.IsArray)
            {
                if (resolvedArguments.Length != resolvedOperand.Type.GetArrayRank())
                {
                    throw new ExpressionsException(
                        "Invalid array index rank",
                        ExpressionsExceptionType.TypeMismatch
                    );
                }

                foreach (var argument in resolvedArguments)
                {
                    if (!TypeUtil.IsCastAllowed(argument.Type, typeof(int)))
                    {
                        throw new ExpressionsException(
                            "Argument of array index must be convertable to integer",
                            ExpressionsExceptionType.TypeMismatch
                        );
                    }
                }

                if (resolvedArguments.Length == 1)
                    return new Expressions.Index(resolvedOperand, resolvedArguments[0], resolvedOperand.Type.GetElementType());
                else
                    return _resolver.ResolveMethod(resolvedOperand, "Get", resolvedArguments);
            }

            var defaultMemberAttributes = resolvedOperand.Type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);

            if (defaultMemberAttributes.Length != 1)
            {
                throw new ExpressionsException(
                    "Operand does not support indexing",
                    ExpressionsExceptionType.TypeMismatch
                );
            }

            var result = _resolver.ResolveMethod(resolvedOperand, "get_" + ((DefaultMemberAttribute)defaultMemberAttributes[0]).MemberName, resolvedArguments);

            if (result == null)
            {
                throw new ExpressionsException(
                    "Unresolved index method",
                    ExpressionsExceptionType.UnresolvedMethod
                );
            }

            return result;
        }

        public IExpression MemberAccess(MemberAccess memberAccess)
        {
            var operand = memberAccess.Operand.Accept(this);

            if (operand is ImportAccess)
            {
                Debug.Assert(((ImportAccess)operand).Import.Type == null);

                foreach (var import in ((ImportAccess)operand).Import.Imports)
                {
                    if (
                        import.Namespace != null &&
                        _resolver.IdentifiersEqual(memberAccess.Member, import.Namespace)
                    )
                    {
                        if (import.Type != null)
                            return new TypeAccess(import.Type);
                        else
                            return new ImportAccess(import);
                    }

                    if (import.Type != null)
                    {
                        var importResult = Resolve(new TypeAccess(import.Type), memberAccess.Member);

                        if (importResult != null)
                            return importResult;
                    }
                }
            }
            else
            {

                var result = Resolve(operand, memberAccess.Member);

                if (result != null)
                    return result;
            }

            throw new ExpressionsException(
                String.Format("Unresolved identifier '{0}'", memberAccess.Member),
                ExpressionsExceptionType.UndefinedName
            );
        }

        public IExpression MethodCall(Ast.MethodCall methodCall)
        {
            var arguments = ResolveArguments(methodCall.Arguments);

            var identifierAccess = methodCall.Operand as IdentifierAccess;

            if (identifierAccess != null)
            {
                var result = ResolveGlobalMethod(identifierAccess.Name, arguments);

                if (result != null)
                    return result;

                if (_resolver.DynamicExpression.Language == ExpressionLanguage.VisualBasic)
                    return Index(methodCall.Operand, methodCall.Arguments);

                throw new ExpressionsException(
                    String.Format("Cannot resolve symbol '{0}'", identifierAccess.Name),
                    ExpressionsExceptionType.UndefinedName
                );
            }

            var memberAccess = methodCall.Operand as MemberAccess;

            if (memberAccess != null)
            {
                var operand = memberAccess.Operand.Accept(this);

                if (operand is ImportAccess)
                {
                    foreach (var import in ((ImportAccess)operand).Import.Imports)
                    {
                        if (import.Type != null)
                        {
                            var result = _resolver.ResolveMethod(
                                new TypeAccess(import.Type), memberAccess.Member, arguments
                            );

                            if (result != null)
                                return result;
                        }
                    }
                }
                else
                {
                    var result = _resolver.ResolveMethod(operand, memberAccess.Member, arguments);

                    if (result != null)
                        return result;
                }
            }

            if (_resolver.DynamicExpression.Language == ExpressionLanguage.VisualBasic)
                return Index(methodCall.Operand, methodCall.Arguments);

            throw new ExpressionsException("Cannot resolve symbol", ExpressionsExceptionType.UndefinedName);
        }

        private IExpression[] ResolveArguments(AstNodeCollection astArguments)
        {
            var arguments = new IExpression[astArguments == null ? 0 : astArguments.Nodes.Count];

            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = astArguments.Nodes[i].Accept(this);
            }
            return arguments;
        }

        private Expressions.MethodCall ResolveGlobalMethod(string name, IExpression[] arguments)
        {
            // Try methods on the owner type.

            if (_resolver.OwnerType != null)
            {
                var result = _resolver.ResolveMethod(new TypeAccess(_resolver.OwnerType), name, arguments);

                if (result == null)
                    result = _resolver.ResolveMethod(new VariableAccess(_resolver.OwnerType, 0), name, arguments);

                if (result != null)
                    return result;
            }

            // Try static methods on the imports.

            foreach (var import in _resolver.Imports)
            {
                if (import.Type != null)
                {
                    var result = _resolver.ResolveMethod(new TypeAccess(import.Type), name, arguments);

                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        public IExpression UnaryExpression(Ast.UnaryExpression unaryExpression)
        {
            var operand = unaryExpression.Operand.Accept(this);

            string operatorName = null;

            switch (unaryExpression.Type)
            {
                case ExpressionType.Plus: operatorName = "op_UnaryPlus"; break;
                case ExpressionType.Minus: operatorName = "op_UnaryNegation"; break;
                case ExpressionType.Not:
                case ExpressionType.LogicalNot:
                    operatorName = "op_Negation"; break;
            }

            if (operatorName != null)
            {
                var method = _resolver.FindOperatorMethod(
                    operatorName,
                    new[] { operand.Type },
                    null,
                    new[] { operand.Type }
                );

                if (method != null)
                {
                    return new Expressions.MethodCall(
                        new TypeAccess(method.DeclaringType),
                        method,
                        new[]
                        {
                            operand
                        }
                    );
                }
            }

            Type type;

            switch (unaryExpression.Type)
            {
                case ExpressionType.Plus:
                    if (!TypeUtil.IsValidUnaryArgument(operand.Type))
                        throw new ExpressionsException("Operand of plus operation must be an integer type", ExpressionsExceptionType.TypeMismatch);

                    type = operand.Type;
                    break;

                case ExpressionType.Group:
                    type = operand.Type;
                    break;

                case ExpressionType.Minus:
                    if (!TypeUtil.IsValidUnaryArgument(operand.Type))
                        throw new ExpressionsException("Operand of minus operation must be an integer type", ExpressionsExceptionType.TypeMismatch);

                    // TODO: Make constants signed and handle minus on unsigned's.

                    type = operand.Type;
                    break;

                case ExpressionType.LogicalNot:
                    if (operand.Type != typeof(bool))
                        throw new ExpressionsException("Operand of not operation must be a boolean type", ExpressionsExceptionType.TypeMismatch);

                    type = typeof(bool);
                    break;

                case ExpressionType.BitwiseNot:
                    if (!TypeUtil.IsInteger(operand.Type))
                    {
                        throw new ExpressionsException(
                            "Cannot not bitwise not boolean types",
                            ExpressionsExceptionType.TypeMismatch
                        );
                    }

                    type = operand.Type;
                    break;

                case ExpressionType.Not:
                    if (TypeUtil.IsInteger(operand.Type))
                    {
                        type = operand.Type;
                    }
                    else
                    {
                        if (operand.Type != typeof(bool))
                            throw new ExpressionsException("Operand of not operation must be a boolean type", ExpressionsExceptionType.TypeMismatch);

                        type = typeof(bool);
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return new Expressions.UnaryExpression(operand, type, unaryExpression.Type);
        }

        private Type ResolveType(string type, int arrayIndex)
        {
            var result = TypeUtil.GetBuiltInType(type, _resolver.DynamicExpression.Language);

            if (result == null)
                result = Type.GetType(type, false, _resolver.IgnoreCase);

            // This deviates from the specs but allows us to not include
            // boolean and single in the known types table.

            if (result == null)
                result = Type.GetType("System." + type, false, _resolver.IgnoreCase);

            if (result == null)
                result = FindTypeInImports(type);

            if (result == null)
            {
                throw new ExpressionsException(
                    String.Format("Unknown type '{0}'", type),
                    ExpressionsExceptionType.InvalidExplicitCast
                );
            }

            if (arrayIndex == 1)
                result = result.MakeArrayType();
            else if (arrayIndex > 1)
                result = result.MakeArrayType(arrayIndex);

            return result;
        }

        private Type FindTypeInImports(string type)
        {
            foreach (var import in _resolver.Imports)
            {
                var result = FindTypeInImport(import, type);

                if (result != null)
                    return result;
            }

            return null;
        }

        private Type FindTypeInImport(Import import, string type)
        {
            if (import.Type != null && _resolver.IdentifiersEqual(import.Type.Name, type))
                return import.Type;

            foreach (var childImport in import.Imports)
            {
                var result = FindTypeInImport(childImport, type);

                if (result != null)
                    return result;
            }

            return null;
        }

        private Type ResolveExpressionCommonType(Type left, Type right, bool allowStringConcat, bool allowBothImplicit, bool allowObjectCoercion)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");

            // No casting required.

            if (left == right)
                return left;

            if (allowObjectCoercion)
            {
                if (left == typeof(object))
                    return right;
                if (right == typeof(object))
                    return left;
            }

            // Special cast for adding strings.

            if (allowStringConcat && (left == typeof(string) || right == typeof(string)))
                return typeof(string);

            // Can we cast implicitly?

            var leftTable = TypeUtil.GetImplicitCastingTable(left);
            var rightTable = TypeUtil.GetImplicitCastingTable(right);

            if (leftTable != null && rightTable != null)
            {
                // See whether one of the types appears in the other.

                foreach (var leftType in leftTable)
                {
                    if (leftType == right)
                        return leftType;
                }

                foreach (var rightType in rightTable)
                {
                    if (rightType == left)
                        return rightType;
                }

                // Otherwise, find the first common type.

                int lowest = int.MaxValue;

                for (int i = 0; i < leftTable.Count; i++)
                {
                    for (int j = 0; j < rightTable.Count; j++)
                    {
                        if (leftTable[i] == rightTable[j])
                            lowest = Math.Min(lowest, j);
                    }
                }

                if (lowest != int.MaxValue)
                {
                    if (
                        allowBothImplicit ||
                        (IsAllowableImplicit(left, rightTable[lowest]) && IsAllowableImplicit(right, rightTable[lowest]))
                    )
                        return rightTable[lowest];
                }
            }

            // Look for a common base-class which might support this operator
            var commonType = FindCommonType(left, right);
            if (commonType != null)
                return commonType;

            // Look for an operator in one of the types which is compatible with the left/right parameters


            throw new ExpressionsException("Cannot resolve expression type", ExpressionsExceptionType.TypeMismatch);
        }

        private static Type FindCommonType(Type left, Type right, bool includeObject = false)
        {
            var leftTypes = GetTypeHierarchy(left);
            var rightTypes = GetTypeHierarchy(right);

            foreach (var l in leftTypes)
            {
                foreach (var r in rightTypes)
                {
                    if (r == l)
                        return l;
                }
            }

            return null;
        }

        private static List<Type> GetTypeHierarchy(Type t)
        {
            Require.NotNull(t, "t");

            // Recursively list inherited classes, excluding System.Object, since will not help with most operators
            var types = new List<Type>() { t };
            var current = t.BaseType;
            if (current != null && current != typeof(object) && current != typeof(ValueType) && current != typeof(Enum))
                types.AddRange(GetTypeHierarchy(current));
            return types;
        }

        private bool IsAllowableImplicit(Type a, Type b)
        {
            return TypeUtil.IsInteger(a) == TypeUtil.IsInteger(b);
        }

        private Type ResolveExpressionType(Type left, Type right, Type commonType, ExpressionType type)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");

            // TODO: Implicit/explicit operators and operators for the expression type.

            string operatorName = GetOperatorName(left, right, type);

            // Boolean operators.

            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.Xor:
                case ExpressionType.AndBoth:
                case ExpressionType.OrBoth:
                    if (TypeUtil.IsInteger(commonType))
                        return commonType;
                    // Look whether operator is overloaded in commontype
                    else if (!string.IsNullOrEmpty(operatorName) && IsOperatorOverloaded(commonType, type, operatorName))
                        return commonType;
                    else if (left != typeof(bool) || right != typeof(bool))
                        throw new ExpressionsException("Invalid operand for expression type", ExpressionsExceptionType.TypeMismatch);

                    return typeof(bool);

                case ExpressionType.Greater:
                case ExpressionType.GreaterOrEquals:
                case ExpressionType.Less:
                case ExpressionType.LessOrEquals:
                    if (
                        !(left.IsEnum || TypeUtil.IsNumeric(left)) ||
                        !(right.IsEnum || TypeUtil.IsNumeric(right))
                    )
                        throw new ExpressionsException("Invalid operand for expression type", ExpressionsExceptionType.TypeMismatch);

                    return typeof(bool);

                case ExpressionType.Equals:
                case ExpressionType.NotEquals:
                case ExpressionType.In:
                case ExpressionType.LogicalAnd:
                case ExpressionType.LogicalOr:
                case ExpressionType.Compares:
                case ExpressionType.NotCompares:
                    return typeof(bool);

                case ExpressionType.Add:
                    if (
                        !(left == typeof(string) || right == typeof(string)) &&
                        !(TypeUtil.IsNumeric(left) && TypeUtil.IsNumeric(right))
                    )
                        throw new ExpressionsException("Invalid operand for expression type", ExpressionsExceptionType.TypeMismatch);

                    return commonType;

                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                case ExpressionType.Power:
                    // Look whether operator is overloaded in commontype
                    if (!string.IsNullOrEmpty(operatorName) && IsOperatorOverloaded(commonType, type, operatorName))
                        return commonType;
                    else if (!TypeUtil.IsNumeric(left) || !TypeUtil.IsNumeric(right))
                        throw new ExpressionsException("Invalid operand for expression type", ExpressionsExceptionType.TypeMismatch);

                    return commonType;

                default:
                    return commonType;
            }
        }

        private static bool IsOperatorOverloaded(Type t, ExpressionType type, string operatorName)
        {
            Require.NotNull(t, "t");
            Require.NotNull(type, "type");
            Require.NotEmpty(operatorName, "operatorName");

            foreach (var baseType in GetTypeHierarchy(t))
            {
                if (baseType.GetMethod(operatorName) != null)
                    return true;
            }

            // We didn't find the operator in any of the base-types.
            return false;

        }

        public IExpression Conditional(Ast.Conditional conditional)
        {
            var condition = conditional.Condition.Accept(this);
            var then = conditional.Then.Accept(this);
            var @else = conditional.Else.Accept(this);

            if (condition.Type != typeof(bool))
                throw new ExpressionsException("Condition of conditional must evaluate to a boolean", ExpressionsExceptionType.TypeMismatch);

            var commonType = ResolveExpressionCommonType(then.Type, @else.Type, false, false, true);

            return new Expressions.Conditional(condition, then, @else, commonType);
        }
    }
}
