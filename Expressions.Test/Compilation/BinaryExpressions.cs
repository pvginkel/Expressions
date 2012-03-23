using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.Compilation
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
            Resolve("true and true", true);
            Resolve("true and false", false);
            Resolve("false and true", false);
            Resolve("false and false", false);
            Resolve("true or true", true);
            Resolve("true or false", true);
            Resolve("false or true", true);
            Resolve("false or false", false);
            Resolve("true xor true", false);
            Resolve("true xor false", true);
            Resolve("false xor true", true);
            Resolve("false xor false", false);
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
    }
}
