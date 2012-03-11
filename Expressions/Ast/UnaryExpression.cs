using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Ast
{
    internal class UnaryExpression : IAstNode
    {
        public IAstNode Operand { get; private set; }

        public ExpressionType Type { get; private set; }

        public UnaryExpression(IAstNode operand, ExpressionType type)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");

            Operand = operand;
            Type = type;
        }

        public override string ToString()
        {
            return String.Format("({0} {1})", Type, Operand);
        }

        public IResolvedAstNode Resolve(Resolver resolver)
        {
            var operand = Operand.Resolve(resolver);
            Type type;

            switch (Type)
            {
                case ExpressionType.Plus:
                    if (!TypeUtil.IsNumeric(operand.Type))
                        throw new NotSupportedException("Cannot plus non numeric type");

                    type = operand.Type;
                    break;

                case ExpressionType.Minus:
                    if (!TypeUtil.IsNumeric(operand.Type))
                        throw new NotSupportedException("Cannot plus non numeric type");

                    // TODO: Make constants signed and handle minus on unsigneds.

                    type = operand.Type;
                    break;

                case ExpressionType.Not:
                    if (operand.Type != typeof(bool))
                        throw new NotSupportedException("Cannot not non boolean types");

                    type = typeof(bool);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return new ResolvedUnaryExpression(operand, type, Type);
        }
    }
}
