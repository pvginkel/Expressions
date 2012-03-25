using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Expressions.Test.CsharpLanguage.Compilation
{
    [TestFixture]
    internal class Imports : TestBase
    {
        [Test]
        public void RootImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import(typeof(Math)) }),
                "Abs(-1)",
                1
            );
        }

        [Test]
        public void NamespaceImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("ns1", typeof(Math)) }),
                "ns1.Abs(-1)",
                1
            );
        }

        [Test]
        public void NestedNamespaceImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("ns1", new Import("ns2", typeof(Math))) }),
                "ns1.ns2.Abs(-1)",
                1
            );
        }

        [Test]
        public void DoubleNestedNamespaceImport()
        {
            Resolve(
                new ExpressionContext(new[] { new Import("ns1", new Import("ns2", new Import("ns3", typeof(Math)))) }),
                "ns1.ns2.ns3.Abs(-1)",
                1
            );
        }

        [Test]
        public void NestedImportOnNamespace()
        {
            Resolve(
                new ExpressionContext(
                    new[]
                    {
                        new Import(
                            "ns",
                            new Import(typeof(Math)),
                            new Import(typeof(int))
                        )
                    }
                ),
                "ns.Abs(ns.MinValue + 10)",
                int.MaxValue - 9
            );
        }

        [Test]
        public void NestedImportOnNestedNamespace()
        {
            Resolve(
                new ExpressionContext(
                    new[]
                    {
                        new Import(
                            "ns1",
                            new Import(typeof(Math)),
                            new Import(
                                "ns2",
                                new Import(typeof(int))
                            )
                        )
                    }
                ),
                "ns1.Abs(ns1.ns2.MinValue + 10)",
                int.MaxValue - 9
            );
        }
    }
}
