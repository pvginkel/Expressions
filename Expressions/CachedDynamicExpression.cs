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

        public CachedDynamicExpression(ParseResult parseResult, ExpressionLanguage language)
        {
            Require.NotNull(parseResult, "parseResult");

            _boundExpressionCache = new BoundExpressionCache(this);

            ParseResult = parseResult;
            Language = language;
        }

        public BoundExpression GetOrCreateBoundExpression(IBindingContext binder, BoundExpressionOptions options)
        {
            return _boundExpressionCache.GetOrCreateBoundExpression(binder, options);
        }
    }
}
