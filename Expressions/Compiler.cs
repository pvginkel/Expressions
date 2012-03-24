using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Expressions.Expressions;

namespace Expressions
{
    internal class Compiler
    {
        private readonly ILGenerator _il;

        public Compiler(ILGenerator il)
        {
            Require.NotNull(il, "il");

            _il = il;
        }

        public void Compile(IExpression expression)
        {
            expression.Accept(new Visitor(this));

            if (expression.Type.IsValueType)
                _il.Emit(OpCodes.Box, expression.Type);

            _il.Emit(OpCodes.Ret);
        }

        private class Visitor : IExpressionVisitor
        {
            private readonly Compiler _compiler;
            private readonly ILGenerator _il;

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

            private void BinaryLogicalExpression(BinaryExpression binaryExpression)
            {
                if (
                    binaryExpression.Left.Type != typeof(bool) ||
                    binaryExpression.Right.Type != typeof(bool)
                )
                    throw new NotSupportedException("Logical expressions must have both operands bool");

                Label beforeLabel;
                Label afterLabel;

                switch (binaryExpression.ExpressionType)
                {
                    case ExpressionType.And:
                        beforeLabel = _il.DefineLabel();
                        afterLabel = _il.DefineLabel();

                        binaryExpression.Left.Accept(this);
                        _il.Emit(OpCodes.Brfalse_S, beforeLabel);

                        binaryExpression.Right.Accept(this);
                        _il.Emit(OpCodes.Br_S, afterLabel);

                        _il.MarkLabel(beforeLabel);
                        ILUtil.EmitConstant(_il, 0);
                        _il.MarkLabel(afterLabel);
                        break;

                    case ExpressionType.Or:
                        beforeLabel = _il.DefineLabel();
                        afterLabel = _il.DefineLabel();

                        binaryExpression.Left.Accept(this);
                        _il.Emit(OpCodes.Brtrue_S, beforeLabel);

                        binaryExpression.Right.Accept(this);
                        _il.Emit(OpCodes.Br_S, afterLabel);

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
                    ILUtil.EmitConvertToType(_il, expression.Type, type, true);
            }

            public void Cast(Cast cast)
            {
                cast.Operand.Accept(this);

                ILUtil.EmitConvertToType(_il, cast.Operand.Type, cast.Type, true);
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
                if (!(fieldAccess.Operand is TypeAccess))
                {
                    fieldAccess.Operand.Accept(this);

                    _il.Emit(OpCodes.Ldfld, fieldAccess.FieldInfo);
                }
                else
                {
                    _il.Emit(OpCodes.Ldsfld, fieldAccess.FieldInfo);
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
                    methodCall.Operand.Accept(this);

                var parameters = methodCall.MethodInfo.GetParameters();
                var arguments = methodCall.Arguments;

                for (int i = 0; i < arguments.Count; i++)
                {
                    Emit(arguments[i], parameters[i].ParameterType);
                }

                _il.Emit(isStatic ? OpCodes.Call : OpCodes.Callvirt, methodCall.MethodInfo);
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
                        if (unaryExpression.Operand.Type != typeof(bool))
                            throw new NotSupportedException("Unary expression not can only be performed on bool");

                        unaryExpression.Operand.Accept(this);

                        ILUtil.EmitConstant(_il, 0);
                        _il.Emit(OpCodes.Ceq);
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

                ILUtil.EmitConvertToType(_il, typeof(object), variableAccess.Type, true);
            }

            public void TypeAccess(TypeAccess typeAccess)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
