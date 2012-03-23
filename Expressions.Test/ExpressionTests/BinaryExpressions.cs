using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

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
        [Ignore("String.Concat must still be implemented")]
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

        [Test]
        public void LegalCasting()
        {
            Resolve(
                "1l + 1",
                new BinaryExpression(
                    new Constant(1L),
                    new Constant(1),
                    ExpressionType.Add,
                    typeof(long)
                )
            );
        }

        [Test]
        public void LogicalAnd()
        {
            Resolve(
                "true and false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.And,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void LogicalOr()
        {
            Resolve(
                "true or false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.Or,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void LogicalXor()
        {
            Resolve(
                "true xor false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.Xor,
                    typeof(bool)
                )
            );
        }
    }
}
