using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using Microsoft.VisualBasic.CompilerServices;
using NUnit.Framework;

namespace Expressions.Test.VisualBasicLanguage.ExpressionTests
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
                "true andalso false",
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
                "true orelse false",
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

        [Test]
        public void Calculation()
        {
            Resolve(
                "2147483648Ui / 2ui",
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
                "10 > 2 = true",
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
                "-100 > 100Ui",
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
                "123 and 100 or 1245 xor 80",
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
                "not 1 > 100",
                new UnaryExpression(
                    new BinaryExpression(
                        new Constant(1),
                        new Constant(100),
                        ExpressionType.Greater,
                        typeof(bool),
                        typeof(int)
                    ),
                    typeof(bool),
                    ExpressionType.Not
                )
            );
        }

        [Test]
        public void AndAndNot()
        {
            Resolve(
                "true andalso not false andalso true",
                new BinaryExpression(
                    new BinaryExpression(
                        new Constant(true),
                        new UnaryExpression(
                            new Constant(false),
                            typeof(bool),
                            ExpressionType.Not
                        ),
                        ExpressionType.And,
                        typeof(bool)
                    ),
                    new Constant(true),
                    ExpressionType.And,
                    typeof(bool)
                )
            );
        }

        [Test]
        public void CompareString()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable") { Value = "ab" });

            Resolve(
                context,
                "Variable = \"a\" + \"b\"",
                new Cast(
                    new MethodCall(
                        new TypeAccess(typeof(Operators)),
                        typeof(Operators).GetMethod("CompareObjectEqual"),
                        new IExpression[]
                        {
                            new VariableAccess(typeof(string), 0),
                            new MethodCall(
                                new TypeAccess(typeof(string)),
                                typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }),
                                new IExpression[]
                                {
                                    new Constant("a"),
                                    new Constant("b")
                                }
                            ),
                            new Constant(false)
                        }
                    ),
                    typeof(bool)
                )
            );
        }

        [Test]
        public void EqualsAndNotEquals()
        {
            Resolve(
                "100 = 20 = false <> false",
                new BinaryExpression(
                    new BinaryExpression(
                        new BinaryExpression(
                            new Constant(100),
                            new Constant(20),
                            ExpressionType.Equals,
                            typeof(bool),
                            typeof(int)
                        ),
                        new Constant(false),
                        ExpressionType.Equals,
                        typeof(bool)
                    ),
                    new Constant(false),
                    ExpressionType.NotEquals,
                    typeof(bool)
                )
            );
        }
    }
}
