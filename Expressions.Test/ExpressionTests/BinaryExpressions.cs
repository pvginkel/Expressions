using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;
using ExpressionType = Expressions.Ast.ExpressionType;

namespace Expressions.Test.ExpressionTests
{
    [TestFixture]
    internal class BinaryExpressions : TestBase
    {
        [Test]
        public void BinaryTypeUnchanged()
        {
            Resolve(
                "1 + 2",
                new BinaryExpression(
                    new Constant(1),
                    new Constant(2),
                    ExpressionType.Add,
                    typeof(int)
                )
            );
        }

        [Test]
        public void BinaryAddWithOneString()
        {
            Resolve(
                "1 + \"2\"",
                new BinaryExpression(
                    new Constant(1),
                    new Constant("2"),
                    ExpressionType.Add,
                    typeof(string)
                )
            );
        }
    }
}
