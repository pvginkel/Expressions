using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;
using Expressions.ResolvedAst;
using NUnit.Framework;

namespace Expressions.Test.Resolving
{
    [TestFixture]
    internal class Casting : ResolvingTestBase
    {
        [Test]
        public void CastWithBuiltInFloat()
        {
            Resolve(
                "cast(7, float)",
                new ResolvedCast(
                    new ResolvedConstant(7),
                    typeof(float)
                )
            );
        }

        [Test]
        public void CastWithBuiltInString()
        {
            Resolve(
                "cast(7, string)",
                new ResolvedCast(
                    new ResolvedConstant(7),
                    typeof(string)
                )
            );
        }

        [Test]
        public void CastWithExpression()
        {
            Resolve(
                "cast(1 + 2, float)",
                new ResolvedCast(
                    new ResolvedBinaryExpression(
                        new ResolvedConstant(1),
                        new ResolvedConstant(2),
                        typeof(int),
                        ExpressionType.Add
                    ),
                    typeof(float)
                )
            );
        }

        [Test]
        public void CastingWithFullType()
        {
            Resolve(
                "cast(7, System.String)",
                new ResolvedCast(
                    new ResolvedConstant(7),
                    typeof(string)
                )
            );
        }

        [Test]
        public void CastingWithBuiltInStringArray()
        {
            Resolve(
                "cast(null, string[])",
                new ResolvedCast(
                    new ResolvedConstant(null),
                    typeof(string[])
                )
            );
        }

        [Test]
        public void CastingWithBuiltInStringArrayRank2()
        {
            Resolve(
                "cast(null, string[,])",
                new ResolvedCast(
                    new ResolvedConstant(null),
                    typeof(string[,])
                )
            );
        }

        [Test]
        public void CastingWithBuiltInStringArrayRank3()
        {
            Resolve(
                "cast(null, string[,,])",
                new ResolvedCast(
                    new ResolvedConstant(null),
                    typeof(string[, ,])
                )
            );
        }

        [Test]
        public void CastingWithFullStringArray()
        {
            Resolve(
                "cast(null, System.String[])",
                new ResolvedCast(
                    new ResolvedConstant(null),
                    typeof(string[])
                )
            );
        }

        [Test]
        public void CastingWithFullStringArrayRank2()
        {
            Resolve(
                "cast(null, System.String[,])",
                new ResolvedCast(
                    new ResolvedConstant(null),
                    typeof(string[,])
                )
            );
        }

        [Test]
        public void CastingWithFullStringArrayRank3()
        {
            Resolve(
                "cast(null, System.String[,,])",
                new ResolvedCast(
                    new ResolvedConstant(null),
                    typeof(string[, ,])
                )
            );
        }

        [Test]
        [ExpectedException]
        public void CastingToUnknownType()
        {
            Resolve(
                "cast(null, Unknown.Type)"
            );
        }
    }
}
