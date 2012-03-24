using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;

namespace Expressions
{
    internal class ConversionVisitor : ExpressionVisitor
    {
        private readonly Resolver _resolver;

        public ConversionVisitor(Resolver resolver)
        {
            Require.NotNull(resolver, "resolver");

            _resolver = resolver;
        }

        public override IExpression BinaryExpression(BinaryExpression binaryExpression)
        {
            var left = binaryExpression.Left.Accept(this);
            var right = binaryExpression.Right.Accept(this);

            if (binaryExpression.ExpressionType == ExpressionType.Add)
            {
                if (left.Type == typeof(string) || right.Type == typeof(string))
                {
                    return _resolver.ResolveMethod(
                        new TypeAccess(typeof(string)),
                        "Concat",
                        new[] { left, right }
                    );
                }
                else
                {
                    if (left == binaryExpression.Left && right == binaryExpression.Right)
                        return binaryExpression;
                    else
                        return new BinaryExpression(left, right, binaryExpression.ExpressionType, binaryExpression.Type, binaryExpression.CommonType);
                }
            }
            else if (binaryExpression.ExpressionType == ExpressionType.Power)
            {
                return _resolver.ResolveMethod(
                    new TypeAccess(typeof(Math)),
                    "Pow",
                    new[] { left, right }
                );
            }

            if (left == binaryExpression.Left && right == binaryExpression.Right)
                return binaryExpression;
            else
                return new BinaryExpression(left, right, binaryExpression.ExpressionType, binaryExpression.Type);
        }

        public override IExpression UnaryExpression(UnaryExpression unaryExpression)
        {
            var operand = unaryExpression.Operand.Accept(this);

            // Coerce -(uint) to -(ulong)(uint)

            if (
                unaryExpression.ExpressionType == ExpressionType.Minus &&
                operand.Type == typeof(uint)
            )
                return new UnaryExpression(new Cast(operand, typeof(long)), typeof(long), ExpressionType.Minus);

            if (operand == unaryExpression.Operand)
                return unaryExpression;
            else
                return new UnaryExpression(operand, unaryExpression.Type, unaryExpression.ExpressionType);
        }
    }
}
