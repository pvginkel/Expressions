// From http://flee.codeplex.com/

using System;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage
{
    [TestFixture()]
    public class BulkTests : ExpressionTests
    {
        [Test(Description = "Expressions that should be valid")]
        public void TestValidExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("ValidExpressions.txt", DoTestValidExpressions);
        }

        [Test(Description = "Expressions that should not be valid")]
        public void TestInvalidExpressions()
        {
            ProcessScriptTests("InvalidExpressions.txt", DoTestInvalidExpressions);
        }

        [Test(Description = "Casts that should be valid")]
        public void TestValidCasts()
        {
            MyCurrentContext = MyValidCastsContext;
            ProcessScriptTests("ValidCasts.txt", DoTestValidExpressions);
        }

        [Test(Description = "Test our handling of checked expressions")]
        public void TestCheckedExpressions()
        {
            ProcessScriptTests("CheckedTests.txt", DoTestCheckedExpressions);
        }

        private void DoTestValidExpressions(string[] arr)
        {
            string typeName = string.Concat("System.", arr[0]);
            Type expressionType = Type.GetType(typeName, true, true);

            ExpressionContext context = MyCurrentContext;
            //context.Options.ResultType = expressionType;

            DynamicExpression e = CreateDynamicExpression(arr[1], context);
            DoTest(e, context, arr[2], expressionType, ExpressionTests.TestCulture);
        }

        private DynamicExpression CreateDynamicExpression(string expression, ExpressionContext context)
        {
            return new DynamicExpression(expression, ExpressionLanguage.Flee);
        }

        private void DoTestInvalidExpressions(string[] arr)
        {
            Type expressionType = Type.GetType(arr[0], true, true);

            //CompileExceptionReason reason = System.Enum.Parse(typeof(CompileExceptionReason), arr[2], true);

            ExpressionContext context = MyGenericContext;
            //ExpressionOptions options = context.Options;
            //options.ResultType = expressionType;
            context.Imports.Add(new Import(typeof(Math)));
            //options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

            AssertCompileException(arr[1], context);
        }

        private void DoTestCheckedExpressions(string[] arr)
        {
            string expression = arr[0];
            bool @checked = bool.Parse(arr[1]);
            bool shouldOverflow = bool.Parse(arr[2]);

            ExpressionContext context = new ExpressionContext(null, MyValidExpressionsOwner);
            //ExpressionOptions options = context.Options;
            context.Imports.Add(new Import(typeof(Math)));
            //context.Imports.ImportBuiltinTypes();
            //options.Checked = @checked;

            try
            {
                DynamicExpression e = CreateDynamicExpression(expression, context);
                e.Invoke(context);
                Assert.IsFalse(shouldOverflow);
            }
            catch (OverflowException)
            {
                Assert.IsTrue(shouldOverflow);
            }
        }
    }

}
