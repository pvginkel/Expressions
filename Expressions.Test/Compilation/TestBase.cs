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
            Resolve(null, expression, expected);
        }

        protected void Resolve(ExpressionContext expressionContext, string expression, object expected)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            object actual = new DynamicExpression(
                expression,
                ExpressionLanguage.Flee
            ).Invoke(
                expressionContext ?? new ExpressionContext()
            );

            Assert.AreEqual(expected, actual);
        }
    }
}
