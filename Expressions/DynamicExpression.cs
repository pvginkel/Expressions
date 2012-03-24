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
            Require.NotNull(expression, "expression");

            Expression = expression;
            Language = language;

            ParseResult = DynamicExpressionCache.GetOrCreateParseResult(expression, language);
        }

        public BoundExpression Bind(IBindingContext binder)
        {
            return Bind(binder, null);
        }

        public BoundExpression Bind(IBindingContext binder, BoundExpressionOptions options)
        {
            Require.NotNull(binder, "binder");

            if (options == null)
                options = new BoundExpressionOptions();

            options.Freeze();

            return BoundExpressionCache.GetOrCreateBoundExpression(this, binder, options);
        }

        public object Invoke(IExpressionContext expressionContext)
        {
            return Invoke(expressionContext, null);
        }

        public object Invoke(IExpressionContext expressionContext, BoundExpressionOptions options)
        {
            Require.NotNull(expressionContext, "expressionContext");

            return Bind(expressionContext, options).Invoke(expressionContext);
        }

        public static bool IsLanguageCaseSensitive(ExpressionLanguage language)
        {
            switch (language)
            {
                case ExpressionLanguage.Flee: return false;
                default: throw new ArgumentOutOfRangeException("language");
            }
        }
    }
}
