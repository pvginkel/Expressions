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
                    if ((NumberStyles & NumberStyles.AllowHexSpecifier) != 0)
                    {
                        ulong hexValue = ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

                        if (hexValue <= int.MaxValue)
                            return (int)hexValue;
                        else if (hexValue <= uint.MaxValue)
                            return (uint)hexValue;
                        else if (hexValue <= long.MaxValue)
                            return (long)hexValue;
                        else
                            return hexValue;
                    }

                    long intValue;

                    if (!long.TryParse(Value, NumberStyles, CultureInfo.InvariantCulture, out intValue))
                        return ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                    
                    if (intValue >= int.MinValue && intValue <= int.MaxValue)
                        return (int)intValue;
                    else
                        return intValue;

                case TypeCode.UInt32:
                    ulong uintValue = ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

                    if (uintValue <= uint.MaxValue)
                        return (uint)uintValue;
                    else
                        return uintValue;

                case TypeCode.Int64:
                    long longValue;

                    if (!long.TryParse(Value, NumberStyles, CultureInfo.InvariantCulture, out longValue))
                        return ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);
                    return longValue;

                case TypeCode.UInt64:
                    return ulong.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

                case TypeCode.Single:
                    return float.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

                case TypeCode.Double:
                    return double.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

                case TypeCode.Decimal:
                    return decimal.Parse(Value, NumberStyles, CultureInfo.InvariantCulture);

                case TypeCode.Char:
                    return char.Parse(Value);

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
