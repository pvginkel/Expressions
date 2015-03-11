using System;
using System.Collections.Generic;
using System.Text;
using Expressions.Ast;

namespace Expressions
{
    internal class BoundExpressionCache
    {
        private readonly CachedDynamicExpression _dynamicExpression;
        private readonly object _syncRoot = new object();
        private readonly Dictionary<CacheKey, BoundExpression> _cache = new Dictionary<CacheKey, BoundExpression>();

        public BoundExpressionCache(CachedDynamicExpression dynamicExpression)
        {
            Require.NotNull(dynamicExpression, "dynamicExpression");

            _dynamicExpression = dynamicExpression;
        }

        public BoundExpression GetOrCreateBoundExpression(IBindingContext binder, BoundExpressionOptions options)
        {
            Require.NotNull(binder, "binder");
            Require.NotNull(options, "options");

            var key = new CacheKey(
                _dynamicExpression.ParseResult.Identifiers,
                !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language),
                binder,
                options
            );

            lock (_syncRoot)
            {
                BoundExpression boundExpression;

                if (!_cache.TryGetValue(key, out boundExpression))
                {
                    boundExpression = new BoundExpression(
                        _dynamicExpression,
                        key.OwnerType,
                        key.Imports,
                        key.IdentifierTypes,
                        key.Options,
                        binder.GetVariableType
                    );

                    _cache.Add(key, boundExpression);
                }

                return boundExpression;
            }
        }

        private class CacheKey
        {
            private int? _hashCode;

            public BoundExpressionOptions Options { get; private set; }

            public Type OwnerType { get; private set; }

            public Import[] Imports { get; private set; }

            public Type[] IdentifierTypes { get; private set; }

            public CacheKey(IdentifierCollection identifiers, bool ignoreCase, IBindingContext bindingContext, BoundExpressionOptions options)
            {
                Options = options;

                OwnerType = bindingContext.OwnerType;

                var imports = bindingContext.Imports;

                Imports = new Import[imports == null ? 0 : imports.Count];

                if (Imports.Length > 0)
                    imports.CopyTo(Imports, 0);

                IdentifierTypes = new Type[identifiers.Count];

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
                    OwnerType != other.OwnerType ||
                    Imports.Length != other.Imports.Length ||
                    !Options.Equals(other.Options)
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
                            OwnerType == null ? 0 : OwnerType.GetHashCode(),
                            Options.GetHashCode()
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
