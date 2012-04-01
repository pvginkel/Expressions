using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage.Compilation
{
    [TestFixture]
    internal class CompilationFailures : TestBase
    {
        [Test]
        [ExpectedException]
        public void InfiniteLoop()
        {
            Resolve("true && false");
        }
    }
}
