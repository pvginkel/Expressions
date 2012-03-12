using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

namespace Expressions
{
    internal class ParseResult
    {
        public IAstNode RootNode { get; private set; }

        public IdentifierCollection Identifiers { get; private set; }

        public ParseResult(IAstNode rootNode, IdentifierCollection identifiers)
        {
            Require.NotNull(rootNode, "rootNode");
            Require.NotNull(identifiers, "identifiers");

            RootNode = rootNode;
            Identifiers = identifiers;
        }
    }
}
