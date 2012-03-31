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

        public static CachedDynamicExpression GetOrCreateCachedDynamicExpression(string expression, ExpressionLanguage language)
        {
            var key = new CacheKey(expression, language);

            lock (_syncRoot)
            {
                CachedDynamicExpression cached;

                if (!_cache.TryGetValue(key, out cached))
                {
                    var parseResult = ParseExpression(expression, language);

                    cached = new CachedDynamicExpression(parseResult, language);

                    _cache.Add(key, cached);
                }

                return cached;
            }
        }

        public static ParseResult ParseExpression(string expression, ExpressionLanguage language)
        {
            switch (language)
            {
                case ExpressionLanguage.Flee:
                    return Flee.FleeParser.Parse(expression);

                case ExpressionLanguage.Csharp:
                    return Csharp.CsharpParser.Parse(expression);

                case ExpressionLanguage.VisualBasic:
                    return VisualBasic.VisualBasicParser.Parse(expression);

                default:
                    throw new ArgumentOutOfRangeException("language");
            }
        }

        private class CacheKey
        {
            private int? _hashCode;
            private readonly string _expression;
            private readonly ExpressionLanguage _language;

            public CacheKey(string expression, ExpressionLanguage language)
            {
                _expression = expression;
                _language = language;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                    return true;

                var other = obj as CacheKey;

                return
                    other != null &&
                    _expression == other._expression &&
                    _language == other._language;
            }

            public override int GetHashCode()
            {
                if (!_hashCode.HasValue)
                {
                    unchecked
                    {
                        _hashCode = ObjectUtil.CombineHashCodes(
                            _expression.GetHashCode(),
                            _language.GetHashCode()
                        );
                    }
                }

                return _hashCode.Value;
            }
        }
    }
}
