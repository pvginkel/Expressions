using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.Compilation
{
    [TestFixture]
    internal class Casting : TestBase
    {
        [Test]
        public void CastWithBuiltInFloat()
        {
            Resolve("cast(7, single)", 7.0f);
        }

        [Test]
        public void CastWithExpression()
        {
            Resolve("cast(1 + 2, single)", 3f);
        }

        [Test]
        public void CastingWithBuiltInStringArray()
        {
            Resolve("cast(null, string[])", null);
        }

        [Test]
        public void CastingWithBuiltInStringArrayRank2()
        {
            Resolve("cast(null, string[,])", null);
        }

        [Test]
        public void CastingWithBuiltInStringArrayRank3()
        {
            Resolve("cast(null, string[,,])", null);
        }

        [Test]
        public void CastingWithFullStringArray()
        {
            Resolve("cast(null, System.String[])", null);
        }

        [Test]
        public void CastingWithFullStringArrayRank2()
        {
            Resolve("cast(null, System.String[,])", null);
        }

        [Test]
        public void CastingWithFullStringArrayRank3()
        {
            Resolve("cast(null, System.String[,,])", null);
        }

        [Test]
        [ExpectedException]
        public void CastWithBuiltInString()
        {
            Resolve("cast(7, string)");
        }

        [Test]
        public void ImplicitCast()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Owner") { Value = new Owner() });

            Resolve(
                context,
                "Owner",
                1.0,
                new BoundExpressionOptions
                {
                    ResultType = typeof(double)
                }
            );
        }

        [Test]
        public void OperatorAdd()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Owner") { Value = new Owner() });

            Resolve(
                context,
                "Owner + 7",
                7
            );
        }

        [Test]
        public void LongNumber()
        {
            Resolve(
                "0x8000000000000000",
                9223372036854775808,
                new BoundExpressionOptions
                {
                    ResultType = typeof(ulong)
                }
            );
        }

        [Test]
        public void LongSignedNumber()
        {
            Resolve(
                "-0x7fffffffffffffff",
                -9223372036854775807,
                new BoundExpressionOptions
                {
                    ResultType = typeof(long)
                }
            );
        }

        [Test]
        public void MinusDecimal()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Decimal") { Value = 100m });

            Resolve(
                context,
                "-Decimal",
                -100m
            );
        }

        public class Owner
        {
            public static implicit operator double(Owner value)
            {
                return 1.0;
            }

            public static int operator +(Owner owner, int value)
            {
                return value;
            }
        }
    }
}
