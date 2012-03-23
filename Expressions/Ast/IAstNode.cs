using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal interface IAstNode
    {
        T Accept<T>(IAstVisitor<T> visitor);
    }
}
