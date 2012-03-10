using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Ast
{
    internal class MethodCall : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public AstNodeCollection Arguments { get; private set; }

        public MethodCall(IAstNode operand)
            : this(operand, null)
        {
        }

        public MethodCall(IAstNode operand, AstNodeCollection arguments)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");

            Operand = operand;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return Operand + (Arguments == null ? "()" : Arguments.ToString());
        }
    }
}
