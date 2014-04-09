using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Test.CsharpLanguage.ExpressionTests;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.Issues
{
    [TestFixture]
    internal class Issue14Fixture : TestBase
    {
        [Test]
        [ExpectedException(typeof(ExpressionsException))]
        public void Test()
        {
            string translatedString = "uppercase(\"hello\")";
            var expr = new DynamicExpression(translatedString, ExpressionLanguage.Csharp);
            var context = new ExpressionContext(null, new CustomOwner(), true);
            var boundExpression = expr.Bind(context);
            object res = boundExpression.Invoke();
        }

        public class CustomOwner
        {
            public string uppercase(string str)
            {
                return str.ToUpperInvariant();
            }
        }
    }
}
