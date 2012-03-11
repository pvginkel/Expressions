using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal class NullResolver : IIdentifierResolver
    {
        public static readonly NullResolver Instance = new NullResolver();

        private NullResolver()
        {
        }
    }
}
