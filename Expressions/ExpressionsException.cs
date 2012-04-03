using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Represents an error that occurred while compiling, binding or
    /// executing an expression.
    /// </summary>
    [Serializable]
    public class ExpressionsException : Exception
    {
        /// <summary>
        /// Get the type of the error.
        /// </summary>
        public ExpressionsExceptionType Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsException"/>
        /// class with the specified type.
        /// </summary>
        /// <param name="type">The type of the error.</param>
        public ExpressionsException(ExpressionsExceptionType type)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsException"/>
        /// class with the specified message and type.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="type">The type of the error.</param>
        public ExpressionsException(string message, ExpressionsExceptionType type)
            : base(message)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionsException"/>
        /// class with the specified message, type and inner exception.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="type">The type of the error.</param>
        /// <param name="innerException">The inner exception of the error.</param>
        public ExpressionsException(string message, ExpressionsExceptionType type, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with serialized data.
        /// 
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        ///                 </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.
        ///                 </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null.
        ///                 </exception><exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        ///                 </exception>
        protected ExpressionsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Type = (ExpressionsExceptionType)info.GetValue("Type", typeof(ExpressionsExceptionType));
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown. 
        ///                 </param><param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination. 
        ///                 </param><exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). 
        ///                 </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Type", Type);
        }
    }
}
