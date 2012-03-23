using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal interface IExpressionVisitor
    {
        void BinaryExpression(BinaryExpression binaryExpression);

        void Cast(Cast cast);

        void Constant(Constant constant);

        void FieldAccess(FieldAccess fieldAccess);

        void Index(Index index);

        void MethodCall(MethodCall methodCall);

        void UnaryExpression(UnaryExpression unaryExpression);

        void VariableAccess(VariableAccess variableAccess);

        void TypeAccess(TypeAccess typeAccess);
    }

    internal interface IExpressionVisitor<T>
    {
        T BinaryExpression(BinaryExpression binaryExpression);

        T Cast(Cast cast);

        T Constant(Constant constant);

        T FieldAccess(FieldAccess fieldAccess);

        T Index(Index index);

        T MethodCall(MethodCall methodCall);

        T UnaryExpression(UnaryExpression unaryExpression);

        T VariableAccess(VariableAccess variableAccess);

        T TypeAccess(TypeAccess typeAccess);
    }

    internal class ExpressionVisitor : IExpressionVisitor<IExpression>
    {
        public virtual IExpression BinaryExpression(BinaryExpression binaryExpression)
        {
            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            if (left == binaryExpression.Left && right == binaryExpression.Right)
                return binaryExpression;
            else
                return new BinaryExpression(left, right, binaryExpression.ExpressionType, binaryExpression.Type);
        }

        public virtual IExpression Cast(Cast cast)
        {
            var operand = cast.Operand.Accept(this);

            if (operand == cast.Operand)
                return cast;
            else
                return new Cast(operand, cast.Type);
        }

        public virtual IExpression Constant(Constant constant)
        {
            return constant;
        }

        public virtual IExpression FieldAccess(FieldAccess fieldAccess)
        {
            var operand = fieldAccess.Operand.Accept(this);

            if (operand == fieldAccess.Operand)
                return fieldAccess;
            else
                return new FieldAccess(operand, fieldAccess.FieldInfo);
        }

        public virtual IExpression Index(Index index)
        {
            var operand = index.Operand.Accept(this);
            var argument = index.Argument.Accept(this);

            if (operand == index.Operand && argument == index.Argument)
                return index;
            else
                return new Index(operand, argument, index.Type);
        }

        public virtual IExpression MethodCall(MethodCall methodCall)
        {
            var operand = methodCall.Operand.Accept(this);

            var arguments = new IExpression[methodCall.Arguments.Count];
            bool equal = operand == methodCall.Operand;

            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = methodCall.Arguments[i].Accept(this);

                if (equal)
                    equal = arguments[i] == methodCall.Arguments[i];
            }

            if (equal)
                return methodCall;
            else
                return new MethodCall(operand, methodCall.MethodInfo, arguments);
        }

        public virtual IExpression UnaryExpression(UnaryExpression unaryExpression)
        {
            var operand = unaryExpression.Operand.Accept(this);

            if (operand == unaryExpression.Operand)
                return unaryExpression;
            else
                return new UnaryExpression(operand, unaryExpression.Type, unaryExpression.ExpressionType);
        }

        public virtual IExpression VariableAccess(VariableAccess variableAccess)
        {
            return variableAccess;
        }

        public IExpression TypeAccess(TypeAccess typeAccess)
        {
            return typeAccess;
        }
    }
}
