using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions.Expressions
{
    internal class ImportAccess : IExpression
    {
        public Type Type
        {
            get { throw new NotSupportedException(); }
        }

        public Import Import { get; private set; }

        public ImportAccess(Import import)
        {
            Require.NotNull(import, "import");

            Import = import;
        }

        public void Accept(IExpressionVisitor visitor)
        {
            throw new NotSupportedException();
        }

        public T Accept<T>(IExpressionVisitor<T> visitor)
        {
            throw new NotSupportedException();
        }
    }
}
