using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.VisualBasicLanguage.Parsing
{
    [TestFixture]
    internal class Parsing
    {
        [Test]
        public void ValidSyntaxCheck()
        {
            DynamicExpression.CheckSyntax(
                "1", ExpressionLanguage.VisualBasic
            );
        }

        [Test]
        [ExpectedException]
        public void InvalidSyntaxCheck()
        {
            DynamicExpression.CheckSyntax(
                "?", ExpressionLanguage.VisualBasic
            );
        }
    }
}
