using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.VisualBasicLanguage.Compilation
{
    [TestFixture]
    internal class Indexing : TestBase
    {
        [Test]
        public void ArrayIndexOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "ArrayProperty(0)", 1);
        }

        [Test]
        public void RankedArrayIndexOnOwner()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "RankedProperty(0,0)", 1);
            Resolve(context, "RankedProperty(1,1)", 4);
        }

        public class Owner
        {
            public int[] ArrayProperty
            {
                get { return new[] { 1, 2, 3 }; }
            }

            public int[,] RankedProperty
            {
                get
                {
                    return new[,]
                    {
                        { 1, 2 },
                        { 3, 4 }
                    };
                }
            }
        }
    }
}
