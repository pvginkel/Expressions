using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Expressions.Expressions
{
    internal class MethodCall : IExpression
    {
        private static readonly IExpression[] EmptyArguments = new IExpression[0];

        public IExpression Operand { get; private set; }

        public MethodInfo MethodInfo { get; private set; }

        public IList<IExpression> Arguments { get; private set; }

        public Type Type
        {
            get { return MethodInfo.ReturnType; }
        }

        public MethodCall(IExpression operand, MethodInfo methodInfo, IList<IExpression> arguments)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(methodInfo, "methodInfo");

            Operand = operand;
            MethodInfo = methodInfo;

            if (arguments == null || arguments.Count == 0)
                Arguments = EmptyArguments;
            else
                Arguments = new ReadOnlyCollection<IExpression>(arguments);
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.MethodCall(this);
        }
    }
}
