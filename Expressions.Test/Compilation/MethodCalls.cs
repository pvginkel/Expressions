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

        public class Owner
        {
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
