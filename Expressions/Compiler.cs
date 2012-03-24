using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Expressions.Expressions;

namespace Expressions
{
    internal class Compiler
    {
        private readonly ILGenerator _il;
        private readonly Resolver _resolver;

        public Compiler(ILGenerator il, Resolver resolver)
        {
            Require.NotNull(il, "il");
            Require.NotNull(resolver, "resolver");

            _il = il;
            _resolver = resolver;
        }

        public void Compile(IExpression expression)
        {
            Require.NotNull(expression, "expression");

            expression.Accept(new Visitor(this));

            if (
                _resolver.Options.ResultType.IsValueType &&
                expression.Type != _resolver.Options.ResultType
            ) {
                ExtendedConvertToType(expression.Type, _resolver.Options.ResultType, true, true);

                _il.Emit(OpCodes.Box, _resolver.Options.ResultType);
            }
            else if (expression.Type.IsValueType)
            {
                _il.Emit(OpCodes.Box, expression.Type);
            }

            _il.Emit(OpCodes.Ret);
        }

        private void ExtendedConvertToType(Type fromType, Type toType, bool isChecked, bool allowExplicit)
        {
            // See whether we can find a constructor on target type.

            var constructor = _resolver.ResolveMethodGroup(
                toType.GetConstructors(_resolver.Options.AccessBindingFlags | BindingFlags.CreateInstance),
                new[] { fromType },
                new[] { false }
            );

            if (constructor != null)
            {
                ILUtil.EmitNew(_il, (ConstructorInfo)constructor);

                return;
            }

            // See whether we have an implicit cast.

            var castMethod =
                ResolveCastMethod(fromType, toType, fromType, "op_Implicit") ??
                ResolveCastMethod(fromType, toType, toType, "op_Implicit");

            if (castMethod == null && allowExplicit)
            {
                castMethod =
                    ResolveCastMethod(fromType, toType, fromType, "op_Explicit") ??
                    ResolveCastMethod(fromType, toType, toType, "op_Explicit");
            }

            if (castMethod != null)
            {
                var parameterType = castMethod.GetParameters()[0].ParameterType;

                if (fromType != parameterType)
                    ILUtil.EmitConvertToType(_il, fromType, parameterType, isChecked);

                _il.Emit(OpCodes.Call, castMethod);

                if (toType != castMethod.ReturnType)
                    ILUtil.EmitConvertToType(_il, castMethod.ReturnType, toType, isChecked);

                return;
            }

            // Let ILUtill handle it.

            ILUtil.EmitConvertToType(_il, fromType, toType, isChecked);
        }

        private MethodInfo ResolveCastMethod(Type fromType, Type toType, Type sourceType, string methodName)
        {
            var candidates = new List<MethodInfo>();

            foreach (var method in sourceType.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (method.Name != methodName)
                    continue;

                var parameters = method.GetParameters();

                if (method.ReturnType == toType && parameters[0].ParameterType == fromType)
                    return method;

                if (
                    TypeUtil.CanCastImplicitely(fromType, parameters[0].ParameterType, false) &&
                    TypeUtil.CanCastImplicitely(method.ReturnType, toType, false)
                )
                    candidates.Add(method);
            }

            return candidates.Count == 1 ? candidates[0] : null;
        }

        private class Visitor : IExpressionVisitor
        {
            private readonly Compiler _compiler;
            private readonly ILGenerator _il;
            private bool _fieldAsParameter;

            public Visitor(Compiler compiler)
            {
                _compiler = compiler;
                _il = _compiler._il;
            }

            public void BinaryExpression(BinaryExpression binaryExpression)
            {
                switch (binaryExpression.ExpressionType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.Divide:
                    case ExpressionType.Multiply:
                    case ExpressionType.Subtract:
                    case ExpressionType.Equals:
                    case ExpressionType.NotEquals:
                    case ExpressionType.Greater:
                    case ExpressionType.GreaterOrEquals:
                    case ExpressionType.Less:
                    case ExpressionType.LessOrEquals:
                    case ExpressionType.ShiftLeft:
                    case ExpressionType.ShiftRight:
                    case ExpressionType.Modulo:
                        BinaryArithicExpression(binaryExpression);
                        break;

                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.Xor:
                        if (TypeUtil.IsInteger(binaryExpression.CommonType))
                            BinaryBitwiseExpression(binaryExpression);
                        else
                            BinaryLogicalExpression(binaryExpression);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            private void BinaryArithicExpression(BinaryExpression binaryExpression)
            {
                Emit(binaryExpression.Left, binaryExpression.CommonType);
                Emit(binaryExpression.Right, binaryExpression.CommonType);

                switch (binaryExpression.ExpressionType)
                {
                    case ExpressionType.Equals:
                    case ExpressionType.NotEquals:
                        _il.Emit(OpCodes.Ceq);

                        if (binaryExpression.ExpressionType == ExpressionType.NotEquals)
                        {
                            _il.Emit(OpCodes.Ldc_I4_0);
                            _il.Emit(OpCodes.Ceq);
                        }
                        break;

                    case ExpressionType.Greater:
                    case ExpressionType.LessOrEquals:
                        if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                            _il.Emit(OpCodes.Cgt_Un);
                        else
                            _il.Emit(OpCodes.Cgt);

                        if (binaryExpression.ExpressionType == ExpressionType.LessOrEquals)
                        {
                            _il.Emit(OpCodes.Ldc_I4_0);
                            _il.Emit(OpCodes.Ceq);
                        }
                        break;

                    case ExpressionType.Less:
                    case ExpressionType.GreaterOrEquals:
                        if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                            _il.Emit(OpCodes.Clt_Un);
                        else
                            _il.Emit(OpCodes.Clt);

                        if (binaryExpression.ExpressionType == ExpressionType.GreaterOrEquals)
                        {
                            _il.Emit(OpCodes.Ldc_I4_0);
                            _il.Emit(OpCodes.Ceq);
                        }
                        break;

                    case ExpressionType.Add:
                        _il.Emit(OpCodes.Add);
                        break;

                    case ExpressionType.Divide:
                        if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                            _il.Emit(OpCodes.Div_Un);
                        else
                            _il.Emit(OpCodes.Div);
                        break;

                    case ExpressionType.Multiply:
                        _il.Emit(OpCodes.Mul);
                        break;

                    case ExpressionType.Subtract:
                        _il.Emit(OpCodes.Sub);
                        break;

                    case ExpressionType.ShiftLeft:
                        _il.Emit(OpCodes.Shl);
                        break;

                    case ExpressionType.ShiftRight:
                        _il.Emit(OpCodes.Shr);
                        break;

                    case ExpressionType.Modulo:
                        if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                            _il.Emit(OpCodes.Rem_Un);
                        else
                            _il.Emit(OpCodes.Rem);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            private void BinaryBitwiseExpression(BinaryExpression binaryExpression)
            {
                Emit(binaryExpression.Left, binaryExpression.CommonType);
                Emit(binaryExpression.Right, binaryExpression.CommonType);

                switch (binaryExpression.ExpressionType)
                {
                    case ExpressionType.And:
                        _il.Emit(OpCodes.And);
                        break;

                    case ExpressionType.Or:
                        _il.Emit(OpCodes.Or);
                        break;

                    case ExpressionType.Xor:
                        _il.Emit(OpCodes.Xor);
                        break;
                }
            }

            private void BinaryLogicalExpression(BinaryExpression binaryExpression)
            {
                Label beforeLabel;
                Label afterLabel;

                switch (binaryExpression.ExpressionType)
                {
                    case ExpressionType.And:
                        beforeLabel = _il.DefineLabel();
                        afterLabel = _il.DefineLabel();

                        binaryExpression.Left.Accept(this);
                        _il.Emit(OpCodes.Brfalse, beforeLabel);

                        binaryExpression.Right.Accept(this);
                        _il.Emit(OpCodes.Br, afterLabel);

                        _il.MarkLabel(beforeLabel);
                        ILUtil.EmitConstant(_il, 0);
                        _il.MarkLabel(afterLabel);
                        break;

                    case ExpressionType.Or:
                        beforeLabel = _il.DefineLabel();
                        afterLabel = _il.DefineLabel();

                        binaryExpression.Left.Accept(this);
                        _il.Emit(OpCodes.Brtrue, beforeLabel);

                        binaryExpression.Right.Accept(this);
                        _il.Emit(OpCodes.Br, afterLabel);

                        _il.MarkLabel(beforeLabel);
                        ILUtil.EmitConstant(_il, 1);
                        _il.MarkLabel(afterLabel);
                        break;

                    case ExpressionType.Xor:
                        binaryExpression.Left.Accept(this);
                        binaryExpression.Right.Accept(this);

                        _il.Emit(OpCodes.Xor);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            private void Emit(IExpression expression, Type type)
            {
                expression.Accept(this);

                if (expression.Type != type)
                    _compiler.ExtendedConvertToType(expression.Type, type, true, false);
            }

            public void Cast(Cast cast)
            {
                cast.Operand.Accept(this);

                _compiler.ExtendedConvertToType(cast.Operand.Type, cast.Type, true, true);
            }

            public void Constant(Constant constant)
            {
                if (constant.Value == null)
                    ILUtil.EmitNull(_il);
                else
                    ILUtil.EmitConstant(_il, constant.Value);
            }



            public void FieldAccess(FieldAccess fieldAccess)
            {
                bool fieldAsParameter = _fieldAsParameter;

                bool loadAddress =
                    fieldAccess.FieldInfo.FieldType.IsValueType &&
                    fieldAsParameter &&
                    !fieldAccess.FieldInfo.IsInitOnly;

                if (!(fieldAccess.Operand is TypeAccess))
                {
                    _fieldAsParameter = true;

                    try
                    {
                        fieldAccess.Operand.Accept(this);
                    }
                    finally
                    {
                        _fieldAsParameter = fieldAsParameter;
                    }

                    if (loadAddress)
                        _il.Emit(OpCodes.Ldflda, fieldAccess.FieldInfo);
                    else
                        _il.Emit(OpCodes.Ldfld, fieldAccess.FieldInfo);
                }
                else
                {
                    if (loadAddress)
                        _il.Emit(OpCodes.Ldsflda, fieldAccess.FieldInfo);
                    else
                        _il.Emit(OpCodes.Ldsfld, fieldAccess.FieldInfo);
                }

                if (
                    fieldAccess.FieldInfo.FieldType.IsValueType &&
                    fieldAsParameter &&
                    fieldAccess.FieldInfo.IsInitOnly
                ) {
                    var builder = _il.DeclareLocal(fieldAccess.FieldInfo.FieldType);

                    _il.Emit(OpCodes.Stloc, builder);
                    _il.Emit(OpCodes.Ldloca, builder);
                }
            }

            public void Index(Index index)
            {
                index.Operand.Accept(this);

                Emit(index.Argument, typeof(int));

                ILUtil.EmitLoadElement(_il, index.Type);
            }

            public void MethodCall(MethodCall methodCall)
            {
                bool isStatic = methodCall.Operand is TypeAccess;

                if (!isStatic)
                {
                    // Signal field access that we're using the field as a
                    // parameter.

                    _fieldAsParameter =
                        methodCall.Operand is FieldAccess &&
                        methodCall.MethodInfo.DeclaringType.IsValueType;

                    try
                    {
                        methodCall.Operand.Accept(this);
                    }
                    finally
                    {
                        _fieldAsParameter = false;
                    }

                    if (
                        methodCall.Operand.Type.IsValueType &&
                        !(methodCall.Operand is FieldAccess)
                    ) {
                        var builder = _il.DeclareLocal(methodCall.Operand.Type);

                        _il.Emit(OpCodes.Stloc, builder);
                        _il.Emit(OpCodes.Ldloca, builder);
                    }
                    else if (
                        methodCall.Operand is FieldAccess &&
                        methodCall.Operand.Type.IsValueType &&
                        !methodCall.MethodInfo.DeclaringType.IsValueType
                    ) {
                        _il.Emit(OpCodes.Box, methodCall.Operand.Type);
                    }
                }

                var parameters = methodCall.MethodInfo.GetParameters();
                var arguments = methodCall.Arguments;

                for (int i = 0; i < arguments.Count; i++)
                {
                    Emit(arguments[i], parameters[i].ParameterType);
                }

                _il.Emit(
                    isStatic || methodCall.MethodInfo.DeclaringType.IsValueType
                    ? OpCodes.Call
                    : OpCodes.Callvirt,
                    methodCall.MethodInfo
                );
            }

            public void UnaryExpression(UnaryExpression unaryExpression)
            {
                switch (unaryExpression.ExpressionType)
                {
                    case ExpressionType.Minus:
                        unaryExpression.Operand.Accept(this);

                        _il.Emit(OpCodes.Neg);
                        break;

                    case ExpressionType.Not:
                        if (TypeUtil.IsInteger(unaryExpression.Operand.Type))
                        {
                            unaryExpression.Operand.Accept(this);

                            _il.Emit(OpCodes.Not);
                        }
                        else
                        {
                            if (unaryExpression.Operand.Type != typeof(bool))
                                throw new NotSupportedException("Unary expression not can only be performed on bool");

                            unaryExpression.Operand.Accept(this);

                            ILUtil.EmitConstant(_il, 0);
                            _il.Emit(OpCodes.Ceq);
                        }
                        break;

                    case ExpressionType.Plus:
                        // Plus really is a no-op.

                        unaryExpression.Operand.Accept(this);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            public void VariableAccess(VariableAccess variableAccess)
            {
                _il.Emit(OpCodes.Ldarg_0);
                ILUtil.EmitConstant(_il, variableAccess.ParameterIndex);
                _il.Emit(OpCodes.Ldelem_Ref);

                _compiler.ExtendedConvertToType(typeof(object), variableAccess.Type, true, false);
            }

            public void TypeAccess(TypeAccess typeAccess)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
