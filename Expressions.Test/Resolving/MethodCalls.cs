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

        [Test]
        public void MethodOnVariable()
        {
            var context = new ExpressionContext();

            context.Variables.Add("Variable", 7);

            Resolve(
                context,
                "Variable.ToString()",
                new ResolvedMethodCall(
                    new MethodIdentifier(
                        new VariableIdentifier(typeof(int), 0),
                        typeof(int).GetMethod("ToString", new Type[0])
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void StaticMethodOfOwner()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "StaticMethod()",
                new ResolvedMethodCall(
                    new MethodIdentifier(
                        new TypeIdentifier(typeof(Owner)),
                        typeof(Owner).GetMethod("StaticMethod")
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void MethodOfOwner()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "Method()",
                new ResolvedMethodCall(
                    new MethodIdentifier(
                        new VariableIdentifier(typeof(Owner), 0),
                        typeof(Owner).GetMethod("Method")
                    ),
                    EmptyArguments
                )
            );
        }

        [Test]
        public void MethodGroupExactParameter()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MethodGroup(7)",
                new ResolvedMethodCall(
                    new MethodIdentifier(
                        new VariableIdentifier(typeof(Owner), 0),
                        typeof(Owner).GetMethod("MethodGroup", new[] { typeof(int) })
                    ),
                    new IResolvedAstNode[]
                    {
                        new ResolvedConstant(7)
                    }
                )
            );
        }

        [Test]
        public void MethodGroupImplicitelyCastedParameter()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MethodGroup(7l)",
                new ResolvedMethodCall(
                    new MethodIdentifier(
                        new VariableIdentifier(typeof(Owner), 0),
                        typeof(Owner).GetMethod("MethodGroup", new[] { typeof(float) })
                    ),
                    new IResolvedAstNode[]
                    {
                        new ResolvedConstant(7L)
                    }
                )
            );
        }

        [Test]
        [ExpectedException]
        public void MethodGroupWithoutOption()
        {
            Resolve(
                new ExpressionContext(null, new Owner()),
                "MethodGroup(7d)"
            );
        }

        [Test]
        public void MethodOnConstant()
        {
            Resolve(
                "1.ToString()",
                new ResolvedMethodCall(
                    new MethodIdentifier(
                        new ConstantIdentifier(1),
                        typeof(int).GetMethod("ToString", new Type[0])
                    ),
                    EmptyArguments
                )
            );
        }

        public class Owner
        {
            public int Variable { get; set; }

            public static int StaticMethod()
            {
                throw new NotImplementedException();
            }

            public int Method()
            {
                throw new NotImplementedException();
            }

            public int MethodGroup()
            {
                throw new NotImplementedException();
            }

            public int MethodGroup(int value)
            {
                throw new NotImplementedException();
            }

            public int MethodGroup(uint value)
            {
                throw new NotImplementedException();
            }

            public int MethodGroup(float value)
            {
                throw new NotImplementedException();
            }
        }
    }
}
