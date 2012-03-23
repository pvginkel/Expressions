using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal static class DynamicExpressionCache
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<CacheKey, ParseResult> _cache = new Dictionary<CacheKey, ParseResult>();

        public static ParseResult GetOrCreateParseResult(string expression, ExpressionLanguage language, DynamicExpressionOptions options)
        {
            var key = new CacheKey(expression, language, options);

            lock (_syncRoot)
            {
                ParseResult parseResult;

                if (!_cache.TryGetValue(key, out parseResult))
                {
                    parseResult = ParseExpression(expression, language);

                    _cache.Add(key, parseResult);
                }

                return parseResult;
            }
        }

        private static ParseResult ParseExpression(string expression, ExpressionLanguage language)
        {
            switch (language)
            {
                case ExpressionLanguage.Flee:
                    return Flee.FleeParser.Parse(expression);

                default:
                    throw new ArgumentOutOfRangeException("language");
            }
        }

        private class CacheKey
        {
            private int? _hashCode;
            private readonly string _expression;
            private readonly ExpressionLanguage _language;
            private readonly DynamicExpressionOptions _options;

            public CacheKey(string expression, ExpressionLanguage language, DynamicExpressionOptions options)
            {
                _expression = expression;
                _language = language;
                _options = options;
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
                    _options.Equals(other._options);
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
                            _options.GetHashCode()
                        );
                    }
                }

                return _hashCode.Value;
            }
        }
    }
}
