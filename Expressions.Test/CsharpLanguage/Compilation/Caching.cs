using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    [TestFixture]
    internal class Caching : TestBase
    {
        [Test]
        public void SameExpressionReuses()
        {
            Assert.AreSame(
                new DynamicExpression("1", ExpressionLanguage.Csharp).Cached,
                new DynamicExpression("1", ExpressionLanguage.Csharp).Cached
            );
        }

        [Test]
        public void SameTypes()
        {
            var dynamicExpression = new DynamicExpression("Variable", ExpressionLanguage.Csharp);

            var context = new ExpressionContext();

            context.Variables.Add(new Variable("Variable") { Value = 1 });

            Assert.AreSame(
                dynamicExpression.Bind(context),
                dynamicExpression.Bind(context)
            );
        }

        [Test]
        public void DifferentTypesDifferentCache()
        {
            var dynamicExpression = new DynamicExpression("Variable", ExpressionLanguage.Csharp);

            var context1 = new ExpressionContext();

            context1.Variables.Add(new Variable("Variable") { Value = 1 });

            var context2 = new ExpressionContext();

            context2.Variables.Add(new Variable("Variable") { Value = 1d });

            Assert.AreNotSame(
                dynamicExpression.Bind(context1),
                dynamicExpression.Bind(context2)
            );
        }

        [Test]
        public void UnusedDifferentTypesAreSame()
        {
            var dynamicExpression = new DynamicExpression("1", ExpressionLanguage.Csharp);

            var context1 = new ExpressionContext();

            context1.Variables.Add(new Variable("Variable") { Value = 1 });

            var context2 = new ExpressionContext();

            context2.Variables.Add(new Variable("Variable") { Value = 1d });

            Assert.AreSame(
                dynamicExpression.Bind(context1),
                dynamicExpression.Bind(context2)
            );
        }
    }
}
