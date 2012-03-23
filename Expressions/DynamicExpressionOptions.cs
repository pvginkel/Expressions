using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions
{
    public class DynamicExpressionOptions
    {
        private bool _frozen;
        private bool _allowPrivateAccess;

        public bool AllowPrivateAccess
        {
            get { return _allowPrivateAccess; }
            set
            {
                if (_frozen)
                    throw new InvalidOperationException("Options cannot be modified");

                _allowPrivateAccess = value;
            }
        }

        internal void Freeze()
        {
            _frozen = true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as DynamicExpressionOptions;

            return
                other != null &&
                _allowPrivateAccess == other._allowPrivateAccess;
        }

        public override int GetHashCode()
        {
            return _allowPrivateAccess.GetHashCode();
        }

        internal BindingFlags AccessBindingFlags
        {
            get
            {
                if (_allowPrivateAccess)
                    return BindingFlags.Public | BindingFlags.NonPublic;
                else
                    return BindingFlags.Public;
            }
        }
    }
}
