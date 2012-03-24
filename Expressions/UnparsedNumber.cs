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
            switch (Type.GetTypeCode(Type))
            {
                case TypeCode.Int32:
                    return int.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                case TypeCode.UInt32:
                    return uint.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                case TypeCode.Int64:
                    return long.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                case TypeCode.UInt64:
                    return ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                case TypeCode.Single:
                    return float.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                case TypeCode.Double:
                    return double.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                case TypeCode.Decimal:
                    return decimal.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                default:
                    throw new InvalidOperationException("Unexpected UnparsedNumber type");
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
