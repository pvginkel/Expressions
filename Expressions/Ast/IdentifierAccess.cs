using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class IdentifierAccess : IAstNode
    {
        public string Name { get; private set; }

        public IdentifierAccess(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            var identifier = GlobalIdentifier.Instance.Resolve(resolver, Name);

            return new ResolvedIdentifierAccess(identifier);
        }
    }
}
