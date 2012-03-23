using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.ExpressionTests
{
    [TestFixture]
    internal class Casting : TestBase
    {
        [Test]
        public void CastWithBuiltInFloat()
        {
            Resolve(
                "cast(7, float)",
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
                "cast(1 + 2, float)",
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
                "cast(7, System.Single)",
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
                "cast(null, string[])",
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
                "cast(null, string[,])",
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
                "cast(null, string[,,])",
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
                "cast(null, System.String[])",
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
                "cast(null, System.String[,])",
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
                "cast(null, System.String[,,])",
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
                "cast(null, Unknown.Type)"
            );
        }
    }
}
