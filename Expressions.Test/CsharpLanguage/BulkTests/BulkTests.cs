// From http://flee.codeplex.com/

using System.Text;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.BulkTests
{
    internal class BulkTests : Test.BulkTests
    {
        public BulkTests()
            : base(ExpressionLanguage.Csharp)
        {
        }
    }
}
