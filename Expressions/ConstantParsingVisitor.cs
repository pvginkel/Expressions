using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Expressions.Ast;

namespace Expressions
{
    internal class ConstantParsingVisitor : AstVisitor
    {
        private Resolver _resolver;

        public ConstantParsingVisitor(Resolver resolver)
        {
            Require.NotNull(resolver, "resolver");

            _resolver = resolver;
        }

        public override IAstNode UnaryExpression(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Type == ExpressionType.Minus)
            {
                var constant = unaryExpression.Operand as Constant;

                if (constant != null)
                {
                    var unparsedNumber = constant.Value as UnparsedNumber;

                    // We do not parse hex in this manner because int.Parse
                    // doesn't allow hex and sign to be combined.

                    if (
                        unparsedNumber != null &&
                        TypeUtil.IsConvertible(unparsedNumber.Type) &&
                        (unparsedNumber.NumberStyles & NumberStyles.AllowHexSpecifier) == 0
                    ) {
                        // Actually parse the constant including the minus sign.

                        unparsedNumber = new UnparsedNumber("-" + unparsedNumber.Value, unparsedNumber.Type, unparsedNumber.NumberStyles | NumberStyles.AllowLeadingSign);

                        return new Constant(
                            unparsedNumber.Parse(_resolver.DynamicExpression.ParsingCulture)
                        );
                    }
                }
            }

            return base.UnaryExpression(unaryExpression);
        }

        public override IAstNode Constant(Constant constant)
        {
            var unparsedNumber = constant.Value as UnparsedNumber;

            if (unparsedNumber != null)
                return new Constant(unparsedNumber.Parse(_resolver.DynamicExpression.ParsingCulture));

            return base.Constant(constant);
        }
    }
}
