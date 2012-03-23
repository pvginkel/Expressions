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

        public class Owner
        {
            public int IntProperty { get; set; }

            public int IntField;

            public int[] IntArrayProperty { get; set; }

            public int[] IntArrayField;
        }
    }
}
