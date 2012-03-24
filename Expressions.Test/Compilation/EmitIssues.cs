using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.Compilation
{
    [TestFixture]
    internal class EmitIssues : TestBase
    {
        [Test]
        public void Issue1()
        {
            Resolve("true or (1.0+2.0+3.0+4.0+5.0+6.0+7.0+8.0+9.0+10.0+11.0+12.0+13.0+14.0+15.0 > 10.0)", true);
        }
    }
}
