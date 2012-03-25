using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.Compilation
{
    [TestFixture]
    internal class MethodCalls : TestBase
    {
        [Test]
        public void StaticsOnImport()
        {
            var context = new ExpressionContext(new[] { new Import(typeof(Math)) });

            Resolve(context, "Max(1, 2)", 2);
            Resolve(context, "Max(1.1, 2)", 2.0);
        }

        [Test]
        public void StaticsOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "StaticMethod(1)", 1);
        }

        [Test]
        public void MethodOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "Method()", 7);
        }

        [Test]
        public void PropertyOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "IntProperty", 7);
        }

        [Test]
        public void NestedPropertyOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "Item.IntProperty", 7);
        }

        [Test]
        public void ParamsWithoutArgs()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.Params(0)", 0);
        }

        [Test]
        public void ParamsWithSingleArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.Params(0, \"a\")", 2);
        }

        [Test]
        public void ParamsWithMultipleArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.Params(0, \"a\", \"a\")", 4);
        }

        [Test]
        public void ParamsWithNullArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.Params(0, null)", -1);
        }

        [Test]
        public void ParamsWithMultipleNullArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.Params(0, null, null)", 2);
        }

        [Test]
        public void ParamsWithNullAndActualArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.Params(0, null, \"a\", null)", 4);
        }

        [Test]
        public void ParamsWithMatchingArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            context.Variables.Add(new Variable("Variable") { Value = new[] { "hi" } });

            Resolve(context, "Owner.Params(0, Variable)", 2);
        }

        [Test]
        public void ObjectParamsWithoutArgs()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.ObjectParams(0)", 0);
        }

        [Test]
        public void ObjectParamsWithSingleArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.ObjectParams(0, \"a\")", 2);
        }

        [Test]
        public void ObjectParamsWithMultipleArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.ObjectParams(0, \"a\", \"a\")", 4);
        }

        [Test]
        public void ObjectParamsWithNullArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.ObjectParams(0, null)", -1);
        }

        [Test]
        public void ObjectParamsWithMultipleNullArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.ObjectParams(0, null, null)", 2);
        }

        [Test]
        public void ObjectParamsWithNullAndActualArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            Resolve(context, "Owner.ObjectParams(0, null, \"a\", null)", 4);
        }

        [Test]
        public void ObjectParamsWithMatchingArg()
        {
            var context = new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) });

            context.Variables.Add(new Variable("Variable") { Value = new[] { "hi" } });

            Resolve(context, "Owner.ObjectParams(0, Variable)", 2);
        }

        [Test]
        public void ImportedMethod()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import(typeof(Guid)) }),
                "NewGuid()"
            );
        }

        [Test]
        public void ImportedMethodWithNamespace()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import("Guid", typeof(Guid)) }),
                "Guid.NewGuid()"
            );
        }

        [Test]
        public void MethodOnMember()
        {
            CompileExpression(
                new ExpressionContext(null, new Owner { Variable = 7 }),
                "Variable.ToString()"
            );
        }

        [Test]
        public void MethodOnStaticProperty()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import(typeof(DateTime)) }),
                "Now.ToString()"
            );
        }

        [Test]
        public void MethodOnStaticPropertyWithNamespace()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import("DateTime", typeof(DateTime)) }),
                "DateTime.Now.ToString()"
            );
        }

        private void CompileExpression(ExpressionContext expressionContext, string expression)
        {
            new DynamicExpression(expression, ExpressionLanguage.Flee).Invoke(expressionContext);
        }

        public class Owner
        {
            public int Variable { get; set; }

            public static int StaticMethod(int value)
            {
                return value;
            }

            public int Method()
            {
                return 7;
            }

            public int IntProperty
            {
                get { return 7; }
            }

            public OwnerItem Item
            {
                get { return new OwnerItem(); }
            }

            public static int Params(int value, params string[] args)
            {
                if (args == null)
                    return value - 1;

                value += args.Length;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] != null)
                        value++;
                }

                return value;
            }


            public static int ObjectParams(int value, params object[] args)
            {
                if (args == null)
                    return value - 1;

                value += args.Length;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] != null)
                        value++;
                }

                return value;
            }
        }

        public class OwnerItem
        {
            public int IntProperty
            {
                get { return 7; }
            }
        }
    }
}
