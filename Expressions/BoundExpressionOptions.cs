using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions
{
    public class BoundExpressionOptions
    {
        private bool _frozen;
        private bool _allowPrivateAccess;
        private bool _checked;
        private bool _importBuildInTypes;
        private Type _resultType = typeof(object);
        private bool _restrictedSkipVisibility;

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

        public Type ResultType
        {
            get { return _resultType; }
            set
            {
                if (_frozen)
                    throw new InvalidOperationException("Options cannot be modified");

                _resultType = value ?? typeof(object);
            }
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_frozen)
                    throw new InvalidOperationException("Options cannot be modified");

                _checked = value;
            }
        }

        public bool ImportBuildInTypes
        {
            get { return _importBuildInTypes; }
            set
            {
                if (_frozen)
                    throw new InvalidOperationException("Options cannot be modified");

                _importBuildInTypes = value;
            }
        }

        public bool RestrictedSkipVisibility
        {
            get { return _restrictedSkipVisibility; }
            set
            {
                if (_frozen)
                    throw new InvalidOperationException("Options cannot be modified");

                _restrictedSkipVisibility = value;
            }
        }

        internal void Freeze()
        {
            _frozen = true;
            _restrictedSkipVisibility = true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            var other = obj as BoundExpressionOptions;

            return
                other != null &&
                _allowPrivateAccess == other._allowPrivateAccess &&
                _checked == other._checked &&
                _resultType == other._resultType &&
                _importBuildInTypes == other._importBuildInTypes &&
                _restrictedSkipVisibility == other._restrictedSkipVisibility;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ObjectUtil.CombineHashCodes(
                    _allowPrivateAccess.GetHashCode(),
                    _checked.GetHashCode(),
                    _resultType.GetHashCode(),
                    _importBuildInTypes.GetHashCode(),
                    _restrictedSkipVisibility.GetHashCode()
                );
            }
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
