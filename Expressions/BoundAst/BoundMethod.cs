using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Expressions.BoundAst
{
    internal class BoundMethod : IBoundAstNode
    {
        public MethodInfo Method { get; private set; }

        public Type ResultType
        {
            get { return Method.ReturnType; }
        }

        public IBoundAstNode Object { get; private set; }

        public IList<IBoundAstNode> Arguments { get; private set; }

        public BoundMethod(MethodInfo method, IBoundAstNode @object, IList<IBoundAstNode> arguments)
        {
            Require.NotNull(method, "method");
            Require.NotNull(arguments, "arguments");

            Method = method;
            Object = @object;
            Arguments = new ReadOnlyCollection<IBoundAstNode>(arguments);
        }
    }
}
