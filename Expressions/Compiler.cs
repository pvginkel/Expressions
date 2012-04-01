using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
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
                (expression.Type.IsValueType || _resolver.Options.ResultType.IsValueType) &&
                expression.Type != _resolver.Options.ResultType
            ) {
                try
                {
                    ExtendedConvertToType(expression.Type, _resolver.Options.ResultType, true);
                }
                catch (Exception ex)
                {
                    throw new ExpressionsException("Cannot convert expression result to expected result type", ExpressionsExceptionType.InvalidExplicitCast, ex);
                }

                _il.Emit(OpCodes.Box, _resolver.Options.ResultType);
            }
            else if (expression.Type.IsValueType)
            {
                _il.Emit(OpCodes.Box, expression.Type);
            }
            else if (!_resolver.Options.ResultType.IsAssignableFrom(expression.Type))
            {
                throw new ExpressionsException("Cannot convert expression result to expected result type", ExpressionsExceptionType.TypeMismatch);
            }

            _il.Emit(OpCodes.Ret);
        }

        private void ExtendedConvertToType(Type fromType, Type toType, bool allowExplicit)
        {
            try
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

                var castMethod = _resolver.FindOperatorMethod(
                    "op_Implicit",
                    new[] { fromType, toType },
                    toType,
                    new[] { fromType }
                );

                if (castMethod == null && allowExplicit)
                {
                    castMethod = _resolver.FindOperatorMethod(
                        "op_Explicit",
                        new[] { fromType, toType },
                        toType,
                        new[] { fromType }
                    );
                }

                if (castMethod != null)
                {
                    var parameterType = castMethod.GetParameters()[0].ParameterType;

                    if (fromType != parameterType)
                        ILUtil.EmitConvertToType(_il, fromType, parameterType, _resolver.Options.Checked);

                    _il.Emit(OpCodes.Call, castMethod);

                    if (toType != castMethod.ReturnType)
                        ILUtil.EmitConvertToType(_il, castMethod.ReturnType, toType, _resolver.Options.Checked);

                    return;
                }

                // Let ILUtill handle it.

                ILUtil.EmitConvertToType(_il, fromType, toType, _resolver.Options.Checked);
            }
            catch (Exception ex)
            {
                throw new ExpressionsException(
                    "Invalid explicit cast",
                    ExpressionsExceptionType.InvalidExplicitCast,
                    ex
                );
            }
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
                    case ExpressionType.Modulo:
                        BinaryArithicExpression(binaryExpression);
                        break;

                    case ExpressionType.ShiftLeft:
                    case ExpressionType.ShiftRight:
                        BinaryShiftExpression(binaryExpression);
                        break;

                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.Xor:
                        if (TypeUtil.IsInteger(binaryExpression.CommonType))
                            BinaryBitwiseExpression(binaryExpression);
                        else
                            BinaryLogicalExpression(binaryExpression);
                        break;

                    case ExpressionType.LogicalAnd:
                    case ExpressionType.AndBoth:
                    case ExpressionType.LogicalOr:
                    case ExpressionType.OrBoth:
                        BinaryLogicalExpression(binaryExpression);
                        break;

                    case ExpressionType.BitwiseAnd:
                    case ExpressionType.BitwiseOr:
                        BinaryBitwiseExpression(binaryExpression);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            private void BinaryArithicExpression(BinaryExpression binaryExpression)
            {
                // Reference compares.

                if (
                    !binaryExpression.Left.Type.IsValueType &&
                    !binaryExpression.Right.Type.IsValueType && (
                        binaryExpression.ExpressionType == ExpressionType.Equals ||
                        binaryExpression.ExpressionType == ExpressionType.NotEquals
                    )
                ) {
                    binaryExpression.Left.Accept(this);
                    binaryExpression.Right.Accept(this);

                    _il.Emit(OpCodes.Ceq);

                    if (binaryExpression.ExpressionType == ExpressionType.NotEquals)
                    {
                        _il.Emit(OpCodes.Ldc_I4_0);
                        _il.Emit(OpCodes.Ceq);
                    }

                    return;
                }

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
                        if (_compiler._resolver.Options.Checked && TypeUtil.IsInteger(binaryExpression.CommonType))
                        {
                            if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                                _il.Emit(OpCodes.Add_Ovf_Un);
                            else
                                _il.Emit(OpCodes.Add_Ovf);
                        }
                        else
                        {
                            _il.Emit(OpCodes.Add);
                        }
                        break;

                    case ExpressionType.Divide:
                        if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                            _il.Emit(OpCodes.Div_Un);
                        else
                            _il.Emit(OpCodes.Div);
                        break;

                    case ExpressionType.Multiply:
                        if (_compiler._resolver.Options.Checked && TypeUtil.IsInteger(binaryExpression.CommonType))
                        {
                            if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                                _il.Emit(OpCodes.Mul_Ovf_Un);
                            else
                                _il.Emit(OpCodes.Mul_Ovf);
                        }
                        else
                        {
                            _il.Emit(OpCodes.Mul);
                        }
                        break;

                    case ExpressionType.Subtract:
                        if (_compiler._resolver.Options.Checked && TypeUtil.IsInteger(binaryExpression.CommonType))
                        {
                            if (TypeUtil.IsUnsigned(binaryExpression.CommonType))
                                _il.Emit(OpCodes.Sub_Ovf_Un);
                            else
                                _il.Emit(OpCodes.Sub_Ovf);
                        }
                        else
                        {
                            _il.Emit(OpCodes.Sub);
                        }
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

            private void BinaryShiftExpression(BinaryExpression binaryExpression)
            {
                binaryExpression.Left.Accept(this);

                binaryExpression.Right.Accept(this);

                int bits = Marshal.SizeOf(binaryExpression.Left.Type) * 8;

                ILUtil.EmitConstant(_il, bits - 1);
                _il.Emit(OpCodes.And);

                switch (binaryExpression.ExpressionType)
                {
                    case ExpressionType.ShiftLeft:
                        _il.Emit(OpCodes.Shl);
                        break;

                    case ExpressionType.ShiftRight:
                        if (TypeUtil.IsUnsigned(binaryExpression.Left.Type))
                            _il.Emit(OpCodes.Shr_Un);
                        else
                            _il.Emit(OpCodes.Shr);
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
                    case ExpressionType.BitwiseAnd:
                        _il.Emit(OpCodes.And);
                        break;

                    case ExpressionType.Or:
                    case ExpressionType.BitwiseOr:
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
                    case ExpressionType.LogicalAnd:
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

                    case ExpressionType.AndBoth:
                        binaryExpression.Left.Accept(this);
                        binaryExpression.Right.Accept(this);

                        _il.Emit(OpCodes.And);
                        break;

                    case ExpressionType.Or:
                    case ExpressionType.LogicalOr:
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

                    case ExpressionType.OrBoth:
                        binaryExpression.Left.Accept(this);
                        binaryExpression.Right.Accept(this);

                        _il.Emit(OpCodes.Or);
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
                    _compiler.ExtendedConvertToType(expression.Type, type, false);
            }

            public void Cast(Cast cast)
            {
                cast.Operand.Accept(this);

                _compiler.ExtendedConvertToType(cast.Operand.Type, cast.Type, true);
            }

            public void Constant(Constant constant)
            {
                if (constant.Value == null)
                {
                    ILUtil.EmitNull(_il);
                }
                else if (constant.Value is DateTime)
                {
                    ILUtil.EmitConstant(_il, ((DateTime)constant.Value).Ticks);
                    ILUtil.EmitNew(_il, typeof(DateTime).GetConstructor(new[] { typeof(long) }));
                }
                else if (constant.Value is TimeSpan)
                {
                    ILUtil.EmitConstant(_il, ((TimeSpan)constant.Value).Ticks);
                    ILUtil.EmitNew(_il, typeof(TimeSpan).GetConstructor(new[] { typeof(long) }));
                }
                else
                {
                    ILUtil.EmitConstant(_il, constant.Value);
                }
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
                    bool emitBox = false;
                    bool emitStoreLoad = false;

                    if (
                        (methodCall.Operand is FieldAccess || methodCall.Operand is Constant) &&
                        methodCall.Operand.Type.IsValueType &&
                        !methodCall.MethodInfo.DeclaringType.IsValueType
                    )
                        emitBox = true;
                    else if (
                        methodCall.Operand.Type.IsValueType && (
                            !(methodCall.Operand is FieldAccess) ||
                            methodCall.MethodInfo.DeclaringType.IsValueType
                        )
                    )
                        emitStoreLoad = true;

                    // Signal field access that we're using the field as a
                    // parameter.

                    _fieldAsParameter =
                        methodCall.Operand is FieldAccess &&
                        methodCall.MethodInfo.DeclaringType.IsValueType &&
                        !emitStoreLoad;

                    try
                    {
                        methodCall.Operand.Accept(this);
                    }
                    finally
                    {
                        _fieldAsParameter = false;
                    }

                    if (emitBox)
                    {
                        _il.Emit(OpCodes.Box, methodCall.Operand.Type);
                    }
                    else if (emitStoreLoad)
                    {
                        Debug.Assert(methodCall.Operand.Type == methodCall.MethodInfo.DeclaringType);

                        var builder = _il.DeclareLocal(methodCall.Operand.Type);

                        _il.Emit(OpCodes.Stloc, builder);
                        _il.Emit(OpCodes.Ldloca, builder);
                    }
                }

                var parameters = methodCall.MethodInfo.GetParameters();
                var arguments = methodCall.Arguments;

                bool paramsMethod =
                    parameters.Length > 0 &&
                    parameters[parameters.Length - 1].GetCustomAttributes(typeof(ParamArrayAttribute), true).Length == 1;

                int mandatoryParameterCount =
                    paramsMethod
                    ? parameters.Length - 1
                    : parameters.Length;

                for (int i = 0; i < mandatoryParameterCount; i++)
                {
                    Emit(arguments[i], parameters[i].ParameterType);
                }

                if (paramsMethod)
                {
                    var paramsType = parameters[parameters.Length - 1].ParameterType;
                    var elementType = paramsType.GetElementType();
                    bool emitted = false;

                    // When the params argument is missing, a new array is issued.

                    if (arguments.Count == mandatoryParameterCount)
                    {
                        ILUtil.EmitEmptyArray(_il, elementType);
                        emitted = true;
                    }
                    else if (arguments.Count == mandatoryParameterCount + 1)
                    {
                        var lastArgument = arguments[arguments.Count - 1];
                        var constant = lastArgument as Constant;

                        // Null arguments are passed blindly.

                        if (constant != null && constant.Value == null)
                        {
                            ILUtil.EmitNull(_il);
                            emitted = true;
                        }
                        else
                        {
                            // So are array arguments that can be casted.

                            if (
                                lastArgument.Type.IsArray &&
                                TypeUtil.CanCastImplicitely(lastArgument.Type, paramsType, false)
                            )
                            {
                                Emit(lastArgument, paramsType);
                                emitted = true;
                            }
                        }
                    }

                    // If we didn't find a shortcut, emit the array with all
                    // arguments.

                    if (!emitted)
                    {
                        ILUtil.EmitArray(
                            _il,
                            elementType,
                            arguments.Count - mandatoryParameterCount,
                            p => Emit(arguments[mandatoryParameterCount + p], elementType)
                        );
                    }
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
                        unaryExpression.Operand.Accept(this);

                        if (TypeUtil.IsInteger(unaryExpression.Operand.Type))
                            _il.Emit(OpCodes.Not);
                        else
                        {
                            ILUtil.EmitConstant(_il, 0);
                            _il.Emit(OpCodes.Ceq);
                        }
                        break;

                    case ExpressionType.LogicalNot:
                        unaryExpression.Operand.Accept(this);

                        ILUtil.EmitConstant(_il, 0);
                        _il.Emit(OpCodes.Ceq);
                        break;

                    case ExpressionType.BitwiseNot:
                        unaryExpression.Operand.Accept(this);

                        _il.Emit(OpCodes.Not);
                        break;

                    case ExpressionType.Plus:
                    case ExpressionType.Group:
                        // No-ops.

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

                _compiler.ExtendedConvertToType(typeof(object), variableAccess.Type, false);
            }

            public void TypeAccess(TypeAccess typeAccess)
            {
                throw new ExpressionsException("Syntax error", ExpressionsExceptionType.TypeMismatch);
            }

            public void Conditional(Conditional conditional)
            {
                conditional.Condition.Accept(this);

                var elseBranch = _il.DefineLabel();
                var afterBranch = _il.DefineLabel();

                _il.Emit(OpCodes.Brfalse, elseBranch);

                conditional.Then.Accept(this);

                if (conditional.Then.Type != conditional.Type)
                    _compiler.ExtendedConvertToType(conditional.Then.Type, conditional.Type, false);

                _il.Emit(OpCodes.Br, afterBranch);

                _il.MarkLabel(elseBranch);

                conditional.Else.Accept(this);

                if (conditional.Else.Type != conditional.Type)
                    _compiler.ExtendedConvertToType(conditional.Else.Type, conditional.Type, false);

                _il.MarkLabel(afterBranch);
            }
        }
    }
}
