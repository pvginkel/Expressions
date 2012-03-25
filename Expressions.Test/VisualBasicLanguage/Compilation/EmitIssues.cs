using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Test.VisualBasicLanguage.BulkTests;
using NUnit.Framework;

namespace Expressions.Test.VisualBasicLanguage.Compilation
{
    [TestFixture]
    internal class EmitIssues : TestBase
    {
        [Test]
        public void LongBranch()
        {
            Resolve("true or (1.0+2.0+3.0+4.0+5.0+6.0+7.0+8.0+9.0+10.0+11.0+12.0+13.0+14.0+15.0 > 10.0)", true);
        }

        [Test]
        public void SpecialLoadOfStructFields()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("Mouse", typeof(Mouse)) }),
                "Mouse.shareddt.year",
                1
            );
        }

        [Test]
        public void ReadOnlyStaticAccess()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("DateTime", typeof(DateTime)) }),
                "DateTime.MinValue.Year",
                DateTime.MinValue.Year
            );
        }

        [Test]
        public void ReadOnlyInstanceAccess()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MinValue.Year",
                DateTime.MinValue.Year
            );
        }

        [Test]
        public void ReadOnlyFieldAsReturn()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("string", typeof(string)) }),
                "string.empty",
                String.Empty
            );
        }

        [Test]
        public void Issue1()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("Mouse", typeof(Mouse)) }, new ExpressionOwner()),
                "DateTimeA.GetType().Name",
                "DateTime",
                new BoundExpressionOptions
                {
                    AllowPrivateAccess = true
                }
            );
        }

        [Test]
        public void Issue2()
        {
            Resolve(
                "(100 < 0) or (not (\"a\" = \"a\"))",
                false
            );
        }

        public class Owner
        {
            public readonly DateTime MinValue;

            public Owner()
            {
                MinValue = DateTime.MinValue;
            }
        }
    }
}
