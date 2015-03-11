using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Expressions;

namespace Expressions.Test
{
    internal class MyTests : ExpressionTests
    {
        public MyTests()
            : base(ExpressionLanguage.Csharp)
        {
        }

        [Test]
        public void DoublePlusInt()
        {
            var str = "double;Int32A + DoubleA;100100.25";
            TestValidExpression(str);
        }

        [Test]
        public void BooleanString()
        {
            var str = "boolean;StringA == InstanceA;false";
            TestValidExpression(str);
        }

        private void TestValidExpression(string expr)
        {
            MyCurrentContext = MyGenericContext;

            var arr = expr.Split(';');

            string typeName = string.Concat("System.", arr[0]);
            Type expressionType = Type.GetType(typeName, true, true);

            ExpressionContext context = MyCurrentContext;

            var expression = new DynamicExpression(arr[1], Language);

            DoTest(expression, context, arr[2], expressionType, ExpressionTests.TestCulture);

        }


    }
}
