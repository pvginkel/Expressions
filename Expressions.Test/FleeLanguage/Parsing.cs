using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr.Runtime;
using NUnit.Framework;
using Expressions.Flee;

namespace Expressions.Test.FleeLanguage
{
    [TestFixture]
    public class Parsing
    {
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

        private void RunTestCase(string resourceName)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + ".TestScripts." + resourceName))
            using (var reader = new StreamReader(stream))
            {
                string expression = reader.ReadToEnd();

                var parser = ParseExpression(expression);

                if (!parser.Success)
                {
                    Console.WriteLine("Failed expression: {0}", expression);
                    Assert.Fail();
                }
            }
        }

        private void RunTestCases(string resourceName)
        {
            bool success = true;

            foreach (var testCase in GetTests(resourceName))
            {
                var parser = ParseExpression(testCase.Expression);

                if (!parser.Success)
                {
                    Console.WriteLine("Failed expression({0}): {1}", testCase.LineNumber, testCase.Expression);
                    success = false;
                }
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
