using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.ExpressionTests
{
    [TestFixture]
    internal class GlobalAccesses : TestBase
    {
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

        public class Owner
        {
            public int IntProperty { get; set; }

            public int IntField;

            public int[] IntArrayProperty { get; set; }

            public int[] IntArrayField;

            public OwnerItem Item;

            public static int StaticIntField;

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
