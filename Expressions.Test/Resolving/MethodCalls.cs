using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Expressions.ResolvedAst;
using NUnit.Framework;

namespace Expressions.Test.Resolving
{
    [TestFixture]
    internal class MethodCalls : ResolvingTestBase
    {
        private static readonly IResolvedAstNode[] EmptyArguments = new IResolvedAstNode[0];

        [Test]
        public void ImportedMethod()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(Guid)) }),
                "NewGuid()",
                new ResolvedMethodCall(
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

        public class Owner
        {
            public int Variable { get; set; }
        }
    }
}
