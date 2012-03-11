using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;
using NUnit.Framework;

namespace Expressions.Test.Resolving
{
    [TestFixture]
    internal class Casting : ResolvingTestBase
    {
        [Test]
        public void CastWithBuiltInType()
        {
            Resolve(
                "cast(7, float)",
                new ResolvedCast(
                    new ResolvedConstant(7),
                    typeof(float)
                )
            );
        }
    }
}
