using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;

namespace Expressions
{
    internal class ConversionVisitor : ExpressionVisitor
    {
        private Resolver _resolver;

        public ConversionVisitor(Resolver resolver)
        {
            Require.NotNull(resolver, "resolver");

            _resolver = resolver;
        }

        public override IExpression BinaryExpression(BinaryExpression binaryExpression)
        {
            if (binaryExpression.ExpressionType == ExpressionType.Add)
            {
                var left = binaryExpression.Left.Accept(this);
                var right = binaryExpression.Right.Accept(this);

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
                        return new BinaryExpression(left, right, binaryExpression.ExpressionType, binaryExpression.Type);
                }
            }

            return base.BinaryExpression(binaryExpression);
        }
    }
}
