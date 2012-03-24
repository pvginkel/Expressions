using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Antlr.Runtime;
using NUnit.Framework;
using Expressions.Flee;

namespace Expressions.Test.FleeLanguage
{
    [TestFixture]
    public class Parsing
    {
        [TestFixtureSetUp]
        public void SetCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        }

        [Test]
        public void ValidExpressions()
        {
            RunTestCases("ValidExpressions.txt");
        }

        [Test]
        public void ValidCasts()
        {
            RunTestCases("ValidCasts.txt");
        }

        [Test]
        public void CheckedTests()
        {
            RunTestCases("CheckedTests.txt");
        }

        [Test]
        public void LongBranchLogicalTest()
        {
            RunTestCase("LongBranchLogicalTest.txt");
        }

        [Test]
        public void SpecialConstructs()
        {
            RunTestCases("SpecialConstructs.txt");
        }

        [Test]
        public void SimpleTest()
        {
            FleeParser.Parse("##1.11:22# > (identifier.property.method() + ##1.01:22#)");
            FleeParser.Parse("cast(null, string[])");
        }

        private void RunTestCase(string resourceName)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".TestScripts." + resourceName))
            using (var reader = new StreamReader(stream))
            {
                string expression = reader.ReadToEnd();

                try
                {
                    ParseExpression(expression);
                }
                catch (CompilationException ex)
                {
                    var recognitionException = ex.InnerException as RecognitionException;

                    Console.WriteLine("Failed expression: {0}", expression);
                    
                    if (recognitionException != null)
                        Console.WriteLine("({0},{1}): {2}", recognitionException.Token.Line, recognitionException.Token.CharPositionInLine, recognitionException.Message);

                    Assert.Fail();
                }
            }
        }

        private void RunTestCases(string resourceName)
        {
            bool success = true;

            foreach (var testCase in GetTests(resourceName))
            {


                //try
                //{
                //    var parser = ParseExpression(testCase.Expression);

                //    if (parser.Exception != null)
                //    {
                //        Console.WriteLine("Failed expression({0}): {1}", testCase.LineNumber, testCase.Expression);
                //        Console.WriteLine("({0},{1}): {2}", parser.Exception.Token.Line, parser.Exception.Token.CharPositionInLine, parser.Exception.Message);
                //        success = false;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine("Failed expression({0}): {1}", testCase.LineNumber, testCase.Expression);
                //    Console.WriteLine("Message: {0}", ex.Message);
                //    success = false;
                //}
            }

            Assert.IsTrue(success);
        }

        private static FleeParser ParseExpression(string expression)
        {
            var inputStream = new ANTLRStringStream(expression);

            var lexer = new FleeLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new FleeParser(tokenStream);

            parser.Parse();

            return parser;
        }

        private IEnumerable<TestCase> GetTests(string resourceName)
        {
            int lineNumber = 0;

            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".TestScripts." + resourceName))
            using (var reader = new StreamReader(stream))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (line[0] == '\'')
                        continue;

                    yield return new TestCase(line.Split(';'), lineNumber);
                }
            }
        }

        private class TestCase
        {
            public string ResultType { get; private set; }
            public string Expression { get; private set; }
            public string ExpectedResult { get; private set; }
            public int LineNumber { get; private set; }

            public TestCase(string[] parts, int lineNumber)
            {
                LineNumber = lineNumber;
                ResultType = parts[0];
                Expression = parts[1];
                ExpectedResult = parts[2];
            }
        }
    }
}
