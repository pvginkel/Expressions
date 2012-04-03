using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Describes the options available when binding an expression.
    /// </summary>
    public class BoundExpressionOptions
    {
        private bool _frozen;
        private bool _allowPrivateAccess;
        private bool _checked;
        private Type _resultType = typeof(object);
        private bool _restrictedSkipVisibility;

        /// <summary>
        /// Get or set whether the expression may access private members.
        /// </summary>
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

        /// <summary>
        /// Get or set the result type of the expression (when left at the default
        /// value, using <see cref="DynamicExpression{T}"/> will automatically
        /// provide the value for this parameter).
        /// </summary>
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

        /// <summary>
        /// Get or set whether the expression must perform checked arithmetic.
        /// </summary>
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

        /// <summary>
        /// Get or set whether the visibility checkes must be skipped when
        /// the expression is compiled.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundExpressionOptions"/>
        /// class.
        /// </summary>
        public BoundExpressionOptions()
        {
        }

        internal BoundExpressionOptions(BoundExpressionOptions other)
        {
            Require.NotNull(other, "other");

            AllowPrivateAccess = other.AllowPrivateAccess;
            Checked = other.Checked;
            ResultType = other.ResultType;
            RestrictedSkipVisibility = other.RestrictedSkipVisibility;
        }

        internal void Freeze()
        {
            _frozen = true;
            _restrictedSkipVisibility = true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        ///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        ///                 </exception><filterpriority>2</filterpriority>
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
                _restrictedSkipVisibility == other._restrictedSkipVisibility;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return ObjectUtil.CombineHashCodes(
                    _allowPrivateAccess.GetHashCode(),
                    _checked.GetHashCode(),
                    _resultType.GetHashCode(),
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
