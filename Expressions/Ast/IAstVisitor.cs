using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal interface IAstVisitor<T>
    {
        T BinaryExpression(BinaryExpression binaryExpression);

        T Cast(Cast cast);

        T Constant(Constant constant);

        T IdentifierAccess(IdentifierAccess identifierAccess);

        T Index(Index index);

        T MemberAccess(MemberAccess memberAccess);

        T MethodCall(MethodCall methodCall);

        T UnaryExpression(UnaryExpression unaryExpression);
    }
}
