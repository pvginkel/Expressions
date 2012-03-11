using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.ResolvedAst
{
    internal interface IResolvedAstNode
    {
        Type Type { get; }

        IResolvedIdentifier Resolve(Resolver resolver, string member);

        IResolvedIdentifier Identifier { get; }
    }
}
