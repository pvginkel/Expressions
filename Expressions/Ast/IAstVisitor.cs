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

        T Conditional(Conditional conditional);
    }

    internal class AstVisitor : IAstVisitor<IAstNode>
    {
        public virtual IAstNode BinaryExpression(BinaryExpression binaryExpression)
        {
            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            if (left == binaryExpression.Left && right == binaryExpression.Right)
                return binaryExpression;
            else
                return new BinaryExpression(left, right, binaryExpression.Type);
        }

        public virtual IAstNode Cast(Cast cast)
        {
            var operand = cast.Operand.Accept(this);

            if (operand == cast.Operand)
                return cast;
            else
                return new Cast(operand, cast.Type);
        }

        public virtual IAstNode Constant(Constant constant)
        {
            return constant;
        }

        public virtual IAstNode IdentifierAccess(IdentifierAccess identifierAccess)
        {
            return identifierAccess;
        }

        public virtual IAstNode Index(Index index)
        {
            var operand = index.Operand.Accept(this);
            var arguments = Arguments(index.Arguments);

            if (operand == index.Operand && arguments == index.Arguments)
                return index;
            else
                return new Index(operand, arguments);
        }

        private AstNodeCollection Arguments(AstNodeCollection arguments)
        {
            if (arguments == null)
                return null;

            var newArguments = new IAstNode[arguments.Nodes.Count];
            bool equal = true;

            for (int i = 0; i < newArguments.Length; i++)
            {
                newArguments[i] = arguments.Nodes[i].Accept(this);

                equal = equal && newArguments[i] == arguments.Nodes[i];
            }

            if (equal)
                return arguments;
            else
                return new AstNodeCollection(newArguments);
        }

        public virtual IAstNode MemberAccess(MemberAccess memberAccess)
        {
            var operand = memberAccess.Operand.Accept(this);

            if (operand == memberAccess.Operand)
                return memberAccess;
            else
                return new MemberAccess(operand, memberAccess.Member);
        }

        public virtual IAstNode MethodCall(MethodCall methodCall)
        {
            var operand = methodCall.Operand.Accept(this);
            var arguments = Arguments(methodCall.Arguments);

            if (operand == methodCall.Operand && arguments == methodCall.Arguments)
                return methodCall;
            else
                return new MethodCall(operand, arguments);
        }

        public virtual IAstNode UnaryExpression(UnaryExpression unaryExpression)
        {
            var operand = unaryExpression.Operand.Accept(this);

            if (operand == unaryExpression.Operand)
                return unaryExpression;
            else
                return new UnaryExpression(operand, unaryExpression.Type);
        }

        public virtual IAstNode Conditional(Conditional conditional)
        {
            var condition = conditional.Condition.Accept(this);
            var then = conditional.Then.Accept(this);
            var @else = conditional.Else.Accept(this);

            if (condition == conditional.Condition && then == conditional.Then && @else == conditional.Else)
                return conditional;
            else
                return new Conditional(condition, then, @else);
        }
    }
}
