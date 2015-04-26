using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Expressions;

namespace Expressions.Test
{
    internal class ExplicitTypeTests : ExpressionTests
    {
        public ExplicitTypeTests()
            : base(ExpressionLanguage.Csharp)
        {
        }

        [Test]
        public void DoublePlusInt()
        {
            TestValidExpression("double", "Int32A + DoubleA", "100100.25");
        }

        [Test]
        public void BooleanString()
        {
            TestValidExpression("boolean", "StringA == InstanceA", "false");
        }

        [Test]
        public void ShouldFail()
        {
            TestInvalidValidExpression("System.object", "DayOfWeek.Friday = ConsoleModifiers.Alt", "TypeMismatch");
        }

        private void TestValidExpression(string type, string test, string result)
        {
            MyCurrentContext = MyGenericContext;

            string typeName = string.Concat("System.", type);
            Type expressionType = Type.GetType(typeName, true, true);

            ExpressionContext context = MyCurrentContext;

            var expression = new DynamicExpression(test, Language);

            DoTest(expression, context, result, expressionType, ExpressionTests.TestCulture);

        }

        private void TestInvalidValidExpression(string type, string test, string result)
        {
            var reason = (ExpressionsExceptionType)Enum.Parse(typeof(ExpressionsExceptionType), result, true);

            var options = new BoundExpressionOptions
            {
                ResultType = Type.GetType(type, true, true),
                AllowPrivateAccess = true
            };

            try
            {
                new DynamicExpression(test, ExpressionLanguage.Flee).Bind(MyGenericContext, options);
                Assert.Fail("Compile exception expected");
            }
            catch (ExpressionsException ex)
            {
                Assert.AreEqual(reason, ex.Type, string.Format("Expected exception type '{0}' but got '{1}'", reason, ex.Type));
            }
        }

        [Test]
        public void RaisesGetVariableTypeEvent()
        {
            var context = new ExpressionContext();
            context.ResolveVariableType += context_ResolveVariableType;
            context.ResolveVariableValue += context_ResolveVariableValue;

            var expr = new DynamicExpression<int>("a + b", ExpressionLanguage.Csharp);
            var s = expr.Invoke(context);
            Assert.That(s, Is.EqualTo("a".GetHashCode() + "b".GetHashCode()));
        }

        void context_ResolveVariableType(object sender, ResolveVariableTypeEventArgs e)
        {
            e.Result = typeof(int);
        }

        void context_ResolveVariableValue(object sender, ResolveVariableValueEventArgs e)
        {
            e.Result = e.Variable.GetHashCode();
        }
    }
}
