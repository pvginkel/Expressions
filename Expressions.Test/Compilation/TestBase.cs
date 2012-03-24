using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.Compilation
{
    internal abstract class TestBase
    {
        protected void Resolve(string expression)
        {
            Resolve(null, expression);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression)
        {
            Resolve(expressionContext, expression, null);
        }

        protected void Resolve(string expression, object expected)
        {
            Resolve(expression, expected, null);
        }

        protected void Resolve(string expression, object expected, BoundExpressionOptions options)
        {
            Resolve(null, expression, expected, options);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression, object expected)
        {
            Resolve(expressionContext, expression, expected, null);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression, object expected, BoundExpressionOptions options)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            object actual = new DynamicExpression(
                expression,
                ExpressionLanguage.Flee
            ).Invoke(
                expressionContext ?? new ExpressionContext(),
                options
            );

            Assert.AreEqual(expected, actual);
        }
    }
}
