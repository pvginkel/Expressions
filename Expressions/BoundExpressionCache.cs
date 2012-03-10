using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal static class BoundExpressionCache
    {
        private static readonly object _syncRoot = new object();
        private static Dictionary<CacheKey, BoundExpression> _cache = new Dictionary<CacheKey, BoundExpression>();

        public static BoundExpression GetOrCreateBoundExpression(DynamicExpression dynamicExpression, IExpressionBinder binder)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (binder == null)
                throw new ArgumentNullException("binder");

            var resolvedIdentifiers = ResolveIdentifiers(dynamicExpression, binder);

            var key = new CacheKey(dynamicExpression, resolvedIdentifiers);

            lock (_syncRoot)
            {
                BoundExpression boundExpression;

                if (!_cache.TryGetValue(key, out boundExpression))
                {
                    boundExpression = new BoundExpression(dynamicExpression, resolvedIdentifiers);

                    _cache.Add(key, boundExpression);
                }

                return boundExpression;
            }
        }

        private static IIdentifierResolver[] ResolveIdentifiers(DynamicExpression dynamicExpression, IExpressionBinder binder)
        {
            throw new NotImplementedException();
        }

        private class CacheKey
        {
            private readonly DynamicExpression _dynamicExpression;
            private readonly IIdentifierResolver[] _resolvedIdentifiers;
            private int? _hashCode;

            public CacheKey(DynamicExpression dynamicExpression, IIdentifierResolver[] resolvedIdentifiers)
            {
                _dynamicExpression = dynamicExpression;
                _resolvedIdentifiers = resolvedIdentifiers;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                    return true;

                var other = obj as CacheKey;

                if (
                    other == null ||
                    _dynamicExpression != other._dynamicExpression ||
                    _resolvedIdentifiers.Length != other._resolvedIdentifiers.Length
                )
                    return false;

                for (int i = 0; i < _resolvedIdentifiers.Length; i++)
                {
                    if (!_resolvedIdentifiers[i].Equals(other._resolvedIdentifiers[i]))
                        return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                if (!_hashCode.HasValue)
                {
                    unchecked
                    {
                        int hashCode = _dynamicExpression.GetHashCode();

                        for (int i = 0; i < _resolvedIdentifiers.Length; i++)
                        {
                            hashCode = ObjectUtil.CombineHashCodes(hashCode, _resolvedIdentifiers[i].GetHashCode());
                        }

                        _hashCode = hashCode;
                    }
                }

                return _hashCode.Value;
            }
        }
    }
}
