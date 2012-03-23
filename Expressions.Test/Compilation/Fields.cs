using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.Compilation
{
    [TestFixture]
    internal class Fields : TestBase
    {
        [Test]
        public void StaticFieldOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "StaticField", 8);
        }

        [Test]
        public void FieldOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "Field", 7);
        }

        public class Owner
        {
            public static int StaticField = 8;

            public int Field = 7;
        }
    }
}
