using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.ResolvedAst
{
    internal interface ITypedResolvedIdentifier : IResolvedIdentifier
    {
        Type Type { get; }
    }
}
