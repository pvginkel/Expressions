using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage.BulkTests
{
    internal class BulkTests : Test.BulkTests
    {
        public BulkTests()
            : base(ExpressionLanguage.Flee)
        {
        }

        [Test(Description = "Expressions that should not be valid")]
        public void TestInvalidExpressions()
        {
            ProcessScriptTests("InvalidExpressions.txt", DoTestInvalidExpressions);
        }

        private void DoTestInvalidExpressions(string[] arr)
        {
            var reason = (ExpressionsExceptionType)Enum.Parse(typeof(ExpressionsExceptionType), arr[2], true);

            var options = new BoundExpressionOptions
            {
                ResultType = Type.GetType(arr[0], true, true),
                AllowPrivateAccess = true
            };

            AssertCompileException(arr[1], MyGenericContext, options, reason);
        }
    }
}
