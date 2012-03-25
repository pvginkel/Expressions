using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test
{
    [TestFixture]
    public class MethodCalls
    {
        //[Test]
        //public void SimpleExpression()
        //{
        //    var context = new ExpressionContext(
        //        new[] { new Import(typeof(Math)) },
        //        new Owner { Variable = 7 }
        //    );

        //    context.Variables.Add("ContextVariable", 7.3);

        //    var expression = new DynamicExpression("Round(Variable * ContextVariable)", ExpressionLanguage.Flee);

        //    Assert.AreEqual(51.0, expression.Invoke(context));
        //}

        [Test]
        public void ImportedMethod()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import(typeof(Guid)) }),
                "NewGuid()"
            );
        }

        [Test]
        public void ImportedMethodWithNamespace()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import("Guid", typeof(Guid)) }),
                "Guid.NewGuid()"
            );
        }

        [Test]
        public void MethodOnMember()
        {
            CompileExpression(
                new ExpressionContext(null, new Owner { Variable = 7 }),
                "Variable.ToString()"
            );
        }

        [Test]
        public void MethodOnStaticProperty()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import(typeof(DateTime)) }),
                "Now.ToString()"
            );
        }

        [Test]
        public void MethodOnStaticPropertyWithNamespace()
        {
            CompileExpression(
                new ExpressionContext(new[] { new Import("DateTime", typeof(DateTime)) }),
                "DateTime.Now.ToString()"
            );
        }

        private void CompileExpression(ExpressionContext expressionContext, string expression)
        {
            new DynamicExpression(expression, ExpressionLanguage.Flee).Invoke(expressionContext);
        }

        public class Owner
        {
            public int Variable { get; set; }
        }
    }
}
