using System.Text;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Globalization;
using System.Threading;

namespace Expressions.Test.FleeLanguage
{
    [TestFixture]
    internal class IndividualTests : Test.ExpressionTests
    {
        public IndividualTests()
            : base(ExpressionLanguage.Flee)
        {
        }

        [Test(Description = "Test that we properly handle newline escapes in strings")]
        public void TestStringNewlineEscape()
        {
            GenericExpression<string> e = this.CreateGenericExpression<string>("\"a\\r\\nb\"");
            string s = e.Invoke();
            string expected = string.Format("a{0}b", ControlChars.CrLf);
            Assert.AreEqual(expected, s);
        }

        [Test(Description = "Test that we can parse from multiple threads")]
        public void TestMultiTreadedParse()
        {
            Thread t1 = new Thread(ThreadRunParse);
            t1.Name = "Thread1";

            Thread t2 = new Thread(ThreadRunParse);
            t2.Name = "Thread2";

            ExpressionContext context = new ExpressionContext();

            t1.Start(context);
            t2.Start(context);

            t1.Join();
            t2.Join();
        }

        private void ThreadRunParse(object o)
        {
            ExpressionContext context = (ExpressionContext)o;
            // Test parse
            for (int i = 0; i <= 40 - 1; i++)
            {
                var e = this.CreateDynamicExpression("1+1*200", context);
            }
        }

        [Test(Description = "Test that we can parse from multiple threads")]
        public void TestMultiTreadedEvaluate()
        {
            System.Threading.Thread t1 = new System.Threading.Thread(ThreadRunEvaluate);
            t1.Name = "Thread1";

            System.Threading.Thread t2 = new System.Threading.Thread(ThreadRunEvaluate);
            t2.Name = "Thread2";

            var e = this.CreateDynamicExpression("1+1*200");

            t1.Start(e);
            t2.Start(e);

            t1.Join();
            t2.Join();
        }

        private void ThreadRunEvaluate(object o)
        {
            // Test evaluate
            var e2 = (DynamicExpression)o;

            for (int i = 0; i <= 40 - 1; i++)
            {
                int result = (int)e2.Invoke();
                Assert.AreEqual(1 + 1 * 200, result);
            }
        }

        [Test(Description = "Test evaluation of generic expressions")]
        public void TestGenericEvaluate()
        {
            ExpressionContext context = default(ExpressionContext);
            context = new ExpressionContext();

            GenericExpression<int> e1 = this.CreateGenericExpression<int>("1000", context);
            Assert.AreEqual(1000, e1.Invoke());

            GenericExpression<double> e2 = this.CreateGenericExpression<double>("1000.25", context);
            Assert.AreEqual(1000.25, e2.Invoke());

            GenericExpression<double> e3 = this.CreateGenericExpression<double>("1000", context);
            Assert.AreEqual(1000.0, e3.Invoke());

            GenericExpression<ValueType> e4 = this.CreateGenericExpression<ValueType>("1000", context);
            ValueType vt = e4.Invoke();
            Assert.AreEqual(1000, vt);

            GenericExpression<object> e5 = this.CreateGenericExpression<object>("1000 + 2.5", context);
            object o = e5.Invoke();
            Assert.AreEqual(1000 + 2.5, o);
        }

        [Test(Description = "Test expression imports")]
        public void TestImports()
        {
            GenericExpression<double> e;
            ExpressionContext context;

            context = new ExpressionContext();
            // Import math type directly
            context.Imports.Add(new Import(typeof(Math)));

            // Should be able to see PI without qualification
            e = this.CreateGenericExpression<double>("pi", context);
            Assert.AreEqual(Math.PI, e.Invoke());

            context = new ExpressionContext(null, MyValidExpressionsOwner);
            // Import math type with prefix
            context.Imports.Add(new Import("math", typeof(Math)));

            // Should be able to see pi by qualifying with Math	
            e = this.CreateGenericExpression<double>("math.pi", context);
            Assert.AreEqual(Math.PI, e.Invoke());

            // Import nothing
            context = new ExpressionContext();
            // Should not be able to see PI
            this.AssertCompileException("pi");
            this.AssertCompileException("math.pi");

            // Test importing of builtin types
            this.CreateGenericExpression<double>("double.maxvalue", new BoundExpressionOptions { ImportBuildInTypes = true });
            this.CreateGenericExpression<string>("string.concat(\"a\", \"b\")", new BoundExpressionOptions { ImportBuildInTypes = true });
            this.CreateGenericExpression<long>("long.maxvalue * 2", new BoundExpressionOptions { ImportBuildInTypes = true });
        }

        [Test(Description = "Test importing of a method")]
        [Ignore("Import of methods is not implemented")]
        public void TestImportMethod()
        {
            //ExpressionContext context = new ExpressionContext();
            //context.Imports.AddMethod("cos", typeof(Math), string.Empty);

            //var e = this.CreateDynamicExpression("cos(100)", context);
            //Assert.AreEqual(Math.Cos(100), (double)e.Invoke());

            //System.Reflection.MethodInfo mi = typeof(int).GetMethod("parse", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase, null, System.Reflection.CallingConventions.Any, new Type[] { typeof(string) }, null);
            //context.Imports.AddMethod(mi, "");

            //e = this.CreateDynamicExpression("parse(\"123\")", context);
            //Assert.AreEqual(123, (int)e.Invoke());
        }

        [Test(Description = "Test that we can import multiple types into a namespace")]
        public void TestImportsNamespaces()
        {
            ExpressionContext context = new ExpressionContext();
            context.Imports.Add(new Import("ns1", typeof(Math)));
            context.Imports.Add(new Import("ns1", typeof(string)));

            var e = this.CreateDynamicExpression("ns1.cos(100)", context);
            Assert.AreEqual(Math.Cos(100), (double)e.Invoke());

            e = this.CreateDynamicExpression("ns1.concat(\"a\", \"b\")", context);
            Assert.AreEqual(string.Concat("a", "b"), (string)e.Invoke());
        }

        [Test(Description = "Test our string equality")]
        [Ignore("String comparison not yet implemented")]
        public void TestStringEquality()
        {
            //ExpressionContext context = new ExpressionContext();
            //ExpressionOptions options = context.Options;

            //GenericExpression<bool> e = default(GenericExpression<bool>);

            //// Should be equal
            //e = this.CreateGenericExpression<bool>("\"abc\" = \"abc\"", context);
            //Assert.IsTrue(e.Invoke());

            //// Should not be equal
            //e = this.CreateGenericExpression<bool>("\"ABC\" = \"abc\"", context);
            //Assert.IsFalse(e.Invoke());

            //// Should be not equal
            //e = this.CreateGenericExpression<bool>("\"ABC\" <> \"abc\"", context);
            //Assert.IsTrue(e.Invoke());

            //// Change string compare type
            //options.StringComparison = StringComparison.OrdinalIgnoreCase;

            //// Should be equal
            //e = this.CreateGenericExpression<bool>("\"ABC\" = \"abc\"", context);
            //Assert.IsTrue(e.Invoke());

            //// Should also be equal
            //e = this.CreateGenericExpression<bool>("\"ABC\" <> \"abc\"", context);
            //Assert.IsFalse(e.Invoke());

            //// Should also be not equal
            //e = this.CreateGenericExpression<bool>("\"A\" <> \"z\"", context);
            //Assert.IsTrue(e.Invoke());
        }

        [Test(Description = "Test expression variables")]
        public void TestVariables()
        {
            this.TestValueTypeVariables();
            this.TestReferenceTypeVariables();
        }

        private void TestValueTypeVariables()
        {
            ExpressionContext context = new ExpressionContext();
            VariableCollection variables = context.Variables;

            variables.Add("a", 100);
            variables.Add("b", -100);
            variables.Add("c", DateTime.Now);

            GenericExpression<int> e1 = this.CreateGenericExpression<int>("a+b", context);
            int result = e1.Invoke();
            Assert.AreEqual(100 + -100, result);

            variables["B"].Value = 1000;
            result = e1.Invoke();
            Assert.AreEqual(100 + 1000, result);

            GenericExpression<string> e2 = this.CreateGenericExpression<string>("c.tolongdatestring() + c.Year.tostring()", context);
            Assert.AreEqual(DateTime.Now.ToLongDateString() + DateTime.Now.Year, e2.Invoke());

            // Test null value
            variables["a"].Value = null;
            e1 = this.CreateGenericExpression<int>("a", context);
            Assert.AreEqual(0, e1.Invoke());
        }

        private void TestReferenceTypeVariables()
        {
            ExpressionContext context = new ExpressionContext();
            VariableCollection variables = context.Variables;

            variables.Add("a", "string");
            variables.Add("b", 100);

            GenericExpression<string> e = this.CreateGenericExpression<string>("a + b + a.tostring()", context);
            string result = e.Invoke();
            Assert.AreEqual("string" + 100 + "string", result);

            variables["a"].Value = "test";
            variables["b"].Value = 1;
            result = e.Invoke();
            Assert.AreEqual("test" + 1 + "test", result);

            // Test null value
            variables["nullVar"].Value = string.Empty;
            variables["nullvar"].Value = null;

            GenericExpression<bool> e2 = this.CreateGenericExpression<bool>("nullvar = null", context);
            Assert.IsTrue(e2.Invoke());
        }

        [Test(Description = "Test that we properly enforce member access on the expression owner")]
        public void TestMemberAccess()
        {
            AccessTestExpressionOwner owner = new AccessTestExpressionOwner();
            ExpressionContext context = new ExpressionContext(null, owner);

            // Private field, access should be denied
            this.AssertCompileException("privateField1", context);

            // Private field but with override allowing access
            var e = this.CreateDynamicExpression("privateField2", context);

            // Private field but with override denying access
            this.AssertCompileException("privateField3", context, new BoundExpressionOptions
            {
                AllowPrivateAccess = true
            });

            // Public field, access should be denied
            this.AssertCompileException("PublicField1", context);

            // Public field, access should be allowed
            e = this.CreateDynamicExpression("publicField1", context);
        }

        [Test(Description = "Test parsing for different culture")]
        [Ignore("Culture sensitive parsing is not supported")]
        public void TestCultureSensitiveParse()
        {
            ExpressionContext context = new ExpressionContext();
            context.Imports.Add(new Import("String", typeof(string)));
            context.Imports.Add(new Import("Math", typeof(Math)));

            GenericExpression<double> e = this.CreateGenericExpression<double>("1,25 + 0,75", context, null);
            Assert.AreEqual(1.25 + 0.75, e.Invoke());

            e = this.CreateGenericExpression<double>("math.pow(1,25 + 0,75; 2)", context, null);
            Assert.AreEqual(Math.Pow(1.25 + 0.75, 2), e.Invoke());

            GenericExpression<string> e2 = this.CreateGenericExpression<string>("string.concat(1;2;3;4)", context, null);
            Assert.AreEqual("1234", e2.Invoke());
        }

        [Test(Description = "Test tweaking of parser options")]
        [Ignore("Advanced parsing options not yet implemented")]
        public void TestParserOptions()
        {
            //ExpressionContext context = new ExpressionContext();
            //context.Imports.Add(new Import("String", typeof(string)));
            //context.Imports.Add(new Import("Math", typeof(Math)));

            //context.ParserOptions.DecimalSeparator = ",";
            //context.ParserOptions.RecreateParser();
            //GenericExpression<double> e = this.CreateGenericExpression<double>("1,25 + 0,75", context);
            //Assert.AreEqual(1.25 + 0.75, e.Invoke());

            //context.ParserOptions.FunctionArgumentSeparator = ";";
            //context.ParserOptions.RecreateParser();
            //e = this.CreateGenericExpression<double>("math.pow(1,25 + 0,75; 2)", context);
            //Assert.AreEqual(Math.Pow(1.25 + 0.75, 2), e.Invoke());

            //e = this.CreateGenericExpression<double>("math.max(,25;,75)", context);
            //Assert.AreEqual(Math.Max(0.25, 0.75), e.Invoke());

            //context.ParserOptions.FunctionArgumentSeparator = ",";
            //context.ParserOptions.DecimalSeparator = ",";
            //context.ParserOptions.RequireDigitsBeforeDecimalPoint = true;
            //context.ParserOptions.RecreateParser();
            //e = this.CreateGenericExpression<double>("math.max(1,25,0,75)", context);
            //Assert.AreEqual(Math.Max(1.25, 0.75), e.Invoke());

            //context.ParserOptions.FunctionArgumentSeparator = ";";
            //context.ParserOptions.RecreateParser();
            //GenericExpression<string> e2 = this.CreateGenericExpression<string>("string.concat(1;2;3;4)", context);
            //Assert.AreEqual("1234", e2.Invoke());

            //// Ambiguous grammar
            //context.ParserOptions.FunctionArgumentSeparator = ",";
            //context.ParserOptions.DecimalSeparator = ",";
            //context.ParserOptions.RequireDigitsBeforeDecimalPoint = false;
            //context.ParserOptions.RecreateParser();
            //this.AssertCompileException("math.max(1,24,4,56)", context, CompileExceptionReason.SyntaxError);
        }

        [Test(Description = "Test getting the variables used in an expression")]
        [Ignore("Getting the variables used in an expression is not implemented")]
        public void TestGetReferencedVariables()
        {
            //ExpressionContext context = new ExpressionContext(MyValidExpressionsOwner);
            //context.Imports.Add(new Import(typeof(Math)));
            //context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.NonPublic;

            //context.Variables.Add("s1", "string");

            //var e = this.CreateDynamicExpression("s1.length + stringa.length", context);
            //string[] referencedVariables = e.Info.GetReferencedVariables();

            //Assert.AreEqual(2, referencedVariables.Length);
            //Assert.AreNotEqual(-1, System.Array.IndexOf<string>(referencedVariables, "s1"));
            //Assert.AreNotEqual(-1, System.Array.IndexOf<string>(referencedVariables, "stringa"));
        }

        [Test(Description = "Test that we can handle long logical expressions and that we properly adjust for long branches")]
        public void TestLongBranchLogical1()
        {
            string expressionText = this.GetIndividualTest("LongBranch1");
            ExpressionContext context = new ExpressionContext();

            VariableCollection vc = context.Variables;

            vc.Add("M0100_ASSMT_REASON", "0");
            vc.Add("M0220_PRIOR_NOCHG_14D", "1");
            vc.Add("M0220_PRIOR_UNKNOWN", "1");
            vc.Add("M0220_PRIOR_UR_INCON", "1");
            vc.Add("M0220_PRIOR_CATH", "1");
            vc.Add("M0220_PRIOR_INTRACT_PAIN", "1");
            vc.Add("M0220_PRIOR_IMPR_DECSN", "1");
            vc.Add("M0220_PRIOR_DISRUPTIVE", "1");
            vc.Add("M0220_PRIOR_MEM_LOSS", "1");
            vc.Add("M0220_PRIOR_NONE", "1");

            vc.Add("M0220_PRIOR_UR_INCON_bool", true);
            vc.Add("M0220_PRIOR_CATH_bool", true);
            vc.Add("M0220_PRIOR_INTRACT_PAIN_bool", true);
            vc.Add("M0220_PRIOR_IMPR_DECSN_bool", true);
            vc.Add("M0220_PRIOR_DISRUPTIVE_bool", true);
            vc.Add("M0220_PRIOR_MEM_LOSS_bool", true);
            vc.Add("M0220_PRIOR_NONE_bool", true);
            vc.Add("M0220_PRIOR_NOCHG_14D_bool", true);
            vc.Add("M0220_PRIOR_UNKNOWN_bool", true);

            var e = this.CreateDynamicExpression(expressionText, context);
            // We only care that the expression is valid and can be evaluated
            object result = e.Invoke();
        }

        [Test(Description = "Test that we can handle long logical expressions and that we properly adjust for long branches")]
        public void TestLongBranchLogical2()
        {
            string expressionText = this.GetIndividualTest("LongBranch2");
            ExpressionContext context = new ExpressionContext();

            GenericExpression<bool> e = this.CreateGenericExpression<bool>(expressionText, context);
            Assert.IsFalse(e.Invoke());
        }

        [Test(Description = "Test that we can work with base and derived owner classes")]
        [Ignore("Does not apply")]
        public void TestDerivedOwner()
        {
            //MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
            //MethodInfo mi = (MethodInfo)mb;

            //ExpressionContext context = new ExpressionContext(null, mi);

            //// Call a property on the base class
            //GenericExpression<bool> e = this.CreateGenericExpression<bool>("IsPublic", context);

            //Assert.AreEqual(mi.IsPublic, e.Invoke());

            //context = new ExpressionContext(null, mb);
            //// Test that setting the owner to a derived class works
            //e = this.CreateGenericExpression<bool>("IsPublic", context);
            //Assert.AreEqual(mb.IsPublic, e.Invoke());

            //// Pick a non-public method and set it as the new owner
            //mi = typeof(Math).GetMethod("InternalTruncate", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            //e.Owner = mi;

            //Assert.AreEqual(mi.IsPublic, e.Invoke());
        }

        [Test(Description = "Test we can handle an expression owner that is a value type")]
        public void TestValueTypeOwner()
        {
            TestStruct owner = new TestStruct(100);
            ExpressionContext context = this.CreateGenericContext(owner);

            GenericExpression<int> e = this.CreateGenericExpression<int>("myA", context);
            int result = e.Invoke();
            Assert.AreEqual(100, result);

            e = this.CreateGenericExpression<int>("mya.compareto(100)", context);
            result = e.Invoke();
            Assert.AreEqual(0, result);

            e = this.CreateGenericExpression<int>("DoStuff()", context);
            result = e.Invoke();
            Assert.AreEqual(100, result);

            DateTime dt = DateTime.Now;
            context = this.CreateGenericContext(dt);

            e = this.CreateGenericExpression<int>("Month", context);
            result = e.Invoke();
            Assert.AreEqual(dt.Month, result);

            GenericExpression<string> e2 = this.CreateGenericExpression<string>("tolongdatestring()", context);
            Assert.AreEqual(dt.ToLongDateString(), e2.Invoke());
        }

        [Test(Description = "We should be able to import non-public types if they are in the same module as our owner")]
        public void TestNonPublicImports()
        {
            ExpressionContext context = new ExpressionContext();

            try
            {
                // Should not be able to import non-public type
                // make sure type is not public
                Assert.IsFalse(typeof(TestImport).IsPublic);
                context.Imports.Add(new Import(typeof(TestImport)));
                Assert.Fail();

            }
            catch (ArgumentException)
            {
            }

            // ...until we set an owner that is in the same module
            context = new ExpressionContext(null, new OverloadTestExpressionOwner());
            context.Imports.Add(new Import(typeof(TestImport)));

            var e = this.CreateDynamicExpression("DoStuff()", context);
            Assert.AreEqual(100, (int)e.Invoke());
        }

        [Test(Description = "We should be able to import non-public types if they are in the same module as our owner")]
        [Ignore("Importing methods is not implemented")]
        public void TestNonPublicMethodImports()
        {
            // Importing methods is not implemented.
            //// Try the same test with an invidual method
            //context = new ExpressionContext();

            //try
            //{
            //    context.Imports.AddMethod("Dostuff", typeof(TestImport), "");
            //    Assert.Fail();

            //}
            //catch (ArgumentException ex)
            //{
            //}

            //context = new ExpressionContext(new OverloadTestExpressionOwner());
            //context.Imports.AddMethod("Dostuff", typeof(TestImport), "");
            //e = this.CreateDynamicExpression("DoStuff()", context);
            //Assert.AreEqual(100, (int)e.Invoke());
        }

        [Test(Description = "Test import with nested types")]
        public void TestNestedTypeImport()
        {
            ExpressionContext context = new ExpressionContext();

            try
            {
                // Should not be able to import non-public nested type
                context.Imports.Add(new Import(typeof(NestedA.NestedInternalB)));
                Assert.Fail();

            }
            catch (ArgumentException)
            {
            }

            // Should be able to import public nested type
            context.Imports.Add(new Import(typeof(NestedA.NestedPublicB)));
            var e = this.CreateDynamicExpression("DoStuff()", context);
            Assert.AreEqual(100, (int)e.Invoke());

            // Should be able to import nested internal type now
            context = new ExpressionContext(null, new OverloadTestExpressionOwner());
            context.Imports.Add(new Import(typeof(NestedA.NestedInternalB)));
            e = this.CreateDynamicExpression("DoStuff()", context);
            Assert.AreEqual(100, (int)e.Invoke());
        }

        [Test(Description = "We should not allow access to the non-public members of a variable")]
        [ExpectedException(typeof(CompilationException))]
        public void TestNonPublicVariableMemberAccess()
        {
            ExpressionContext context = new ExpressionContext();
            context.Variables.Add("a", "abc");

            this.CreateDynamicExpression("a.m_length", context);
        }

        [Test(Description = "We should not compile an expression that accesses a non-public field with the same name as a variable")]
        public void TestFieldWithSameNameAsVariable()
        {
            ExpressionContext context = new ExpressionContext(null, new Monitor());
            context.Variables.Add("doubleA", new ExpressionOwner());
            this.AssertCompileException("doubleA.doubleA", context);

            // But it should work for a public member
            context = new ExpressionContext();
            Monitor m = new Monitor();
            context.Variables.Add("I", m);

            var e = this.CreateDynamicExpression("i.i", context);
            Assert.AreEqual(m.I, (int)e.Invoke());
        }

        [Test(Description = "Test we can match members with names that differ only by case")]
        [Ignore]
        public void TestMemberCaseSensitivity()
        {
            //FleeTest.CaseSensitiveOwner owner = new FleeTest.CaseSensitiveOwner();
            //ExpressionContext context = new ExpressionContext(owner);
            //context.Options.CaseSensitive = true;
            //context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

            //context.Variables.Add("x", 300);
            //context.Variables.Add("X", 400);

            //var e = this.CreateDynamicExpression("a + A + x + X", context);
            //Assert.AreEqual(100 + 200 + 300 + 400, (int)e.Invoke());

            //// Should fail since the function is called Cos
            //this.AssertCompileException("cos(1)", context, CompileExceptionReason.UndefinedName);
        }

        [Test(Description = "Test handling of on-demand variables")]
        public void TestOnDemandVariables()
        {
            var e = this.CreateDynamicExpression("a + b", new TestExpressionContext());
            Assert.AreEqual(100 + 100, (int)e.Invoke());
        }

        [Test(Description = "Test on-demand functions")]
        [Ignore("On demand methods is not implemented")]
        public void TestOnDemandFunctions()
        {
            //ExpressionContext context = new ExpressionContext();
            //context.Variables.ResolveFunction += OnResolveFunction;
            //context.Variables.InvokeFunction += OnInvokeFunction;

            //var e = this.CreateDynamicExpression("func1(100) * func2(0.25)", context);
            //Assert.AreEqual(100 * 0.25, (double)e.Invoke());
        }

        //private void OnResolveFunction(object sender, ResolveFunctionEventArgs e)
        //{
        //    switch (e.FunctionName)
        //    {
        //        case "func1":
        //            e.ReturnType = typeof(int);
        //            break;
        //        case "func2":
        //            e.ReturnType = typeof(double);
        //            break;
        //    }
        //}

        //private void OnInvokeFunction(object sender, InvokeFunctionEventArgs e)
        //{
        //    e.Result = e.Arguments(0);
        //}

        [Test(Description = "Test that we properly resolve method overloads")]
        public void TestOverloadResolution()
        {
            OverloadTestExpressionOwner owner = new OverloadTestExpressionOwner();
            ExpressionContext context = new ExpressionContext(null, owner);

            // Test value types
            this.DoTestOverloadResolution("valuetype1(100)", context, 1);
            this.DoTestOverloadResolution("valuetype1(100.0f)", context, 2);
            this.DoTestOverloadResolution("valuetype1(100.0)", context, 3);
            this.DoTestOverloadResolution("valuetype2(100)", context, 1);

            // Test value type -> reference type
            this.DoTestOverloadResolution("Value_ReferenceType1(100)", context, 1);
            this.DoTestOverloadResolution("Value_ReferenceType2(100)", context, 1);
            //	with interfaces
            this.DoTestOverloadResolution("Value_ReferenceType3(100)", context, 1);

            // Test reference types
            this.DoTestOverloadResolution("ReferenceType1(\"abc\")", context, 2);
            this.DoTestOverloadResolution("ReferenceType1(b)", context, 1);
            this.DoTestOverloadResolution("ReferenceType2(a)", context, 2);
            //	with interfaces
            this.DoTestOverloadResolution("ReferenceType3(\"abc\")", context, 2);

            // Test nulls
            this.DoTestOverloadResolution("ReferenceType1(null)", context, 2);
            this.DoTestOverloadResolution("ReferenceType2(null)", context, 2);

            // Test ambiguous match
            this.DoTestOverloadResolution("valuetype3(100)", context, -1);
            this.DoTestOverloadResolution("Value_ReferenceType4(100)", context, -1);
            this.DoTestOverloadResolution("ReferenceType4(\"abc\")", context, -1);
            this.DoTestOverloadResolution("ReferenceType4(null)", context, -1);

            // Test access control
            this.DoTestOverloadResolution("Access1(\"abc\")", context, 1);
            this.DoTestOverloadResolution("Access2(\"abc\")", context, -1);
            this.DoTestOverloadResolution("Access2(null)", context, -1);

            // Test with multiple arguments
            this.DoTestOverloadResolution("Multiple1(1.0f, 2.0)", context, 1);
            this.DoTestOverloadResolution("Multiple1(100, 2.0)", context, 2);
            this.DoTestOverloadResolution("Multiple1(100, 2.0f)", context, 2);
        }

        private void DoTestOverloadResolution(string expression, ExpressionContext context, int expectedResult)
        {
            try
            {
                GenericExpression<int> e = this.CreateGenericExpression<int>(expression, context);
                int result = e.Invoke();
                Assert.AreEqual(expectedResult, result);
            }
            catch (Exception)
            {
                Assert.AreEqual(-1, expectedResult);
            }
        }

        [Test(Description = "Test the NumbersAsDoubles option")]
        [Ignore("Integer as doubles not implemented")]
        public void TestNumbersAsDoubles()
        {
            //ExpressionContext context = new ExpressionContext();
            //context.Options.IntegersAsDoubles = true;

            //GenericExpression<double> e = this.CreateGenericExpression<double>("1 / 2", context);
            //Assert.AreEqual(1 / 2.0, e.Invoke());

            //e = this.CreateGenericExpression<double>("4 * 4 / 10", context);
            //Assert.AreEqual(4 * 4 / 10.0, e.Invoke());

            //context.Variables.Add("a", 1);

            //e = this.CreateGenericExpression<double>("a / 10", context);
            //Assert.AreEqual(1 / 10.0, e.Invoke());

            //// Should get a double back
            //var e2 = this.CreateDynamicExpression("100", context);
            //Assert.IsInstanceOfType(typeof(double), e2.Invoke());
        }

        [Test(Description = "Test variables that are expressions")]
        public void TestExpressionVariables()
        {
            ExpressionContext context1 = new ExpressionContext();
            context1.Imports.Add(new Import(typeof(Math)));
            context1.Variables.Add("a", Math.PI);
            var exp1 = this.CreateDynamicExpression("sin(a)", context1);

            ExpressionContext context2 = new ExpressionContext();
            context2.Imports.Add(new Import(typeof(Math)));
            context2.Variables.Add("a", Math.PI);
            GenericExpression<double> exp2 = this.CreateGenericExpression<double>("cos(a/2)", context2);

            ExpressionContext context3 = new ExpressionContext();
            context3.Variables.Add("a", exp1);
            context3.Variables.Add("b", exp2);
            var exp3 = this.CreateDynamicExpression("a + b", context3);

            double a = Math.Sin(Math.PI);
            double b = Math.Cos(Math.PI / 2);

            Assert.AreEqual(a + b, exp3.Invoke());

            ExpressionContext context4 = new ExpressionContext();
            context4.Variables.Add("a", exp1);
            context4.Variables.Add("b", exp2);
            GenericExpression<double> exp4 = this.CreateGenericExpression<double>("(a * b) + (b - a)", context4);

            Assert.AreEqual((a * b) + (b - a), exp4.Invoke());
        }

        [Test(Description = "Test the RealLiteralDataType option")]
        [Ignore("Real literal data type not yet implemented")]
        public void TestRealLiteralDataTypeOption()
        {
            //ExpressionContext context = new ExpressionContext();
            //context.Options.RealLiteralDataType = RealLiteralDataType.Single;

            //var e = this.CreateDynamicExpression("100.25", context);

            //Assert.IsInstanceOfType(typeof(float), e.Invoke());

            //context.Options.RealLiteralDataType = RealLiteralDataType.Decimal;
            //e = this.CreateDynamicExpression("100.25", context);

            //Assert.IsInstanceOfType(typeof(decimal), e.Invoke());

            //context.Options.RealLiteralDataType = RealLiteralDataType.Double;
            //e = this.CreateDynamicExpression("100.25", context);

            //Assert.IsInstanceOfType(typeof(double), e.Invoke());

            //// Override should still work though
            //e = this.CreateDynamicExpression("100.25f", context);
            //Assert.IsInstanceOfType(typeof(float), e.Invoke());

            //e = this.CreateDynamicExpression("100.25d", context);
            //Assert.IsInstanceOfType(typeof(double), e.Invoke());

            //e = this.CreateDynamicExpression("100.25M", context);
            //Assert.IsInstanceOfType(typeof(decimal), e.Invoke());
        }

        [Test(Description = "Test the string quote parser option")]
        [Ignore("String quoting not implemented")]
        public void TestStringQuote()
        {
            //ExpressionContext context = new ExpressionContext();
            //context.ParserOptions.StringQuote = '`';
            //context.ParserOptions.RecreateParser();

            //GenericExpression<string> e = this.CreateGenericExpression<string>("`string`", context);
            //Assert.AreEqual("string", e.Invoke());

            //e = this.CreateGenericExpression<string>("`string` + `_2`", context);
            //Assert.AreEqual("string_2", e.Invoke());

            //// With escape
            //e = this.CreateGenericExpression<string>("`string + \\`quote\\` _2`", context);
            //Assert.AreEqual("string + `quote` _2", e.Invoke());

            //// Should be able to use regular double quote as it's no longer special
            //e = this.CreateGenericExpression<string>("`string + \"quote\" _2`", context);
            //Assert.AreEqual("string + \"quote\" _2", e.Invoke());

            //// Char
            //e = this.CreateGenericExpression<string>("`string` + '1'.ToString()", context);
            //Assert.AreEqual("string1", e.Invoke());
        }

        private GenericExpression<T> CreateGenericExpression<T>(string p)
        {
            return CreateGenericExpression<T>(p, null, null);
        }

        private GenericExpression<T> CreateGenericExpression<T>(string p, ExpressionContext expressionContext)
        {
            return CreateGenericExpression<T>(p, expressionContext, null);
        }

        private GenericExpression<T> CreateGenericExpression<T>(string p, BoundExpressionOptions options)
        {
            return CreateGenericExpression<T>(p, null, options);
        }

        private GenericExpression<T> CreateGenericExpression<T>(string p, ExpressionContext expressionContext, BoundExpressionOptions options)
        {
            return new GenericExpression<T>(p, expressionContext, options, Language);
        }

        private class GenericExpression<T>
        {
            private readonly string _expression;
            private readonly ExpressionContext _expressionContext;
            private readonly BoundExpressionOptions _options;
            private readonly ExpressionLanguage _language;

            public GenericExpression(string expression, ExpressionContext expressionContext, BoundExpressionOptions options, ExpressionLanguage language)
            {
                _expression = expression;
                _expressionContext = expressionContext;
                _options = options ?? new BoundExpressionOptions();
                _language = language;
            }

            public T Invoke()
            {
                var dynamicExpression = new DynamicExpression(_expression, _language);

                _options.ResultType = typeof(T);

                return (T)dynamicExpression.Invoke(_expressionContext, _options);
            }
        }

        private class TestExpressionContext : IExpressionContext
        {
            public Type OwnerType { get { return null; } }

            public IList<Import> Imports { get { return null; } }

            public Type GetVariableType(string variable, bool ignoreCase)
            {
                return typeof(int);
            }

            public object Owner { get { return null; } }

            public object GetVariableValue(string variable, bool ignoreCase)
            {
                return 100;
            }
        }
    }
}
