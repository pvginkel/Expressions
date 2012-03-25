using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
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
                new MethodCall(
                    new TypeAccess(typeof(string)),
                    typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) }),
                    new IExpression[]
                    {
                        new Constant(1),
                        new Constant("2")
                    }
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
                "true && false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.LogicalAnd,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void LogicalOr()
        {
            Resolve(
                "true || false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.LogicalOr,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void LogicalXor()
        {
            Resolve(
                "true ^ false",
                new BinaryExpression(
                    new Constant(true),
                    new Constant(false),
                    ExpressionType.Xor,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void Calculation()
        {
            Resolve(
                "2147483648U / 2u",
                new BinaryExpression(
                    new Constant(2147483648u),
                    new Constant(2u),
                    ExpressionType.Divide,
                    typeof(uint)
                )
            );
        }

        [Test]
        public void MultipleAdditions()
        {
            Resolve(
                "100 + .25 + 000.25 + 1.50",
                new BinaryExpression(
                    new BinaryExpression(
                        new BinaryExpression(
                            new Constant(100),
                            new Constant(0.25),
                            ExpressionType.Add,
                            typeof(double)
                        ),
                        new Constant(0.25),
                        ExpressionType.Add,
                        typeof(double)
                    ),
                    new Constant(1.5),
                    ExpressionType.Add,
                    typeof(double)
                )
            );
        }

        [Test]
        public void CompareAndEquals()
        {
            Resolve(
                "10 > 2 == true",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(10),
                        new Constant(2),
                        ExpressionType.Greater,
                        typeof(bool),
                        typeof(int)
                    ),
                    new Constant(true),
                    ExpressionType.Equals,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void ComparesWithLongCast()
        {
            Resolve(
                "-100 > 100U",
                new BinaryExpression(
                    new Constant(-100),
                    new Constant(100u),
                    ExpressionType.Greater,
                    typeof(bool),
                    typeof(long)
                )
            );
        }

        [Test]
        public void MultipleBitwise()
        {
            Resolve(
                "123 & 100 | 1245 ^ 80",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(123),
                        new Constant(100),
                        ExpressionType.BitwiseAnd,
                        typeof(int)
                    ),
                    new BinaryExpression(
                        new Constant(1245),
                        new Constant(80),
                        ExpressionType.Xor,
                        typeof(int)
                    ),
                    ExpressionType.BitwiseOr,
                    typeof(int)
                )
            );
        }

        [Test]
        public void PrecedenceOfNot()
        {
            Resolve(
                "!(1 > 100)",
                new UnaryExpression(
                    new UnaryExpression(
                        new BinaryExpression(
                            new Constant(1),
                            new Constant(100),
                            ExpressionType.Greater,
                            typeof(bool),
                            typeof(int)
                        ),
                        typeof(bool),
                        ExpressionType.Group
                    ),
                    typeof(bool),
                    ExpressionType.LogicalNot
                )
            );
        }

        [Test]
        public void AndAndNot()
        {
            Resolve(
                "true && !false && true",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(true),
                        new UnaryExpression(
                            new Constant(false),
                            typeof(bool),
                            ExpressionType.LogicalNot
                        ),
                        ExpressionType.LogicalAnd,
                        typeof(bool)
                    ),
                    new Constant(true),
                    ExpressionType.LogicalAnd,
                    typeof(bool)
                )
            );
        }
    }
}
