using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Expressions.Expressions;

namespace Expressions.Test.VisualBasicLanguage.ExpressionTests
{
    [TestFixture]
    internal class StringExpressions : TestBase
    {
        [Test]
        public void Escaping()
        {
            Resolve(
                @"""\d""",
                new Constant(@"\d")
            );
        }

        [Test]
        public void Quotes()
        {
            Resolve(
                "\"\"\"\"",
                new Constant("\"")
            );
        }
    }
}
