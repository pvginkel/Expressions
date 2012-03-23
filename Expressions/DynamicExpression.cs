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

        public DynamicExpressionOptions Options { get; private set; }

        public DynamicExpression(string expression, ExpressionLanguage language)
            : this(expression, language, null)
        {
        }

        public DynamicExpression(string expression, ExpressionLanguage language, DynamicExpressionOptions options)
        {
            Require.NotNull(expression, "expression");

            Expression = expression;
            Language = language;

            Options = options ?? new DynamicExpressionOptions();
            Options.Freeze();

            ParseResult = DynamicExpressionCache.GetOrCreateParseResult(expression, language, Options);
        }

        public BoundExpression Bind(IBindingContext binder)
        {
            Require.NotNull(binder, "binder");

            return BoundExpressionCache.GetOrCreateBoundExpression(this, binder);
        }

        public object Invoke(IExpressionContext expressionContext)
        {
            Require.NotNull(expressionContext, "expressionContext");

            return Bind(expressionContext).Invoke(expressionContext);
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
