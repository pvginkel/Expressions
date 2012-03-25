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

        public IExpression BinaryExpression(Ast.BinaryExpression binaryExpression)
        {
            // In expressions are converted to method calls.

            if (binaryExpression.Type == ExpressionType.In)
                return BinaryInExpression(binaryExpression);

            // We need to do this here and not in the conversion phase because
            // we need the return type of the method to fully bind the tree.

            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            string operatorName = null;

            switch (binaryExpression.Type)
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
                case ExpressionType.LogicalOr: operatorName = "op_LogicalOr"; break;
                case ExpressionType.BitwiseAnd: operatorName = "op_BitwiseAnd"; break;
                case ExpressionType.BitwiseOr: operatorName = "op_BitwiseOr"; break;

                case ExpressionType.And:
                    if (left.Type == typeof(bool) && right.Type == typeof(bool))
                        operatorName = "op_LogicalAnd";
                    else
                        operatorName = "op_BitwiseAnd";
                    break;

                case ExpressionType.Or:
                    if (left.Type == typeof(bool) && right.Type == typeof(bool))
                        operatorName = "op_LogicalOr";
                    else
                        operatorName = "op_BitwiseOr";
                    break;
                    
                case ExpressionType.Xor:
                    operatorName = "op_ExclusiveOr";
                    break;
            }

            if (operatorName != null)
            {
                var method = _resolver.FindOperatorMethod(
                    operatorName,
                    new[] { left.Type, right.Type },
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

            Type commonType;

            switch (binaryExpression.Type)
            {
                case ExpressionType.ShiftLeft:
                case ExpressionType.ShiftRight:
                    commonType = left.Type;
                    break;

                case ExpressionType.Power:
                    commonType = typeof(double);
                    break;

                default:
                    commonType = ResolveExpressionCommonType(left.Type, right.Type, binaryExpression.Type == ExpressionType.Add);
                    break;
            }
            
            var type = ResolveExpressionType(left.Type, right.Type, commonType, binaryExpression.Type);

            return new Expressions.BinaryExpression(left, right, binaryExpression.Type, type, commonType);
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
                    throw new NotSupportedException("Right operand of set must be an enumerable");
                }
            }
        }

        public IExpression Cast(Ast.Cast cast)
        {
            return new Expressions.Cast(
                cast.Operand.Accept(this),
                ResolveType(cast.Type.Name, cast.Type.ArrayIndex)
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
                    return new VariableAccess(_resolver.IdentifierTypes[i], _resolver.IdentifierIndexes[i]);
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

            string identifierName = identifierAccess.Name;

            if (_resolver.IgnoreCase)
                identifierName = identifierName.ToLowerInvariant();

            var type = TypeUtil.GetBuiltInType(identifierName, _resolver.DynamicExpression.Language);

            if (type != null)
                return new TypeAccess(type);

            throw new NotSupportedException("Could not resolve identifier");
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
            var arguments = ResolveArguments(index.Arguments);

            var operand = index.Operand.Accept(this);

            if (operand.Type.IsArray)
            {
                if (arguments.Length != operand.Type.GetArrayRank())
                    throw new NotSupportedException("Rank of array is incorrect");

                foreach (var argument in arguments)
                {
                    if (!TypeUtil.IsCastAllowed(argument.Type, typeof(int)))
                        throw new NotSupportedException("Arguments of array index must be convertible to int");
                }

                if (arguments.Length == 1)
                    return new Expressions.Index(operand, arguments[0], operand.Type.GetElementType());
                else
                    return _resolver.ResolveMethod(operand, "Get", arguments);
            }

            var defaultMemberAttributes = operand.Type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);

            if (defaultMemberAttributes.Length != 1)
                throw new NotSupportedException("Operand does not support indexing");

            var result = _resolver.ResolveMethod(operand, "get_" + ((DefaultMemberAttribute)defaultMemberAttributes[0]).MemberName, arguments);

            if (result == null)
                throw new NotSupportedException("Cannot resolve index method");

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
                    ) {
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

                throw new NotSupportedException("Could not resolve member");
            }

            var result = Resolve(operand, memberAccess.Member);

            if (result == null)
                throw new NotSupportedException("Could not resolve member");

            return result;
        }

        public IExpression MethodCall(Ast.MethodCall methodCall)
        {
            var arguments = ResolveArguments(methodCall.Arguments);

            var identifierAccess = methodCall.Operand as IdentifierAccess;

            if (identifierAccess != null)
                return ResolveGlobalMethod(identifierAccess.Name, arguments);

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

            throw new NotSupportedException("Cannot resolve method call on " + methodCall.Operand.GetType().Name);
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

            throw new NotSupportedException("Could not resolve method");
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
                        throw new NotSupportedException("Cannot plus non numeric type");

                    type = operand.Type;
                    break;

                case ExpressionType.Group:
                    type = operand.Type;
                    break;

                case ExpressionType.Minus:
                    if (!TypeUtil.IsValidUnaryArgument(operand.Type))
                        throw new NotSupportedException("Cannot plus non numeric type");

                    // TODO: Make constants signed and handle minus on unsigned's.

                    type = operand.Type;
                    break;

                case ExpressionType.LogicalNot:
                    if (operand.Type != typeof(bool))
                        throw new NotSupportedException("Cannot not non boolean types");

                    type = typeof(bool);
                    break;

                case ExpressionType.BitwiseNot:
                    if (!TypeUtil.IsInteger(operand.Type))
                        throw new NotSupportedException("Cannot not bitwise not boolean types");

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
                            throw new NotSupportedException("Cannot not non boolean types");

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
            string builtInType = type;

            if (_resolver.IgnoreCase)
                builtInType = builtInType.ToLowerInvariant();

            var result = TypeUtil.GetBuiltInType(builtInType, _resolver.DynamicExpression.Language);

            if (result == null)
                result = Type.GetType(type, false, _resolver.IgnoreCase);

            // This deviates from the specs but allows us to not include
            // boolean and single in the known types table.

            if (result == null)
                result = Type.GetType("System." + type, false, _resolver.IgnoreCase);

            if (result == null)
                result = FindTypeInImports(type);

            if (result == null)
                throw new NotSupportedException(String.Format("Unknown type '{0}'", type));

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

        private Type ResolveExpressionCommonType(Type left, Type right, bool allowStringConcat)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");

            // No casting required.

            if (left == right)
                return left;

            if (left == typeof(object))
                return right;
            if (right == typeof(object))
                return left;

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
                    return rightTable[lowest];
            }

            // We can't cast implicitly.

            throw new NotSupportedException(String.Format("Cannot implicitly cast {0} and {1}", left, right));
        }

        private Type ResolveExpressionType(Type left, Type right, Type commonType, ExpressionType type)
        {
            Require.NotNull(left, "left");
            Require.NotNull(right, "right");

            // TODO: Implicit/explicit operators and operators for the expression type.

            // Boolean operators.

            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.Xor:
                    if (TypeUtil.IsInteger(commonType))
                        return commonType;

                    else if (left != typeof(bool) || right != typeof(bool))
                        throw new NotSupportedException("Operands of logical operation must be logical");

                    return typeof(bool);

                case ExpressionType.Equals:
                case ExpressionType.NotEquals:
                case ExpressionType.Greater:
                case ExpressionType.GreaterOrEquals:
                case ExpressionType.Less:
                case ExpressionType.LessOrEquals:
                case ExpressionType.In:
                case ExpressionType.LogicalAnd:
                case ExpressionType.LogicalOr:
                    return typeof(bool);

                default:
                    return commonType;
            }
        }

        public IExpression Conditional(Ast.Conditional conditional)
        {
            var condition = conditional.Condition.Accept(this);
            var then = conditional.Then.Accept(this);
            var @else = conditional.Else.Accept(this);

            var commonType = ResolveExpressionCommonType(then.Type, @else.Type, false);

            return new Expressions.Conditional(condition, then, @else, commonType);
        }
    }
}
