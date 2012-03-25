// From http://flee.codeplex.com/

using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage
{
    [TestFixture()]
    public class BulkTests : ExpressionTests
    {
        [TestFixtureSetUp]
        public void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [Test(Description = "Expressions that should be valid")]
        public void TestValidExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("ValidExpressions.txt", DoTestValidExpressions);
        }

        [Test(Description = "Type descriptor expressions that should be valid")]
        [Ignore("Not yet implemented")]
        public void TestTypeDescriptorExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("TypeDescriptor.txt", DoTestValidExpressions);
        }

        [Test(Description = "On demand method expressions that should be valid")]
        [Ignore("Not yet implemented")]
        public void TestOnDemandMethodExpressions()
        {
            MyCurrentContext = MyGenericContext;

            ProcessScriptTests("OnDemandMethods.txt", DoTestValidExpressions);
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
            //options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

            AssertCompileException(arr[1], context);
        }

        private void DoTestCheckedExpressions(string[] arr)
        {
            string expression = arr[0];
            bool @checked = bool.Parse(arr[1]);
            bool shouldOverflow = bool.Parse(arr[2]);

            var imports = new[]
            {
                new Import(typeof(Math)),
                // ImportBuiltinTypes
                new Import("boolean", typeof(bool)),
		        new Import("byte", typeof(byte)),
		        new Import("sbyte", typeof(sbyte)),
		        new Import("short", typeof(short)),
		        new Import("ushort", typeof(ushort)),
		        new Import("int", typeof(int)),
		        new Import("uint", typeof(uint)),
		        new Import("long", typeof(long)),
		        new Import("ulong", typeof(ulong)),
		        new Import("single", typeof(float)),
		        new Import("double", typeof(double)),
		        new Import("decimal", typeof(decimal)),
		        new Import("char", typeof(char)),
		        new Import("object", typeof(object)),
		        new Import("string", typeof(string))
           };

            ExpressionContext context = new ExpressionContext(imports, MyValidExpressionsOwner);

            try
            {
                DynamicExpression e = CreateDynamicExpression(expression, context);
                e.Invoke(context, new BoundExpressionOptions
                {
                    Checked = @checked
                });
                Assert.IsFalse(shouldOverflow);
            }
            catch (OverflowException)
            {
                Assert.IsTrue(shouldOverflow);
            }
        }
    }

}
