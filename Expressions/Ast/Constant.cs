using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class Constant : IAstNode
    {
        public static readonly Constant True = new Constant(true);
        public static readonly Constant False = new Constant(false);
        public static readonly Constant Null = new Constant(null);

        public object Value { get; private set; }

        public Constant(object value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value == null ? "null" : Value.ToString();
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            return new ResolvedConstant(Value);
        }
    }
}
