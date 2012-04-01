using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.FleeLanguage.Compilation
{
    [TestFixture]
    internal class ExpectedExceptions : TestBase
    {
        [Test]
        public void IntAndBoolSubtraction()
        {
            Resolve("1 - True", ExpressionsExceptionType.TypeMismatch);
        }

        [Test]
        public void LongAndUlongEquals()
        {
            Resolve("ulong.minvalue = long.minvalue", ExpressionsExceptionType.TypeMismatch);
        }

        [Test]
        public void StringAndString()
        {
            Resolve("\"abc\" and \"def\"", ExpressionsExceptionType.TypeMismatch);
        }

        [Test]
        public void StringResultTypeForInt()
        {
            Resolve("100", ExpressionsExceptionType.InvalidExplicitCast, new BoundExpressionOptions
            {
                ResultType = typeof(string)
            });
        }

        [Test]
        public void IntResultTypeForString()
        {
            Resolve("\"a\"", ExpressionsExceptionType.InvalidExplicitCast, new BoundExpressionOptions
            {
                ResultType = typeof(int)
            });
        }

        [Test]
        public void StringResultTypeForType()
        {
            var context = new ExpressionContext();

            context.Variables.Add("a", typeof(string));

            Resolve(context, "a", ExpressionsExceptionType.TypeMismatch, new BoundExpressionOptions
            {
                ResultType = typeof(string)
            });
        }

        [Test]
        public void UnresolvedIdentifier()
        {
            Resolve("FakeField + 1", ExpressionsExceptionType.UndefinedName);
        }

        [Test]
        public void ReturnBuiltInType()
        {
            Resolve("String", ExpressionsExceptionType.TypeMismatch);
        }

        [Test]
        public void UnresolvedMethod()
        {
            Resolve("Method()", ExpressionsExceptionType.UndefinedName);
        }

        [Test]
        public void UnresolvedType()
        {
            Resolve("cast(1, UnknownType)", ExpressionsExceptionType.InvalidExplicitCast);
        }

        [Test]
        public void IllegalValueCast()
        {
            Resolve("cast(\"a\", boolean)", ExpressionsExceptionType.InvalidExplicitCast);
        }

        [Test]
        public void IllegalReferenceCast()
        {
            var context = new ExpressionContext(null, new Owner());

            Resolve(context, "cast(ICollectionA, Guid)", ExpressionsExceptionType.InvalidExplicitCast);
        }

        [Test]
        public void SwallowedLexerErrors()
        {
            Resolve("\"\\z\"", ExpressionsExceptionType.SyntaxError);
        }

        [Test]
        public void BoolEqualsNull()
        {
            Resolve("true = null", ExpressionsExceptionType.TypeMismatch);
        }

        [Test]
        public void IntToString()
        {
            var context = new ExpressionContext();

            context.Variables.Add("stringA", "Hi");

            Resolve(context, "stringA.length", ExpressionsExceptionType.InvalidExplicitCast, new BoundExpressionOptions
            {
                ResultType = typeof(string)
            });
        }

        private void Resolve(string expression, ExpressionsExceptionType exceptionType)
        {
            Resolve(expression, exceptionType, null);
        }

        private void Resolve(string expression, ExpressionsExceptionType exceptionType, BoundExpressionOptions options)
        {
            Resolve(null, expression, exceptionType, options);
        }

        private void Resolve(ExpressionContext context, string expression, ExpressionsExceptionType exceptionType)
        {
            Resolve(context, expression, exceptionType, null);
        }

        private void Resolve(ExpressionContext context, string expression, ExpressionsExceptionType exceptionType, BoundExpressionOptions options)
        {
            try
            {
                Resolve(context, expression, null, options);
                Assert.Fail("Expected exception type '{0}'", exceptionType);
            }
            catch (ExpressionsException ex)
            {
                Assert.AreEqual(exceptionType, ex.Type, String.Format("Expected exception type '{0}' got '{1}'", exceptionType, ex.Type));
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected ExpressionsException got '{0}'", ex.GetType());
            }
        }

        public class Owner
        {
            public ICollection ICollectionA { get; set; }

            public Owner()
            {
                ICollectionA = new ArrayList();
            }
        }
    }
}
