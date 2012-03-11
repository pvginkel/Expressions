using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal static class BoundExpressionCache
    {
        private static readonly object _syncRoot = new object();
        private static readonly Dictionary<CacheKey, BoundExpression> _cache = new Dictionary<CacheKey, BoundExpression>();

        public static BoundExpression GetOrCreateBoundExpression(DynamicExpression dynamicExpression, IBindingContext binder)
        {
            if (dynamicExpression == null)
                throw new ArgumentNullException("dynamicExpression");
            if (binder == null)
                throw new ArgumentNullException("binder");

            var key = new CacheKey(dynamicExpression, binder);

            lock (_syncRoot)
            {
                BoundExpression boundExpression;

                if (!_cache.TryGetValue(key, out boundExpression))
                {
                    boundExpression = new BoundExpression(
                        key.DynamicExpression,
                        key.OwnerType,
                        key.Imports,
                        key.IdentifierTypes
                    );

                    _cache.Add(key, boundExpression);
                }

                return boundExpression;
            }
        }

        private class CacheKey
        {
            private int? _hashCode;

            public DynamicExpression DynamicExpression { get; private set; }

            public Type OwnerType { get; private set; }

            public Import[] Imports { get; private set; }

            public Type[] IdentifierTypes { get; private set; }

            public CacheKey(DynamicExpression dynamicExpression, IBindingContext bindingContext)
            {
                DynamicExpression = dynamicExpression;

                OwnerType = bindingContext.OwnerType;

                var imports = bindingContext.Imports;

                Imports = new Import[imports == null ? 0 : imports.Count];

                if (Imports.Length > 0)
                    imports.CopyTo(Imports, 0);

                var identifiers = dynamicExpression.ParseResult.Identifiers;

                IdentifierTypes = new Type[identifiers.Count];

                bool ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(dynamicExpression.Language);

                for (int i = 0; i < identifiers.Count; i++)
                {
                    IdentifierTypes[i] = bindingContext.GetVariableType(
                        identifiers[i].Name,
                        ignoreCase
                    );
                }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj))
                    return true;

                var other = obj as CacheKey;

                if (
                    other == null ||
                    DynamicExpression != other.DynamicExpression ||
                    OwnerType != other.OwnerType ||
                    Imports.Length != other.Imports.Length
                )
                    return false;

                for (int i = 0; i < Imports.Length; i++)
                {
                    if (!Equals(Imports[i], other.Imports[i]))
                        return false;
                }

                for (int i = 0; i < IdentifierTypes.Length; i++)
                {
                    if (!ReferenceEquals(IdentifierTypes[i], other.IdentifierTypes[i]))
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
                        int hashCode = ObjectUtil.CombineHashCodes(
                            DynamicExpression.GetHashCode(),
                            OwnerType == null ? 0 : OwnerType.GetHashCode()
                        );

                        for (int i = 0; i < Imports.Length; i++)
                        {
                            hashCode = ObjectUtil.CombineHashCodes(hashCode, Imports[i].GetHashCode());
                        }

                        for (int i = 0; i < IdentifierTypes.Length; i++)
                        {
                            if (IdentifierTypes[i] != null)
                                hashCode = ObjectUtil.CombineHashCodes(hashCode, IdentifierTypes[i].GetHashCode());
                        }

                        _hashCode = hashCode;
                    }
                }

                return _hashCode.Value;
            }
        }
    }
}
