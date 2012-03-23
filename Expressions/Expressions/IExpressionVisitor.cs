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
}
