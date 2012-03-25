using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Expressions
{
    internal static class DynamicExpressionCache
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<CacheKey, CachedDynamicExpression> _cache = new Dictionary<CacheKey, CachedDynamicExpression>();

        public static CachedDynamicExpression GetOrCreateCachedDynamicExpression(string expression, ExpressionLanguage language, CultureInfo parsingCulture)
        {
            var key = new CacheKey(expression, language, parsingCulture);

            lock (_syncRoot)
            {
                CachedDynamicExpression cached;

                if (!_cache.TryGetValue(key, out cached))
                {
                    var parseResult = ParseExpression(expression, language, parsingCulture);

                    cached = new CachedDynamicExpression(parseResult, language, parsingCulture);

                    _cache.Add(key, cached);
                }

                return cached;
            }
        }

        private static ParseResult ParseExpression(string expression, ExpressionLanguage language, CultureInfo parsingCulture)
        {
            switch (language)
            {
                case ExpressionLanguage.Flee:
                    return Flee.FleeParser.Parse(expression, parsingCulture);

                default:
                    throw new ArgumentOutOfRangeException("language");
            }
        }

        private class CacheKey
        {
            private int? _hashCode;
            private readonly string _expression;
            private readonly ExpressionLanguage _language;
            private readonly CultureInfo _parsingCulture;

            public CacheKey(string expression, ExpressionLanguage language, CultureInfo parsingCulture)
            {
                _expression = expression;
                _language = language;
                _parsingCulture = parsingCulture;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                    return true;

                var other = obj as CacheKey;

                return
                    other != null &&
                    _expression == other._expression &&
                    _language == other._language &&
                    _parsingCulture == other._parsingCulture;
            }

            public override int GetHashCode()
            {
                if (!_hashCode.HasValue)
                {
                    unchecked
                    {
                        _hashCode = ObjectUtil.CombineHashCodes(
                            _expression.GetHashCode(),
                            _language.GetHashCode(),
                            _parsingCulture.GetHashCode()
                        );
                    }
                }

                return _hashCode.Value;
            }
        }
    }
}
