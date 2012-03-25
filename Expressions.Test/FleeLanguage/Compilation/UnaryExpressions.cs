using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage.Compilation
{
    [TestFixture]
    internal class UnaryExpressions : TestBase
    {
        [Test]
        public void Arithatics()
        {
            Resolve("+1", 1);
            Resolve("-1", -1);

            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable1") { Value = 1 });

            Resolve(context, "+Variable1", 1);
            Resolve(context, "-Variable1", -1);
        }

        [Test]
        public void Logicals()
        {
            Resolve("not true", false);
        }
    }
}
