using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Expressions
{
    public class CompilationException : Exception
    {
        public CompilationException()
        {
        }

        public CompilationException(string message)
            : base(message)
        {
        }

        public CompilationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CompilationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
