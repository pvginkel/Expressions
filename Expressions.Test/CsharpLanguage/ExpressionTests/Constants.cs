using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.ExpressionTests
{
    [TestFixture]
    internal class Constants : TestBase
    {
        [Test]
        public void Max()
        {
            Resolve(
                int.MinValue.ToString(),
                new Constant(int.MinValue)
            );
        }
    }
}
