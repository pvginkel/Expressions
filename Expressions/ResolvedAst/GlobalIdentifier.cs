using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.ResolvedAst
{
    internal class GlobalIdentifier : IResolvedIdentifier
    {
        public static readonly GlobalIdentifier Instance = new GlobalIdentifier();

        public bool IsStatic { get { return true; } }

        public bool IsResolved { get { return false; } }

        private GlobalIdentifier()
        {
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.ResolveGlobal(name);
        }
    }
}
