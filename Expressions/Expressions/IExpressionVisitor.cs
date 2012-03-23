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
    }
}
