using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;
using Expressions.ResolvedAst;
using NUnit.Framework;

namespace Expressions.Test.Resolving
{
    [TestFixture]
    internal class BinaryExpressions : ResolvingTestBase
    {
        [Test]
        public void BinaryTypeUnchanged()
        {
            Resolve(
                "1 + 2",
                new ResolvedBinaryExpression(
                    new ResolvedConstant(1),
                    new ResolvedConstant(2),
                    typeof(int),
                    ExpressionType.Add
                )
            );
        }
    }
}
