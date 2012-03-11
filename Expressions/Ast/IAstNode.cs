using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal interface IAstNode
    {
        IResolvedAstNode Resolve(Resolver resolver);
    }
}
