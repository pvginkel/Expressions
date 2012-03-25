using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Expressions
{
    internal class CachedDynamicExpression
    {
        private readonly BoundExpressionCache _boundExpressionCache;

        public ParseResult ParseResult { get; private set; }

        public ExpressionLanguage Language { get; private set; }

        public CultureInfo ParsingCulture { get; private set; }

        public CachedDynamicExpression(ParseResult parseResult, ExpressionLanguage language, CultureInfo parsingCulture)
        {
            Require.NotNull(parseResult, "parseResult");
            Require.NotNull(parsingCulture, "parsingCulture");

            _boundExpressionCache = new BoundExpressionCache(this);

            ParseResult = parseResult;
            Language = language;
            ParsingCulture = parsingCulture;
        }

        public BoundExpression GetOrCreateBoundExpression(IBindingContext binder, BoundExpressionOptions options)
        {
            return _boundExpressionCache.GetOrCreateBoundExpression(binder, options);
        }
    }
}
