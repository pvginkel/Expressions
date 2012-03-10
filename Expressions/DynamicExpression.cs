using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public sealed class DynamicExpression
    {
        internal ParseResult ParseResult { get; private set; }

        public string Expression { get; private set; }

        public ExpressionLanguage Language { get; private set; }

        public DynamicExpression(string expression, ExpressionLanguage language)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Expression = expression;
            Language = language;

            ParseResult = DynamicExpressionCache.GetOrCreateParseResult(expression, language);
        }

        public BoundExpression Bind(IExpressionBinder binder)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");

            return BoundExpressionCache.GetOrCreateBoundExpression(this, binder);
        }
    }
}
