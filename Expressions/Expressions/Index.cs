using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions.Expressions
{
    internal class Index : IExpression
    {
        private static readonly IExpression[] EmptyArguments = new IExpression[0];

        public IExpression Operand { get; private set; }

        public IList<IExpression> Arguments { get; private set; }

        public Type Type { get; private set; }

        public Index(IExpression operand, IList<IExpression> arguments, Type type)
        {
            Require.NotNull(operand, "operand");
            Require.NotNull(arguments, "arguments");
            Require.NotNull(type, "type");

            Operand = operand;
            Type = type;

            Arguments = new ReadOnlyCollection<IExpression>(arguments);
        }
    }
}
