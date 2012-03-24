using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Expressions
{
    internal class UnparsedNumber
    {
        public string Value { get; private set; }

        public Type Type { get; private set; }

        public NumberStyles NumberStyles { get; private set; }

        public UnparsedNumber(string value, Type type, NumberStyles numberStyles)
        {
            Require.NotEmpty(value, "value");
            Require.NotNull(type, "type");

            Value = value;
            Type = type;
            NumberStyles = numberStyles;
        }

        public object Parse()
        {
            if (Type == typeof(int))
                return int.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
            if (Type == typeof(uint))
                return uint.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
            if (Type == typeof(long))
                return long.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
            if (Type == typeof(ulong))
                return ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
            if (Type == typeof(float))
                return float.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
            if (Type == typeof(double))
                return double.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
            if (Type == typeof(decimal))
                return decimal.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

            throw new InvalidOperationException("Unexpected UnparsedNumber type");
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
