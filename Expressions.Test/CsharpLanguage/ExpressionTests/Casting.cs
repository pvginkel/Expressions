using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    [TestFixture]
    internal class Casting : TestBase
    {
        [Test]
        public void CastWithBuiltInFloat()
        {
            Resolve(
                "(float)7",
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
                "(float)(1 + 2)",
                new Cast(
                    new UnaryExpression(
                        new BinaryExpression(
                            new Constant(1),
                            new Constant(2),
                            ExpressionType.Add,
                            typeof(int)
                        ),
                        typeof(int),
                        ExpressionType.Group
                    ),
                    typeof(float)
                )
            );
        }

        [Test]
        public void CastingWithFullType()
        {
            Resolve(
                "(System.Single)7",
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
                "(string[])null",
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
                "(string[,])null",
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
                "(string[,,])null",
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
                "(System.String[])null",
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
                "(System.String[,])null",
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
                "(System.String[,,])null",
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
                "(Unknown.Type)null"
            );
        }
    }
}
