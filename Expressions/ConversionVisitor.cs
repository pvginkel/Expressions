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

            if (
                binaryExpression.ExpressionType == ExpressionType.Add &&
                (left.Type == typeof(string) || right.Type == typeof(string))
            ) {
                return _resolver.ResolveMethod(
                    new TypeAccess(typeof(string)),
                    "Concat",
                    FlattenConcatArguments(binaryExpression.Left, binaryExpression.Right)
                );
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

        private IExpression[] FlattenConcatArguments(IExpression left, IExpression right)
        {
            var arguments = new List<IExpression>();

            FlattenConcatArguments(arguments, left);
            FlattenConcatArguments(arguments, right);

            return arguments.ToArray();
        }

        private void FlattenConcatArguments(List<IExpression> arguments, IExpression argument)
        {
            var binaryExpression = argument as BinaryExpression;

            if (
                binaryExpression != null &&
                binaryExpression.ExpressionType == ExpressionType.Add
            ) {
                FlattenConcatArguments(arguments, binaryExpression.Left);
                FlattenConcatArguments(arguments, binaryExpression.Right);
            }
            else
            {
                arguments.Add(argument.Accept(this));
            }
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
