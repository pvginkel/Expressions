using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Equals is here just for unit testing
#pragma warning disable 0659

namespace Expressions.ResolvedAst
{
    internal class PropertyIdentifier : ITypedResolvedIdentifier
    {
        public bool IsStatic { get { return Property.IsStatic; } }

        public bool IsResolved { get { return true; } }

        public IResolvedIdentifier Operand { get; private set; }

        public MethodInfo Property { get; private set; }

        public Type Type { get { return Property.ReturnType; } }

        public PropertyIdentifier(IResolvedIdentifier operand, MethodInfo propertyInfo)
        {
            if (operand == null)
                throw new ArgumentNullException("operand");
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            Operand = operand;
            Property = propertyInfo;
        }

        public IResolvedIdentifier Resolve(Resolver resolver, string name)
        {
            return resolver.Resolve(this, Type, name, false);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as PropertyIdentifier;

            return
                other != null &&
                Equals(Operand, other.Operand) &&
                Property == other.Property;
        }
    }
}
