using System;
using System.Collections.Generic;
using System.Text;
using Expressions.ResolvedAst;

namespace Expressions.Test.Resolving
{
    internal class ResolvedNodePrinter
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public ResolvedNodePrinter(IResolvedAstNode resolvedNode)
        {
            if (resolvedNode == null)
                throw new ArgumentNullException("resolvedNode");

            PrintNode(resolvedNode, 0);
        }

        private void PrintNode(IResolvedAstNode node, int indent)
        {
            AppendLine(indent, "{" + node.GetType().Name + "}");

            if (node is ResolvedBinaryExpression)
                PrintResolvedBinaryExpression((ResolvedBinaryExpression)node, indent + 1);
            else if (node is ResolvedCast)
                PrintResolvedCast((ResolvedCast)node, indent + 1);
            else if (node is ResolvedConstant)
                PrintResolvedConstant((ResolvedConstant)node, indent + 1);
            else if (node is ResolvedIdentifierAccess)
                PrintResolvedIdentifierAccess((ResolvedIdentifierAccess)node, indent + 1);
            else if (node is ResolvedIndex)
                PrintResolvedIndex((ResolvedIndex)node, indent + 1);
            else if (node is ResolvedMemberAccess)
                PrintResolvedMemberAccess((ResolvedMemberAccess)node, indent + 1);
            else if (node is ResolvedMethodCall)
                PrintResolvedMethodCall((ResolvedMethodCall)node, indent + 1);
            else if (node is ResolvedType)
                PrintResolvedType((ResolvedType)node, indent + 1);
            else if (node is ResolvedUnaryExpression)
                PrintResolvedUnaryExpression((ResolvedUnaryExpression)node, indent + 1);
            else
                throw new NotImplementedException();
        }

        private void PrintResolvedUnaryExpression(ResolvedUnaryExpression node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "ExpressionType = " + node.ExpressionType);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void PrintResolvedType(ResolvedType node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "Identifier =");
            PrintNode(node.Identifier, indent + 1);
        }

        private void PrintResolvedMethodCall(ResolvedMethodCall node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Method =");
            PrintNode(node.Method, indent + 1);
            AppendLine(indent, "Arguments =");

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                AppendLine(indent + 1, "[1]");
                PrintNode(node.Arguments[i], indent + 2);
            }
        }

        private void PrintResolvedMemberAccess(ResolvedMemberAccess node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Identifier =");
            PrintNode(node.Identifier, indent + 1);
        }

        private void PrintResolvedIndex(ResolvedIndex node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Property =");
            PrintNode(node.Property, indent + 1);
            AppendLine(indent, "Arguments =");
            PrintNodeArray(indent, node.Arguments);
        }

        private void PrintNodeArray(int indent, IList<IResolvedAstNode> arguments)
        {
            for (int i = 0; i < arguments.Count; i++)
            {
                AppendLine(indent + 1, "[" + i + "]");
                PrintNode(arguments[i], indent + 2);
            }
        }

        private void PrintResolvedIdentifierAccess(ResolvedIdentifierAccess node, int indent)
        {
            AppendLine(indent, "Identifier =");
            PrintNode(node.Identifier, indent + 1);
        }

        private void PrintResolvedConstant(ResolvedConstant node, int indent)
        {
            AppendLine(indent, "Value = " + node.Value);
            AppendLine(indent, "Identifier =");
            PrintNode(node.Identifier, indent + 1);
        }

        private void PrintResolvedCast(ResolvedCast node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void AppendLine(int indent, string line)
        {
            _sb.AppendLine(new string(' ', indent * 4) + line);
        }

        private void PrintResolvedBinaryExpression(ResolvedBinaryExpression node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "ExpressionType = " + node.ExpressionType);
            AppendLine(indent, "Left =");
            PrintNode(node.Left, indent + 1);
            AppendLine(indent, "Right =");
            PrintNode(node.Right, indent + 1);
            AppendLine(indent, "Identifier =");
            PrintNode(node.Identifier, indent + 1);
        }

        private void PrintNode(IResolvedIdentifier node, int indent)
        {
            AppendLine(indent, "{" + node.GetType().Name + "}");

            if (node is BinaryIdentifier)
                PrintBinaryIdentifier((BinaryIdentifier)node, indent + 1);
            else if (node is ConstantIdentifier)
                PrintConstantIdentifier((ConstantIdentifier)node, indent + 1);
            else if (node is FieldIdentifier)
                PrintFieldIdentifier((FieldIdentifier)node, indent + 1);
            else if (node is GlobalIdentifier)
                PrintGlobalIdentifier((GlobalIdentifier)node, indent + 1);
            else if (node is ImportIdentifier)
                PrintImportIdentifier((ImportIdentifier)node, indent + 1);
            else if (node is MethodGroupIdentifier)
                PrintMethodGroupIdentifier((MethodGroupIdentifier)node, indent + 1);
            else if (node is MethodIdentifier)
                PrintMethodIdentifier((MethodIdentifier)node, indent + 1);
            else if (node is NullIdentifier)
                PrintNullIdentifier((NullIdentifier)node, indent + 1);
            else if (node is PropertyGroupIdentifier)
                PrintPropertyGroupIdentifier((PropertyGroupIdentifier)node, indent + 1);
            else if (node is PropertyIdentifier)
                PrintPropertyIdentifier((PropertyIdentifier)node, indent + 1);
            else if (node is TypeIdentifier)
                PrintTypeIdentifier((TypeIdentifier)node, indent + 1);
            else if (node is VariableIdentifier)
                PrintVariableIdentifier((VariableIdentifier)node, indent + 1);
            else
                throw new NotImplementedException();
        }

        private void PrintVariableIdentifier(VariableIdentifier node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
            AppendLine(indent, "ParameterIndex = " + node.ParameterIndex);
        }

        private void PrintTypeIdentifier(TypeIdentifier node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
        }

        private void PrintPropertyIdentifier(PropertyIdentifier node, int indent)
        {
            AppendLine(indent, "Property = " + node.Property);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void PrintPropertyGroupIdentifier(PropertyGroupIdentifier node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Properties =");

            for (int i = 0; i < node.Properties.Count; i++)
            {
                AppendLine(indent + 1, "[" + i + "] = " + node.Properties[i]);
            }
        }

        private void PrintNullIdentifier(NullIdentifier node, int indent)
        {
        }

        private void PrintMethodIdentifier(MethodIdentifier node, int indent)
        {
            AppendLine(indent, "Method = " + node.Method);
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
        }

        private void PrintMethodGroupIdentifier(MethodGroupIdentifier node, int indent)
        {
            AppendLine(indent, "Operand =");
            PrintNode(node.Operand, indent + 1);
            AppendLine(indent, "Methods =");

            for (int i = 0; i < node.Methods.Count; i++)
            {
                AppendLine(indent + 1, "[" + i + "] = " + node.Methods[i]);
            }
        }

        private void PrintGlobalIdentifier(GlobalIdentifier node, int indent)
        {
        }

        private void PrintImportIdentifier(ImportIdentifier node, int indent)
        {
            AppendLine(indent, "Import = " + node.Import.Type + (node.Import.Namespace == null ? "" : " (" + node.Import.Namespace + ")"));
        }

        private void PrintFieldIdentifier(FieldIdentifier node, int indent)
        {
            AppendLine(indent, "Field = " + node.Field);
        }

        private void PrintConstantIdentifier(ConstantIdentifier node, int indent)
        {
            AppendLine(indent, "Value = " + node.Value);
        }

        private void PrintBinaryIdentifier(BinaryIdentifier node, int indent)
        {
            AppendLine(indent, "Type = " + node.Type);
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
