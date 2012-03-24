using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions.Ast
{
    internal class AstNodeCollection : IAstNode
    {
        private readonly IAstNode[] _nodes;

        public IList<IAstNode> Nodes { get; private set; }

        public AstNodeCollection(IAstNode node)
        {
            Require.NotNull(node, "node");

            _nodes = new[] { node };

            Nodes = new ReadOnlyCollection<IAstNode>(_nodes);
        }

        public AstNodeCollection(AstNodeCollection other, IAstNode node)
        {
            Require.NotNull(other, "other");
            Require.NotNull(node, "node");

            _nodes = new IAstNode[other._nodes.Length + 1];

            Array.Copy(other._nodes, _nodes, other._nodes.Length);

            _nodes[_nodes.Length - 1] = node;

            Nodes = new ReadOnlyCollection<IAstNode>(_nodes);
        }

        public AstNodeCollection(IList<IAstNode> nodes)
        {
            Nodes = new ReadOnlyCollection<IAstNode>(nodes);
        }

        public T Accept<T>(IAstVisitor<T> visitor)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append('(');

            for (int i = 0; i < _nodes.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                sb.Append(_nodes[i]);
            }

            sb.Append(')');

            return sb.ToString();
        }
    }
}
