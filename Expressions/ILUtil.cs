using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Expressions
{
    internal static class ILUtil
    {
        public static void Emit(ILGenerator il, OpCode opcode, MethodBase methodBase)
        {
            Debug.Assert(methodBase is MethodInfo || methodBase is ConstructorInfo);

            if (methodBase.MemberType == MemberTypes.Constructor)
            {
                il.Emit(opcode, (ConstructorInfo)methodBase);
            }
            else
            {
                il.Emit(opcode, (MethodInfo)methodBase);
            }
        }

        public static void EmitLoadArg(ILGenerator il, int index)
        {
            Debug.Assert(index >= 0);

            switch (index)
            {
                case 0:
                    il.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    il.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    il.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    il.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= Byte.MaxValue)
                    {
                        il.Emit(OpCodes.Ldarg_S, (byte)index);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldarg, index);
                    }
                    break;
            }
        }

        public static void EmitLoadArgAddress(ILGenerator il, int index)
        {
            Debug.Assert(index >= 0);

            if (index <= Byte.MaxValue)
            {
                il.Emit(OpCodes.Ldarga_S, (byte)index);
            }
            else
            {
                il.Emit(OpCodes.Ldarga, index);
            }
        }

        public static void EmitStoreArg(ILGenerator il, int index)
        {
            Debug.Assert(index >= 0);

            if (index <= Byte.MaxValue)
            {
                il.Emit(OpCodes.Starg_S, (byte)index);
            }
            else
            {
                il.Emit(OpCodes.Starg, index);
            }
        }

        /// <summary>
        /// Emits a Ldind* instruction for the appropriate type
        /// </summary>
        public static void EmitLoadValueIndirect(ILGenerator il, Type type)
        {
            Require.NotNull(type, "type");
            if (type.IsValueType)
            {
                if (type == typeof(int))
                {
                    il.Emit(OpCodes.Ldind_I4);
                }
                else if (type == typeof(uint))
                {
                    il.Emit(OpCodes.Ldind_U4);
                }
                else if (type == typeof(short))
                {
                    il.Emit(OpCodes.Ldind_I2);
                }
                else if (type == typeof(ushort))
                {
                    il.Emit(OpCodes.Ldind_U2);
                }
                else if (type == typeof(long) || type == typeof(ulong))
                {
                    il.Emit(OpCodes.Ldind_I8);
                }
                else if (type == typeof(char))
                {
                    il.Emit(OpCodes.Ldind_I2);
                }
                else if (type == typeof(bool))
                {
                    il.Emit(OpCodes.Ldind_I1);
                }
                else if (type == typeof(float))
                {
                    il.Emit(OpCodes.Ldind_R4);
                }
                else if (type == typeof(double))
                {
                    il.Emit(OpCodes.Ldind_R8);
                }
                else
                {
                    il.Emit(OpCodes.Ldobj, type);
                }
            }
            else
            {
                il.Emit(OpCodes.Ldind_Ref);
            }
        }


        /// <summary>
        /// Emits a Stind* instruction for the appropriate type.
        /// </summary>
        public static void EmitStoreValueIndirect(ILGenerator il, Type type)
        {
            Require.NotNull(type, "type");

            if (type.IsValueType)
            {
                if (type == typeof(int))
                {
                    il.Emit(OpCodes.Stind_I4);
                }
                else if (type == typeof(short))
                {
                    il.Emit(OpCodes.Stind_I2);
                }
                else if (type == typeof(long) || type == typeof(ulong))
                {
                    il.Emit(OpCodes.Stind_I8);
                }
                else if (type == typeof(char))
                {
                    il.Emit(OpCodes.Stind_I2);
                }
                else if (type == typeof(bool))
                {
                    il.Emit(OpCodes.Stind_I1);
                }
                else if (type == typeof(float))
                {
                    il.Emit(OpCodes.Stind_R4);
                }
                else if (type == typeof(double))
                {
                    il.Emit(OpCodes.Stind_R8);
                }
                else
                {
                    il.Emit(OpCodes.Stobj, type);
                }
            }
            else
            {
                il.Emit(OpCodes.Stind_Ref);
            }
        }

        // Emits the Ldelem* instruction for the appropriate type

        public static void EmitLoadElement(ILGenerator il, Type type)
        {
            Require.NotNull(type, "type");

            if (!type.IsValueType)
            {
                il.Emit(OpCodes.Ldelem_Ref);
            }
            else if (type.IsEnum)
            {
                il.Emit(OpCodes.Ldelem, type);
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                        il.Emit(OpCodes.Ldelem_I1);
                        break;
                    case TypeCode.Byte:
                        il.Emit(OpCodes.Ldelem_U1);
                        break;
                    case TypeCode.Int16:
                        il.Emit(OpCodes.Ldelem_I2);
                        break;
                    case TypeCode.Char:
                    case TypeCode.UInt16:
                        il.Emit(OpCodes.Ldelem_U2);
                        break;
                    case TypeCode.Int32:
                        il.Emit(OpCodes.Ldelem_I4);
                        break;
                    case TypeCode.UInt32:
                        il.Emit(OpCodes.Ldelem_U4);
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        il.Emit(OpCodes.Ldelem_I8);
                        break;
                    case TypeCode.Single:
                        il.Emit(OpCodes.Ldelem_R4);
                        break;
                    case TypeCode.Double:
                        il.Emit(OpCodes.Ldelem_R8);
                        break;
                    default:
                        il.Emit(OpCodes.Ldelem, type);
                        break;
                }
            }
        }

        /// <summary>
        /// Emits a Stelem* instruction for the appropriate type.
        /// </summary>
        public static void EmitStoreElement(ILGenerator il, Type type)
        {
            Require.NotNull(type, "type");

            if (type.IsEnum)
            {
                il.Emit(OpCodes.Stelem, type);
                return;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                    il.Emit(OpCodes.Stelem_I1);
                    break;
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                    il.Emit(OpCodes.Stelem_I2);
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Stelem_I4);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Stelem_I8);
                    break;
                case TypeCode.Single:
                    il.Emit(OpCodes.Stelem_R4);
                    break;
                case TypeCode.Double:
                    il.Emit(OpCodes.Stelem_R8);
                    break;
                default:
                    if (type.IsValueType)
                    {
                        il.Emit(OpCodes.Stelem, type);
                    }
                    else
                    {
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                    break;
            }
        }

        public static void EmitType(ILGenerator il, Type type)
        {
            Require.NotNull(type, "type");

            il.Emit(OpCodes.Ldtoken, type);
            il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
        }

        public static void EmitFieldAddress(ILGenerator il, FieldInfo fi)
        {
            Require.NotNull(fi, "fi");

            if (fi.IsStatic)
            {
                il.Emit(OpCodes.Ldsflda, fi);
            }
            else
            {
                il.Emit(OpCodes.Ldflda, fi);
            }
        }

        public static void EmitFieldGet(ILGenerator il, FieldInfo fi)
        {
            Require.NotNull(fi, "fi");

            if (fi.IsStatic)
            {
                il.Emit(OpCodes.Ldsfld, fi);
            }
            else
            {
                il.Emit(OpCodes.Ldfld, fi);
            }
        }

        public static void EmitFieldSet(ILGenerator il, FieldInfo fi)
        {
            Require.NotNull(fi, "fi");

            if (fi.IsStatic)
            {
                il.Emit(OpCodes.Stsfld, fi);
            }
            else
            {
                il.Emit(OpCodes.Stfld, fi);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        public static void EmitNew(ILGenerator il, ConstructorInfo ci)
        {
            Require.NotNull(ci, "ci");
            Require.That(!ci.DeclaringType.ContainsGenericParameters, "Illegal new generic params");

            il.Emit(OpCodes.Newobj, ci);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
        public static void EmitNew(ILGenerator il, Type type, Type[] paramTypes)
        {
            Require.NotNull(type, "type");
            Require.NotNull(paramTypes, "paramTypes");

            ConstructorInfo ci = type.GetConstructor(paramTypes);
            if (ci == null)
                throw new InvalidOperationException("Type does not have the correct constructor");

            EmitNew(il, ci);
        }

        public static void EmitNull(ILGenerator il)
        {
            il.Emit(OpCodes.Ldnull);
        }

        public static void EmitString(ILGenerator il, string value)
        {
            Require.NotNull(value, "value");

            il.Emit(OpCodes.Ldstr, value);
        }

        public static void EmitBoolean(ILGenerator il, bool value)
        {
            if (value)
            {
                il.Emit(OpCodes.Ldc_I4_1);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4_0);
            }
        }

        public static void EmitChar(ILGenerator il, char value)
        {
            EmitInt(il, value);
            il.Emit(OpCodes.Conv_U2);
        }

        public static void EmitByte(ILGenerator il, byte value)
        {
            EmitInt(il, value);
            il.Emit(OpCodes.Conv_U1);
        }

        public static void EmitSByte(ILGenerator il, sbyte value)
        {
            EmitInt(il, value);
            il.Emit(OpCodes.Conv_I1);
        }

        public static void EmitShort(ILGenerator il, short value)
        {
            EmitInt(il, value);
            il.Emit(OpCodes.Conv_I2);
        }

        public static void EmitUShort(ILGenerator il, ushort value)
        {
            EmitInt(il, value);
            il.Emit(OpCodes.Conv_U2);
        }

        public static void EmitInt(ILGenerator il, int value)
        {
            OpCode c;
            switch (value)
            {
                case -1:
                    c = OpCodes.Ldc_I4_M1;
                    break;
                case 0:
                    c = OpCodes.Ldc_I4_0;
                    break;
                case 1:
                    c = OpCodes.Ldc_I4_1;
                    break;
                case 2:
                    c = OpCodes.Ldc_I4_2;
                    break;
                case 3:
                    c = OpCodes.Ldc_I4_3;
                    break;
                case 4:
                    c = OpCodes.Ldc_I4_4;
                    break;
                case 5:
                    c = OpCodes.Ldc_I4_5;
                    break;
                case 6:
                    c = OpCodes.Ldc_I4_6;
                    break;
                case 7:
                    c = OpCodes.Ldc_I4_7;
                    break;
                case 8:
                    c = OpCodes.Ldc_I4_8;
                    break;
                default:
                    if (value >= -128 && value <= 127)
                    {
                        il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldc_I4, value);
                    }
                    return;
            }
            il.Emit(c);
        }

        public static void EmitUInt(ILGenerator il, uint value)
        {
            EmitInt(il, (int)value);
            il.Emit(OpCodes.Conv_U4);
        }

        public static void EmitLong(ILGenerator il, long value)
        {
            il.Emit(OpCodes.Ldc_I8, value);

            //
            // Now, emit convert to give the constant type information.
            //
            // Otherwise, it is treated as unsigned and overflow is not
            // detected if it's used in checked ops.
            //
            il.Emit(OpCodes.Conv_I8);
        }

        public static void EmitULong(ILGenerator il, ulong value)
        {
            il.Emit(OpCodes.Ldc_I8, (long)value);
            il.Emit(OpCodes.Conv_U8);
        }

        public static void EmitDouble(ILGenerator il, double value)
        {
            il.Emit(OpCodes.Ldc_R8, value);
        }

        public static void EmitSingle(ILGenerator il, float value)
        {
            il.Emit(OpCodes.Ldc_R4, value);
        }

        // matches TryEmitConstant
        public static bool CanEmitConstant(object value, Type type)
        {
            if (value == null || CanEmitILConstant(type))
            {
                return true;
            }

            Type t = value as Type;
            if (t != null && ShouldLdtoken(t))
            {
                return true;
            }

            MethodBase mb = value as MethodBase;
            if (mb != null && ShouldLdtoken(mb))
            {
                return true;
            }

            return false;
        }

        // matches TryEmitILConstant
        private static bool CanEmitILConstant(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
            }
            return false;
        }

        public static void EmitConstant(ILGenerator il, object value)
        {
            Debug.Assert(value != null);
            EmitConstant(il, value, value.GetType());
        }


        //
        // Note: we support emitting more things as IL constants than
        // Linq does
        public static void EmitConstant(ILGenerator il, object value, Type type)
        {
            if (value == null)
            {
                // Smarter than the Linq implementation which uses the initobj
                // pattern for all value types (works, but requires a local and
                // more IL)
                EmitDefault(il, type);
                return;
            }

            // Handle the easy cases
            if (TryEmitILConstant(il, value, type))
            {
                return;
            }

            // Check for a few more types that we support emitting as constants
            Type t = value as Type;
            if (t != null && ShouldLdtoken(t))
            {
                EmitType(il, t);
                if (type != typeof(Type))
                {
                    il.Emit(OpCodes.Castclass, type);
                }
                return;
            }

            MethodBase mb = value as MethodBase;
            if (mb != null && ShouldLdtoken(mb))
            {
                Emit(il, OpCodes.Ldtoken, mb);
                Type dt = mb.DeclaringType;
                if (dt != null && dt.IsGenericType)
                {
                    il.Emit(OpCodes.Ldtoken, dt);
                    il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }));
                }
                else
                {
                    il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle) }));
                }
                if (type != typeof(MethodBase))
                {
                    il.Emit(OpCodes.Castclass, type);
                }
                return;
            }

            throw new InvalidOperationException();
        }

        public static bool ShouldLdtoken(Type t)
        {
            return t is TypeBuilder || t.IsGenericParameter || t.IsVisible;
        }

        public static bool ShouldLdtoken(MethodBase mb)
        {
            // Can't ldtoken on a DynamicMethod
            if (mb is DynamicMethod)
            {
                return false;
            }

            Type dt = mb.DeclaringType;
            return dt == null || ShouldLdtoken(dt);
        }


        private static bool TryEmitILConstant(ILGenerator il, object value, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    EmitBoolean(il, (bool)value);
                    return true;
                case TypeCode.SByte:
                    EmitSByte(il, (sbyte)value);
                    return true;
                case TypeCode.Int16:
                    EmitShort(il, (short)value);
                    return true;
                case TypeCode.Int32:
                    EmitInt(il, (int)value);
                    return true;
                case TypeCode.Int64:
                    EmitLong(il, (long)value);
                    return true;
                case TypeCode.Single:
                    EmitSingle(il, (float)value);
                    return true;
                case TypeCode.Double:
                    EmitDouble(il, (double)value);
                    return true;
                case TypeCode.Char:
                    EmitChar(il, (char)value);
                    return true;
                case TypeCode.Byte:
                    EmitByte(il, (byte)value);
                    return true;
                case TypeCode.UInt16:
                    EmitUShort(il, (ushort)value);
                    return true;
                case TypeCode.UInt32:
                    EmitUInt(il, (uint)value);
                    return true;
                case TypeCode.UInt64:
                    EmitULong(il, (ulong)value);
                    return true;
                case TypeCode.Decimal:
                    EmitDecimal(il, (decimal)value);
                    return true;
                case TypeCode.String:
                    EmitString(il, (string)value);
                    return true;
                default:
                    return false;
            }
        }

        public static void EmitConvertToType(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            if (TypeUtils.AreEquivalent(typeFrom, typeTo))
            {
                return;
            }

            if (typeFrom == typeof(void) || typeTo == typeof(void))
            {
                throw new InvalidOperationException();
            }

            bool isTypeFromNullable = TypeUtils.IsNullableType(typeFrom);
            bool isTypeToNullable = TypeUtils.IsNullableType(typeTo);

            Type nnExprType = TypeUtils.GetNonNullableType(typeFrom);
            Type nnType = TypeUtils.GetNonNullableType(typeTo);

            if (typeFrom.IsInterface || // interface cast
               typeTo.IsInterface ||
               typeFrom == typeof(object) || // boxing cast
               typeTo == typeof(object) ||
               typeFrom == typeof(System.Enum) ||
               typeFrom == typeof(System.ValueType) ||
               TypeUtils.IsLegalExplicitVariantDelegateConversion(typeFrom, typeTo))
            {
                EmitCastToType(il, typeFrom, typeTo);
            }
            else if (isTypeFromNullable || isTypeToNullable)
            {
                EmitNullableConversion(il, typeFrom, typeTo, isChecked);
            }
            else if (!(TypeUtils.IsConvertible(typeFrom) && TypeUtils.IsConvertible(typeTo)) // primitive runtime conversion
                     &&
                     (nnExprType.IsAssignableFrom(nnType) || // down cast
                     nnType.IsAssignableFrom(nnExprType))) // up cast
            {
                EmitCastToType(il, typeFrom, typeTo);
            }
            else if (typeFrom.IsArray && typeTo.IsArray)
            {
                // See DevDiv Bugs #94657.
                EmitCastToType(il, typeFrom, typeTo);
            }
            else
            {
                EmitNumericConversion(il, typeFrom, typeTo, isChecked);
            }
        }


        private static void EmitCastToType(ILGenerator il, Type typeFrom, Type typeTo)
        {
            if (!typeFrom.IsValueType && typeTo.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, typeTo);
            }
            else if (typeFrom.IsValueType && !typeTo.IsValueType)
            {
                il.Emit(OpCodes.Box, typeFrom);
                if (typeTo != typeof(object))
                {
                    il.Emit(OpCodes.Castclass, typeTo);
                }
            }
            else if (!typeFrom.IsValueType && !typeTo.IsValueType)
            {
                il.Emit(OpCodes.Castclass, typeTo);
            }
            else
            {
                throw new NotSupportedException("Invalid cast");
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static void EmitNumericConversion(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool isFromUnsigned = TypeUtils.IsUnsigned(typeFrom);
            bool isFromFloatingPoint = TypeUtils.IsFloatingPoint(typeFrom);
            if (typeTo == typeof(Single))
            {
                if (isFromUnsigned)
                    il.Emit(OpCodes.Conv_R_Un);
                il.Emit(OpCodes.Conv_R4);
            }
            else if (typeTo == typeof(Double))
            {
                if (isFromUnsigned)
                    il.Emit(OpCodes.Conv_R_Un);
                il.Emit(OpCodes.Conv_R8);
            }
            else
            {
                TypeCode tc = Type.GetTypeCode(typeTo);
                if (isChecked)
                {
                    // Overflow checking needs to know if the source value on the IL stack is unsigned or not.
                    if (isFromUnsigned)
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                                il.Emit(OpCodes.Conv_Ovf_I1_Un);
                                break;
                            case TypeCode.Int16:
                                il.Emit(OpCodes.Conv_Ovf_I2_Un);
                                break;
                            case TypeCode.Int32:
                                il.Emit(OpCodes.Conv_Ovf_I4_Un);
                                break;
                            case TypeCode.Int64:
                                il.Emit(OpCodes.Conv_Ovf_I8_Un);
                                break;
                            case TypeCode.Byte:
                                il.Emit(OpCodes.Conv_Ovf_U1_Un);
                                break;
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                il.Emit(OpCodes.Conv_Ovf_U2_Un);
                                break;
                            case TypeCode.UInt32:
                                il.Emit(OpCodes.Conv_Ovf_U4_Un);
                                break;
                            case TypeCode.UInt64:
                                il.Emit(OpCodes.Conv_Ovf_U8_Un);
                                break;
                            default:
                                throw new InvalidOperationException("Unhandled convert");
                        }
                    }
                    else
                    {
                        switch (tc)
                        {
                            case TypeCode.SByte:
                                il.Emit(OpCodes.Conv_Ovf_I1);
                                break;
                            case TypeCode.Int16:
                                il.Emit(OpCodes.Conv_Ovf_I2);
                                break;
                            case TypeCode.Int32:
                                il.Emit(OpCodes.Conv_Ovf_I4);
                                break;
                            case TypeCode.Int64:
                                il.Emit(OpCodes.Conv_Ovf_I8);
                                break;
                            case TypeCode.Byte:
                                il.Emit(OpCodes.Conv_Ovf_U1);
                                break;
                            case TypeCode.UInt16:
                            case TypeCode.Char:
                                il.Emit(OpCodes.Conv_Ovf_U2);
                                break;
                            case TypeCode.UInt32:
                                il.Emit(OpCodes.Conv_Ovf_U4);
                                break;
                            case TypeCode.UInt64:
                                il.Emit(OpCodes.Conv_Ovf_U8);
                                break;
                            default:
                                throw new InvalidOperationException("Unhandled convert");
                        }
                    }
                }
                else
                {
                    switch (tc)
                    {
                        case TypeCode.SByte:
                            il.Emit(OpCodes.Conv_I1);
                            break;
                        case TypeCode.Byte:
                            il.Emit(OpCodes.Conv_U1);
                            break;
                        case TypeCode.Int16:
                            il.Emit(OpCodes.Conv_I2);
                            break;
                        case TypeCode.UInt16:
                        case TypeCode.Char:
                            il.Emit(OpCodes.Conv_U2);
                            break;
                        case TypeCode.Int32:
                            il.Emit(OpCodes.Conv_I4);
                            break;
                        case TypeCode.UInt32:
                            il.Emit(OpCodes.Conv_U4);
                            break;
                        case TypeCode.Int64:
                            if (isFromUnsigned)
                            {
                                il.Emit(OpCodes.Conv_U8);
                            }
                            else
                            {
                                il.Emit(OpCodes.Conv_I8);
                            }
                            break;
                        case TypeCode.UInt64:
                            if (isFromUnsigned || isFromFloatingPoint)
                            {
                                il.Emit(OpCodes.Conv_U8);
                            }
                            else
                            {
                                il.Emit(OpCodes.Conv_I8);
                            }
                            break;
                        default:
                            throw new InvalidOperationException("Unhandled convert");
                    }
                }
            }
        }

        private static void EmitNullableToNullableConversion(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(TypeUtils.IsNullableType(typeTo));
            LocalBuilder locFrom = il.DeclareLocal(typeFrom);
            il.Emit(OpCodes.Stloc, locFrom);
            LocalBuilder locTo = il.DeclareLocal(typeTo);
            // test for null
            il.Emit(OpCodes.Ldloca, locFrom);
            EmitHasValue(il, typeFrom);
            Label labIfNull = il.DefineLabel();
            il.Emit(OpCodes.Brfalse_S, labIfNull);
            il.Emit(OpCodes.Ldloca, locFrom);
            EmitGetValueOrDefault(il, typeFrom);
            Type nnTypeFrom = TypeUtils.GetNonNullableType(typeFrom);
            Type nnTypeTo = TypeUtils.GetNonNullableType(typeTo);
            EmitConvertToType(il, nnTypeFrom, nnTypeTo, isChecked);
            // construct result type
            ConstructorInfo ci = typeTo.GetConstructor(new[] { nnTypeTo });
            il.Emit(OpCodes.Newobj, ci);
            il.Emit(OpCodes.Stloc, locTo);
            Label labEnd = il.DefineLabel();
            il.Emit(OpCodes.Br_S, labEnd);
            // if null then create a default one
            il.MarkLabel(labIfNull);
            il.Emit(OpCodes.Ldloca, locTo);
            il.Emit(OpCodes.Initobj, typeTo);
            il.MarkLabel(labEnd);
            il.Emit(OpCodes.Ldloc, locTo);
        }


        private static void EmitNonNullableToNullableConversion(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(!TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(TypeUtils.IsNullableType(typeTo));
            LocalBuilder locTo = il.DeclareLocal(typeTo);
            Type nnTypeTo = TypeUtils.GetNonNullableType(typeTo);
            EmitConvertToType(il, typeFrom, nnTypeTo, isChecked);
            ConstructorInfo ci = typeTo.GetConstructor(new[] { nnTypeTo });
            il.Emit(OpCodes.Newobj, ci);
            il.Emit(OpCodes.Stloc, locTo);
            il.Emit(OpCodes.Ldloc, locTo);
        }


        private static void EmitNullableToNonNullableConversion(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(!TypeUtils.IsNullableType(typeTo));
            if (typeTo.IsValueType)
                EmitNullableToNonNullableStructConversion(il, typeFrom, typeTo, isChecked);
            else
                EmitNullableToReferenceConversion(il, typeFrom);
        }


        private static void EmitNullableToNonNullableStructConversion(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            Debug.Assert(!TypeUtils.IsNullableType(typeTo));
            Debug.Assert(typeTo.IsValueType);
            LocalBuilder locFrom = il.DeclareLocal(typeFrom);
            il.Emit(OpCodes.Stloc, locFrom);
            il.Emit(OpCodes.Ldloca, locFrom);
            EmitGetValue(il, typeFrom);
            Type nnTypeFrom = TypeUtils.GetNonNullableType(typeFrom);
            EmitConvertToType(il, nnTypeFrom, typeTo, isChecked);
        }


        private static void EmitNullableToReferenceConversion(ILGenerator il, Type typeFrom)
        {
            Debug.Assert(TypeUtils.IsNullableType(typeFrom));
            // We've got a conversion from nullable to Object, ValueType, Enum, etc.  Just box it so that
            // we get the nullable semantics.  
            il.Emit(OpCodes.Box, typeFrom);
        }


        private static void EmitNullableConversion(ILGenerator il, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool isTypeFromNullable = TypeUtils.IsNullableType(typeFrom);
            bool isTypeToNullable = TypeUtils.IsNullableType(typeTo);
            Debug.Assert(isTypeFromNullable || isTypeToNullable);
            if (isTypeFromNullable && isTypeToNullable)
                EmitNullableToNullableConversion(il, typeFrom, typeTo, isChecked);
            else if (isTypeFromNullable)
                EmitNullableToNonNullableConversion(il, typeFrom, typeTo, isChecked);
            else
                EmitNonNullableToNullableConversion(il, typeFrom, typeTo, isChecked);
        }


        public static void EmitHasValue(ILGenerator il, Type nullableType)
        {
            MethodInfo mi = nullableType.GetMethod("get_HasValue", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(nullableType.IsValueType);
            il.Emit(OpCodes.Call, mi);
        }


        public static void EmitGetValue(ILGenerator il, Type nullableType)
        {
            MethodInfo mi = nullableType.GetMethod("get_Value", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(nullableType.IsValueType);
            il.Emit(OpCodes.Call, mi);
        }


        public static void EmitGetValueOrDefault(ILGenerator il, Type nullableType)
        {
            MethodInfo mi = nullableType.GetMethod("GetValueOrDefault", System.Type.EmptyTypes);
            Debug.Assert(nullableType.IsValueType);
            il.Emit(OpCodes.Call, mi);
        }

        /// <summary>
        /// Emits an array of constant values provided in the given list.
        /// The array is strongly typed.
        /// </summary>
        public static void EmitArray<T>(ILGenerator il, IList<T> items)
        {
            Require.NotNull(items, "items");

            EmitInt(il, items.Count);
            il.Emit(OpCodes.Newarr, typeof(T));
            for (int i = 0; i < items.Count; i++)
            {
                il.Emit(OpCodes.Dup);
                EmitInt(il, i);
                EmitConstant(il, items[i], typeof(T));
                EmitStoreElement(il, typeof(T));
            }
        }

        /// <summary>
        /// Emits an array of values of count size.  The items are emitted via the callback
        /// which is provided with the current item index to emit.
        /// </summary>
        public static void EmitArray(ILGenerator il, Type elementType, int count, Action<int> emit)
        {
            Require.NotNull(elementType, "elementType");
            Require.NotNull(emit, "emit");
            Require.That(count >= 0, "Cannot be negative", "count");

            EmitInt(il, count);
            il.Emit(OpCodes.Newarr, elementType);
            for (int i = 0; i < count; i++)
            {
                il.Emit(OpCodes.Dup);
                EmitInt(il, i);

                emit(i);

                EmitStoreElement(il, elementType);
            }
        }

        /// <summary>
        /// Emits an array construction code.  
        /// The code assumes that bounds for all dimensions
        /// are already emitted.
        /// </summary>
        public static void EmitArray(ILGenerator il, Type arrayType)
        {
            Require.NotNull(arrayType, "arrayType");
            Require.That(arrayType.IsArray, "Must be array", "arrayType");

            int rank = arrayType.GetArrayRank();
            if (rank == 1)
            {
                il.Emit(OpCodes.Newarr, arrayType.GetElementType());
            }
            else
            {
                Type[] types = new Type[rank];
                for (int i = 0; i < rank; i++)
                {
                    types[i] = typeof(int);
                }
                EmitNew(il, arrayType, types);
            }
        }

        public static void EmitDecimal(ILGenerator il, decimal value)
        {
            if (Decimal.Truncate(value) == value)
            {
                if (Int32.MinValue <= value && value <= Int32.MaxValue)
                {
                    int intValue = Decimal.ToInt32(value);
                    EmitInt(il, intValue);
                    EmitNew(il, typeof(Decimal).GetConstructor(new[] { typeof(int) }));
                }
                else if (Int64.MinValue <= value && value <= Int64.MaxValue)
                {
                    long longValue = Decimal.ToInt64(value);
                    EmitLong(il, longValue);
                    EmitNew(il, typeof(Decimal).GetConstructor(new[] { typeof(long) }));
                }
                else
                {
                    EmitDecimalBits(il, value);
                }
            }
            else
            {
                EmitDecimalBits(il, value);
            }
        }

        private static void EmitDecimalBits(ILGenerator il, decimal value)
        {
            int[] bits = Decimal.GetBits(value);
            EmitInt(il, bits[0]);
            EmitInt(il, bits[1]);
            EmitInt(il, bits[2]);
            EmitBoolean(il, (bits[3] & 0x80000000) != 0);
            EmitByte(il, (byte)(bits[3] >> 16));
            EmitNew(il, typeof(decimal).GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) }));
        }

        /// <summary>
        /// Emits default(T)
        /// Semantics match C# compiler behavior
        /// </summary>
        public static void EmitDefault(ILGenerator il, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                case TypeCode.DateTime:
                    if (type.IsValueType)
                    {
                        // Type.GetTypeCode on an enum returns the underlying
                        // integer TypeCode, so we won't get here.
                        Debug.Assert(!type.IsEnum);

                        // This is the IL for default(T) if T is a generic type
                        // parameter, so it should work for any type. It's also
                        // the standard pattern for structs.
                        LocalBuilder lb = il.DeclareLocal(type);
                        il.Emit(OpCodes.Ldloca, lb);
                        il.Emit(OpCodes.Initobj, type);
                        il.Emit(OpCodes.Ldloc, lb);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldnull);
                    }
                    break;

                case TypeCode.Empty:
                case TypeCode.String:
                case TypeCode.DBNull:
                    il.Emit(OpCodes.Ldnull);
                    break;

                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    il.Emit(OpCodes.Ldc_I4_0);
                    break;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Conv_I8);
                    break;

                case TypeCode.Single:
                    il.Emit(OpCodes.Ldc_R4, default(Single));
                    break;

                case TypeCode.Double:
                    il.Emit(OpCodes.Ldc_R8, default(Double));
                    break;

                case TypeCode.Decimal:
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new[] { typeof(int) }));
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
