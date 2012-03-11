using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.ResolvedAst
{
    internal interface IResolvedIdentifier
    {
        bool IsStatic { get; }

        bool IsResolved { get; }

        IResolvedIdentifier Resolve(Resolver resolver, string name);
    }
}
