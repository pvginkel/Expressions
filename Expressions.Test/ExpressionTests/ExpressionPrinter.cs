using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Expressions;

namespace Expressions.Test.ExpressionTests
{
    internal class ExpressionPrinter
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public ExpressionPrinter(IExpression resolvedNode)
        {
            if (resolvedNode == null)
                throw new ArgumentNullException("resolvedNode");

            PrintNode(resolvedNode, 0);
        }

        private void PrintNode(IExpression node, int indent)
        {
            if (node == null)
                return;

            AppendLine(indent, "{" + node.GetType().Name + "}");

            if (node is BinaryExpression)
                PrintResolvedBinaryExpression((BinaryExpression)node, indent + 1);
            else if (node is Cast)
                PrintResolvedCast((Cast)node, indent + 1);
            else if (node is Constant)
                PrintResolvedConstant((Constant)node, indent + 1);
            else if (node is FieldAccess)
                PrintResolvedFieldAccess((FieldAccess)node, indent + 1);
            else if (node is Index)
                PrintResolvedIndex((Index)node, indent + 1);
            else if (node is MethodCall)
                PrintResolvedMethodCall((MethodCall)node, indent + 1);
            else if (node is UnaryExpression)
                PrintResolvedUnaryExpression((UnaryExpression)node, indent + 1);
            else if (node is VariableAccess)
                PrintResolvedVariableAccess((VariableAccess)node, indent + 1);
            else
                throw new NotSupportedException();
        }

        private void PrintResolvedVariableAccess(VariableAccess node, int indent)
        {
            AppendLine(indent, "ParameterIndex = " + node.ParameterIndex);
        }

        private void PrintResolvedFieldAccess(FieldAccess node, int indent)
        {
            AppendLine(indent, "FieldInfo = " + node.FieldInfo + " on " + node.FieldInfo.DeclaringType.Name);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void PrintResolvedUnaryExpression(UnaryExpression node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "ExpressionType = " + node.ExpressionType);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void PrintResolvedMethodCall(MethodCall node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Method = " + node.MethodInfo + " on " + node.MethodInfo.DeclaringType.Name);
            AppendLine(indent, "Arguments =");

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                AppendLine(indent + 1, "[1]");
                PrintNode(node.Arguments[i], indent + 2);
            }
        }

        private void PrintResolvedIndex(Index node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "Arguments =");
            PrintNodeArray(indent, node.Arguments);
        }

        private void PrintNodeArray(int indent, IList<IExpression> arguments)
        {
            for (int i = 0; i < arguments.Count; i++)
            {
                AppendLine(indent + 1, "[" + i + "]");
                PrintNode(arguments[i], indent + 2);
            }
        }

        private void PrintResolvedConstant(Constant node, int indent)
        {
            AppendLine(indent, "Value = " + node.Value);
        }

        private void PrintResolvedCast(Cast node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void AppendLine(int indent, string line)
        {
            _sb.AppendLine(new string(' ', indent * 4) + line);
        }

        private void PrintResolvedBinaryExpression(BinaryExpression node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "ExpressionType = " + node.ExpressionType);
            AppendLine(indent, "Left =");
            PrintNode(node.Left, indent + 1);
            AppendLine(indent, "Right =");
            PrintNode(node.Right, indent + 1);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
