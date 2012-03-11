using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Expressions.ResolvedAst;
using NUnit.Framework;

namespace Expressions.Test.Resolving
{
    [TestFixture]
    public class MethodCalls
    {
        private static readonly IResolvedAstNode[] EmptyArguments = new IResolvedAstNode[0];

        [Test]
        public void ImportedMethod()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(Guid)) }),
                "NewGuid()",
                new ResolvedMethodCall(
                    new ResolvedIdentifierAccess(
                        new MethodIdentifier(
                            new TypeIdentifier(typeof(Guid)),
                            typeof(Guid).GetMethod("NewGuid")
                        )
                    ),
                    new MethodIdentifier(
                        new TypeIdentifier(typeof(Guid)),
                        typeof(Guid).GetMethod("NewGuid")
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void ImportedMethodWithNamespace()
        {
            var import = new Import(typeof(Guid), "Guid");

            Resolve(
                new ExpressionContext(new[] { import }),
                "Guid.NewGuid()",
                new ResolvedMethodCall(
                    new ResolvedMemberAccess(
                        new ResolvedIdentifierAccess(
                            new ImportIdentifier(import)
                        ),
                        new MethodIdentifier(
                            new ImportIdentifier(import),
                            typeof(Guid).GetMethod("NewGuid")
                        )
                    ),
                    new MethodIdentifier(
                        new ImportIdentifier(import),
                        typeof(Guid).GetMethod("NewGuid")
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void MethodOnMember()
        {
            Resolve(
                new ExpressionContext(null, new Owner { Variable = 7 }),
                "Variable.ToString()",
                new ResolvedMethodCall(
                    new ResolvedMemberAccess(
                        new ResolvedIdentifierAccess(
                            new PropertyIdentifier(
                                new VariableIdentifier(typeof(Owner), 0),
                                typeof(Owner).GetMethod("get_Variable")
                            )
                        ),
                        new MethodGroupIdentifier(
                            new PropertyIdentifier(
                                new VariableIdentifier(typeof(Owner), 0),
                                typeof(Owner).GetMethod("get_Variable")
                            ),
                            TypeExtensions.GetMethods(typeof(int), "ToString", BindingFlags.Instance | BindingFlags.Public)
                        )
                    ),
                    new MethodIdentifier(
                        new PropertyIdentifier(
                            new VariableIdentifier(typeof(Owner), 0),
                            typeof(Owner).GetMethod("get_Variable")
                        ),
                        typeof(int).GetMethod("ToString", new Type[0])
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void MethodOnStaticProperty()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(DateTime)) }),
                "Now.ToString()",
                new ResolvedMethodCall(
                    new ResolvedMemberAccess(
                        new ResolvedIdentifierAccess(
                            new PropertyIdentifier(
                                new TypeIdentifier(typeof(DateTime)),
                                typeof(DateTime).GetMethod("get_Now")
                            )
                        ),
                        new MethodGroupIdentifier(
                            new PropertyIdentifier(
                                new TypeIdentifier(typeof(DateTime)),
                                typeof(DateTime).GetMethod("get_Now")
                            ),
                            TypeExtensions.GetMethods(typeof(DateTime), "ToString", BindingFlags.Instance | BindingFlags.Public)
                        )
                    ),
                    new MethodIdentifier(
                        new PropertyIdentifier(
                            new TypeIdentifier(typeof(DateTime)),
                            typeof(DateTime).GetMethod("get_Now")
                        ),
                        typeof(DateTime).GetMethod("ToString", new Type[0])
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void MethodOnStaticPropertyWithNamespace()
        {
            var import = new Import(typeof(DateTime), "DateTime");

            Resolve(
                new ExpressionContext(new[] { import }),
                "DateTime.Now.ToString()",
                new ResolvedMethodCall(
                    new ResolvedMemberAccess(
                        new ResolvedMemberAccess(
                            new ResolvedIdentifierAccess(
                                new ImportIdentifier(import)
                            ),
                            new PropertyIdentifier(
                                new ImportIdentifier(import),
                                typeof(DateTime).GetMethod("get_Now")
                            )
                        ),
                        new MethodGroupIdentifier(
                            new PropertyIdentifier(
                                new ImportIdentifier(import),
                                typeof(DateTime).GetMethod("get_Now")
                            ),
                            TypeExtensions.GetMethods(typeof(DateTime), "ToString", BindingFlags.Instance | BindingFlags.Public)
                        )
                    ),
                    new MethodIdentifier(
                        new PropertyIdentifier(
                            new ImportIdentifier(import),
                            typeof(DateTime).GetMethod("get_Now")
                        ),
                        typeof(DateTime).GetMethod("ToString", new Type[0])
                    ),
                    EmptyArguments
                )
            );
        }

        private void Resolve(ExpressionContext expressionContext, string expression, IResolvedAstNode expectedResult)
        {
            var boundExpression = new DynamicExpression(expression, ExpressionLanguage.Flee).Bind(expressionContext);

            if (!Equals(expectedResult, boundExpression.ResolvedTree))
            {
                string expected = new ResolvedNodePrinter(expectedResult).ToString();
                string actual = new ResolvedNodePrinter(boundExpression.ResolvedTree).ToString();

                Console.WriteLine(PrintSideToSide("Expected:\r\n" + expected, "Actual:\r\n" + actual));

                Assert.AreEqual(expectedResult, boundExpression.ResolvedTree);
            }
        }

        private string PrintSideToSide(string left, string right)
        {
            var sb = new StringBuilder();

            var leftLines = GetLines(left);
            var rightLines = GetLines(right);

            int longestLeft = int.MinValue;

            foreach (string line in leftLines)
            {
                longestLeft = Math.Max(longestLeft, line.TrimEnd().Length);
            }

            for (int i = 0; i < Math.Max(leftLines.Length, rightLines.Length); i++)
            {
                string leftLine = i >= leftLines.Length ? "" : leftLines[i];
                string rightLine = i >= rightLines.Length ? "" : rightLines[i];

                string separator = leftLine != rightLine ? " | " : "   ";

                sb.Append(leftLine);
                sb.Append(new string(' ', longestLeft - leftLine.Length));
                sb.Append(separator);
                sb.AppendLine(rightLine);
            }

            return sb.ToString();
        }

        private string[] GetLines(string text)
        {
            var lines = new List<string>();

            foreach (string line in text.Split('\n'))
            {
                lines.Add(line.TrimEnd());
            }

            return lines.ToArray();
        }

        public class Owner
        {
            public int Variable { get; set; }
        }
    }
}
