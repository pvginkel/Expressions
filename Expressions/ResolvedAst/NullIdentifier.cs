using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.ResolvedAst
{
    internal class NullIdentifier : IResolvedIdentifier
    {
        public static readonly NullIdentifier Instance = new NullIdentifier();

        public bool IsStatic
        {
            get { throw new NotSupportedException(); }
        }

        public bool IsResolved { get { return false; } }

        private NullIdentifier()
        {
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            throw new NotSupportedException(String.Format("Cannot resolve '{0}'", name));
        }
    }
}
