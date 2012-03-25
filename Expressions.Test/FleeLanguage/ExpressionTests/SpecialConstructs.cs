using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage.ExpressionTests
{
    [TestFixture]
    internal class SpecialConstructs : TestBase
    {
        [Test]
        public void MethodOnZeroConstant()
        {
            Resolve(
                "0.ToString()",
                new MethodCall(
                    new Constant(0),
                    typeof(int).GetMethod("ToString", new Type[0]),
                    new IExpression[0]
                )
            );
        }

        [Test]
        public void MethodOnZerosConstant()
        {
            Resolve(
                "000.ToString()",
                new MethodCall(
                    new Constant(0),
                    typeof(int).GetMethod("ToString", new Type[0]),
                    new IExpression[0]
                )
            );
        }

        [Test]
        public void MethodOnOneConstant()
        {
            Resolve(
                "1.ToString()",
                new MethodCall(
                    new Constant(1),
                    typeof(int).GetMethod("ToString", new Type[0]),
                    new IExpression[0]
                )
            );
        }
    }
}
