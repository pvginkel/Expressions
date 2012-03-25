using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.VisualBasicLanguage.ExpressionTests
{
    [TestFixture]
    internal class Casting : TestBase
    {
        [Test]
        public void CastWithBuiltInFloat()
        {
            Resolve(
                "ctype(7, single)",
                new Cast(
                    new Constant(7),
                    typeof(float)
                )
            );
        }

        [Test]
        public void CastWithExpression()
        {
            Resolve(
                "ctype(1 + 2, single)",
                new Cast(
                    new BinaryExpression(
                        new Constant(1),
                        new Constant(2),
                        ExpressionType.Add,
                        typeof(int)
                    ),
                    typeof(float)
                )
            );
        }

        [Test]
        public void CastingWithFullType()
        {
            Resolve(
                "ctype(7, System.Single)",
                new Cast(
                    new Constant(7),
                    typeof(float)
                )
            );
        }

        [Test]
        public void CastingWithBuiltInStringArray()
        {
            Resolve(
                "ctype(nothing, string())",
                new Cast(
                    new Constant(null),
                    typeof(string[])
                )
            );
        }

        [Test]
        public void CastingWithBuiltInStringArrayRank2()
        {
            Resolve(
                "ctype(nothing, string(,))",
                new Cast(
                    new Constant(null),
                    typeof(string[,])
                )
            );
        }

        [Test]
        public void CastingWithBuiltInStringArrayRank3()
        {
            Resolve(
                "ctype(nothing, string(,,))",
                new Cast(
                    new Constant(null),
                    typeof(string[, ,])
                )
            );
        }

        [Test]
        public void CastingWithFullStringArray()
        {
            Resolve(
                "ctype(nothing, System.String())",
                new Cast(
                    new Constant(null),
                    typeof(string[])
                )
            );
        }

        [Test]
        public void CastingWithFullStringArrayRank2()
        {
            Resolve(
                "ctype(nothing, System.String(,))",
                new Cast(
                    new Constant(null),
                    typeof(string[,])
                )
            );
        }

        [Test]
        public void CastingWithFullStringArrayRank3()
        {
            Resolve(
                "ctype(nothing, System.String(,,))",
                new Cast(
                    new Constant(null),
                    typeof(string[, ,])
                )
            );
        }

        [Test]
        [ExpectedException]
        public void CastingToUnknownType()
        {
            Resolve(
                "ctype(nothing, Unknown.Type)"
            );
        }

        [Test]
        public void CTypeString()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable") { Value = new object() });

            Resolve(
                context,
                "ctype(Variable, string)",
                new MethodCall(
                    new TypeAccess(typeof(Microsoft.VisualBasic.CompilerServices.Conversions)),
                    typeof(Microsoft.VisualBasic.CompilerServices.Conversions).GetMethod("ToString", new[] { typeof(object) }),
                    new[]
                    {
                        new VariableAccess(typeof(object), 0)
                    }
                )
            );
        }

        [Test]
        public void RemovedCType()
        {
            Resolve(
                "ctype(1, short)",
                new Cast(
                    new Constant(1),
                    typeof(short)
                )
            );
        }
    }
}
