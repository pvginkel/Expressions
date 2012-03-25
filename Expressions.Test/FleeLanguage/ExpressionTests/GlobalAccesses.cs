using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    [TestFixture]
    internal class GlobalAccesses : TestBase
    {
        [Test]
        public void HexConstant()
        {
            Resolve("0xDeaDU", new Constant(57005u));
        }

        [Test]
        public void AccessGlobalVariable()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "IntProperty",
                new MethodCall(
                    new VariableAccess(typeof(int), 0),
                    typeof(Owner).GetMethod("get_IntProperty"),
                    null
                )
            );
        }

        [Test]
        public void AccessGlobalField()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "IntField",
                new FieldAccess(
                    new VariableAccess(typeof(int), 0),
                    typeof(Owner).GetField("IntField")
                )
            );
        }

        [Test]
        public void MemberOnField()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "Item.Value",
                new MethodCall(
                    new FieldAccess(
                        new VariableAccess(typeof(Owner), 0),
                        typeof(Owner).GetField("Item")
                    ),
                    typeof(OwnerItem).GetMethod("get_Value"),
                    null
                )
            );
        }

        [Test]
        public void StaticField()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "StaticIntField",
                new FieldAccess(
                    new TypeAccess(typeof(Owner)),
                    typeof(Owner).GetField("StaticIntField")
                )
            );
        }

        [Test]
        public void StaticProperty()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "StaticIntProperty",
                new MethodCall(
                    new TypeAccess(typeof(Owner)),
                    typeof(Owner).GetMethod("get_StaticIntProperty"),
                    null
                )
            );
        }

        [Test]
        public void StaticMethod()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "StaticMethod()",
                new MethodCall(
                    new TypeAccess(typeof(Owner)),
                    typeof(Owner).GetMethod("StaticMethod"),
                    null
                )
            );
        }

        [Test]
        public void Variable()
        {
            var context = new ExpressionContext();

            context.Variables.Add(new global::Expressions.Variable("Variable") { Value = 1 });

            Resolve(
                context,
                "Variable",
                new VariableAccess(typeof(int), 0)
            );
        }

        [Test]
        public void MethodOnImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(Owner)) }),
                "StaticMethod()",
                new MethodCall(
                    new TypeAccess(typeof(Owner)),
                    typeof(Owner).GetMethod("StaticMethod"),
                    null
                )
            );
        }

        [Test]
        public void MethodOnImportWithNamespace()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("Owner", typeof(Owner)) }),
                "Owner.StaticMethod()",
                new MethodCall(
                    new TypeAccess(typeof(Owner)),
                    typeof(Owner).GetMethod("StaticMethod"),
                    null
                )
            );
        }

        [Test]
        [ExpectedException]
        public void CannotCallInstanceOnImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(Owner)) }),
                "IntProperty"
            );
        }

        public class Owner
        {
            public int IntProperty { get; set; }

            public int IntField = 0;

            public int[] IntArrayProperty { get; set; }

            public int[] IntArrayField = null;

            public OwnerItem Item = null;

            public static int StaticIntField = 0;

            public static int StaticIntProperty { get; set; }

            public static int StaticMethod()
            {
                throw new NotImplementedException();
            }
        }

        public class OwnerItem
        {
            public int Value { get; set; }
        }
    }
}
