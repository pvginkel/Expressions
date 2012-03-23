using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.ExpressionTests
{
    [TestFixture]
    internal class MethodCalls : TestBase
    {
        [Test]
        public void SimpleMethodCall()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "IntMethod(0)",
                new MethodCall(
                    new VariableAccess(typeof(int), 0),
                    typeof(Owner).GetMethod("IntMethod"),
                    new IExpression[]
                    {
                        new Constant(0)
                    }
                )
            );
        }

        [Test]
        public void MethodWithLegalOverload()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MethodWithOverload(0)",
                new MethodCall(
                    new VariableAccess(typeof(int), 0),
                    typeof(Owner).GetMethod("MethodWithOverload", new[] { typeof(int) }),
                    new IExpression[]
                    {
                        new Constant(0)
                    }
                )
            );
        }

        [Test]
        [ExpectedException]
        public void MethodWithIllegalOverload()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MethodWithOverload(1.7)"
            );
        }

        [Test]
        public void MethodOnSubItem()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "Item.Method()",
                new MethodCall(
                    new MethodCall(
                        new VariableAccess(typeof(int), 0),
                        typeof(Owner).GetMethod("get_Item"),
                        null
                    ),
                    typeof(OwnerItem).GetMethod("Method"),
                    null
                )
            );
        }

        public class Owner
        {
            public OwnerItem Item { get { return new OwnerItem(); } }

            public int IntMethod(int value)
            {
                throw new NotSupportedException();
            }

            public int MethodWithOverload(int value)
            {
                throw new NotSupportedException();
            }

            public int MethodWithOverload(string value)
            {
                throw new NotSupportedException();
            }
        }

        public class OwnerItem
        {
            public int Method()
            {
                throw new NotSupportedException();
            }
        }
    }
}
