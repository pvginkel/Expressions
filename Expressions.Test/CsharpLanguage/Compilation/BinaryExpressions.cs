using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    [TestFixture]
    internal class BinaryExpressions : TestBase
    {
        [Test]
        public void Constants()
        {
            Resolve("1 + 1", 2);
            Resolve("1 + 1.1", 2.1);
            Resolve("\"hi\"", "hi");
        }

        [Test]
        public void ConstantAndVariable()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable1") { Value = 1 });
            context.Variables.Add(new Variable("Variable2") { Value = 1.1 });

            Resolve(context, "Variable1 + 1", 2);
            Resolve(context, "Variable2 + 1", 2.1);
        }

        [Test]
        public void Logicals()
        {
            Resolve("true && true", true);
            Resolve("true && false", false);
            Resolve("false && true", false);
            Resolve("false && false", false);
            Resolve("true || true", true);
            Resolve("true || false", true);
            Resolve("false || true", true);
            Resolve("false || false", false);
            Resolve("true ^ true", false);
            Resolve("true ^ false", true);
            Resolve("false ^ true", true);
            Resolve("false ^ false", false);
        }

        [Test]
        public void Calculation()
        {
            Resolve("2147483648U / 2u", 1073741824u);
        }

        [Test]
        public void Remainder()
        {
            Resolve("2147483648U % 5U", 3);
        }

        [Test]
        public void GreaterEquals()
        {
            Resolve("100>=1", true);
        }

        [Test]
        public void UnsignedLessThan()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("uint", typeof(uint)) }),
                "uint.MinValue < uint.MaxValue",
                true
            );
        }

        [Test]
        public void CompareString()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable") { Value = "ab" });

            Resolve(
                context,
                "Variable == \"a\" + \"b\"",
                true
            );
        }

        [Test]
        public void UnsignedShiftRight()
        {
            Resolve(
                "100U >> 2",
                25u
            );
        }

        [Test]
        public void LongShiftLeft()
        {
            Resolve(
                "100L << 2",
                400
            );
        }

        [Test]
        public void HexConstant()
        {
            Resolve("0x80000000.GetType()", typeof(uint));
        }

        [Test]
        public void ShiftWithSignedRight()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Left") { Value = 0x80000000 });
            context.Variables.Add(new Variable("Right") { Value = -1 });

            Resolve(
                context,
                "Left >> Right",
                1
            );
        }
    }
}
