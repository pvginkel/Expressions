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
            if (method == null)
                throw new ArgumentNullException("method");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            Method = method;
            Object = @object;
            Arguments = new ReadOnlyCollection<IBoundAstNode>(arguments);
        }
    }
}
