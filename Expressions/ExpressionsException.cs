using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Expressions
{
    [Serializable]
    public class ExpressionsException : Exception
    {
        public ExpressionsExceptionType Type { get; private set; }

        public ExpressionsException(ExpressionsExceptionType type)
        {
            Type = type;
        }

        public ExpressionsException(string message, ExpressionsExceptionType type)
            : base(message)
        {
            Type = type;
        }

        public ExpressionsException(string message, ExpressionsExceptionType type, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
        }

        protected ExpressionsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Type = (ExpressionsExceptionType)info.GetValue("Type", typeof(ExpressionsExceptionType));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Type", Type);
        }
    }
}
