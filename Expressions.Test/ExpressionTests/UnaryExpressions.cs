using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.ExpressionTests
{
    [TestFixture]
    internal class UnaryExpressions : TestBase
    {
        [Test]
        public void UnaryMinus()
        {
            Resolve(
                "-1",
                new UnaryExpression(
                    new Constant(1),
                    typeof(int),
                    Ast.ExpressionType.Minus
                )
            );
        }

        [Test]
        public void UnaryNot()
        {
            Resolve(
                "not true",
                new UnaryExpression(
                    new Constant(true),
                    typeof(bool),
                    Ast.ExpressionType.Not
                )
            );
        }
    }
}
