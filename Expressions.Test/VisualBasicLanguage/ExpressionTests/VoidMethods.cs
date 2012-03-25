using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.VisualBasicLanguage.ExpressionTests
{
    [TestFixture]
    internal class VoidMethods : TestBase
    {
        [Test]
        [ExpectedException]
        public void VoidMethodsAreInvisible()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "VoidMethod()"
            );
        }

        public class Owner
        {
            public void VoidMethod()
            {
            }
        }
    }
}
