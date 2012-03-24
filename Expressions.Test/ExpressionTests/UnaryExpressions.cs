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
        public void UnaryPlus()
        {
            Resolve(
                "+1",
                new UnaryExpression(
                    new Constant(1),
                    typeof(int),
                    ExpressionType.Plus
                )
            );
        }

        [Test]
        public void UnaryMinus()
        {
            Resolve(
                "-1",
                new Constant(-1)
            );
        }

        [Test]
        [ExpectedException]
        public void IllegalUnaryPlus()
        {
            Resolve(
                "+\"\""
            );
        }

        [Test]
        [ExpectedException]
        public void IllegalUnaryMinus()
        {
            Resolve(
                "-\"\""
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
                    ExpressionType.Not
                )
            );
        }

        [Test]
        [ExpectedException]
        public void IllegalUnaryNot()
        {
            Resolve(
                "not \"\""
            );
        }
    }
}
