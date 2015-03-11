/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
﻿using System.Diagnostics;
﻿using System.Reflection;
﻿using System.Text;

namespace Expressions
{
    internal static class TypeUtil
    {
        private const BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Dictionary<Type, IList<Type>> ImplicitCastingTable = new Dictionary<Type, IList<Type>>
        {
            { typeof(char), new ReadOnlyCollection<Type>(new[] { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(sbyte), new ReadOnlyCollection<Type>(new[] { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(byte), new ReadOnlyCollection<Type>(new[] { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(short), new ReadOnlyCollection<Type>(new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(ushort), new ReadOnlyCollection<Type>(new[] { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(int), new ReadOnlyCollection<Type>(new[] { typeof(long), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(uint), new ReadOnlyCollection<Type>(new[] { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(long), new ReadOnlyCollection<Type>(new[] { typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(ulong), new ReadOnlyCollection<Type>(new[] { typeof(float), typeof(double), typeof(decimal) }) },
            { typeof(float), new ReadOnlyCollection<Type>(new[] { typeof(double) }) },
            { typeof(double), new Type[0] },
            { typeof(decimal), new Type[0] }
        };

        private static readonly Dictionary<string, Type> FleeBuiltInTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "object", typeof(object) },
            { "char", typeof(char) },
            { "sbyte", typeof(sbyte) },
            { "byte", typeof(byte) },
            { "short", typeof(short) },
            { "ushort", typeof(ushort) },
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "long", typeof(long) },
            { "ulong", typeof(ulong) },
            { "single", typeof(float) },
            { "double", typeof(double) },
            { "decimal", typeof(decimal) },
            { "boolean", typeof(bool) },
            { "string", typeof(string) }
        };

        private static readonly Dictionary<string, Type> CsharpBuiltInTypes = new Dictionary<string, Type>
        {
            { "object", typeof(object) },
            { "char", typeof(char) },
            { "sbyte", typeof(sbyte) },
            { "byte", typeof(byte) },
            { "short", typeof(short) },
            { "ushort", typeof(ushort) },
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "long", typeof(long) },
            { "ulong", typeof(ulong) },
            { "float", typeof(float) },
            { "double", typeof(double) },
            { "decimal", typeof(decimal) },
            { "bool", typeof(bool) },
            { "string", typeof(string) }
        };

        private static readonly Dictionary<string, Type> VisualBasicBuiltInTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "Boolean", typeof(bool) },
            { "Byte", typeof(byte) },
            { "Char", typeof(char) },
            { "Date", typeof(DateTime) },
            { "Decimal", typeof(decimal) },
            { "Double", typeof(double) },
            { "Integer", typeof(int) },
            { "Long", typeof(long) },
            { "Object", typeof(object) },
            { "SByte", typeof(sbyte) },
            { "Short", typeof(short) },
            { "Single", typeof(float) },
            { "String", typeof(string) },
            { "UInteger", typeof(uint) },
            { "ULong", typeof(ulong) },
            { "UShort", typeof(ushort) }
        };

        public static IList<Type> GetImplicitCastingTable(Type type)
        {
            Require.NotNull(type, "type");

            IList<Type> result;

            ImplicitCastingTable.TryGetValue(type, out result);

            return result;
        }

        public static bool CanCastImplicitely(Type fromType, Type toType, bool toIsNull)
        {
            if (
                toType == typeof(object) ||
                toType == fromType
            )
                return true;

            var castingTable = GetImplicitCastingTable(fromType);

            if (castingTable != null)
            {
                for (int j = 0; j < castingTable.Count; j++)
                {
                    if (castingTable[j] == toType)
                        return true;
                }

                return false;
            }
            else
            {
                return 
                    toType.IsAssignableFrom(fromType) ||
                    (!toType.IsValueType && toIsNull);
            }
        }

        public static object CastImplicitely(object value, Type toType)
        {
            if (!CanCastImplicitely(value == null ? typeof(object) : value.GetType(), toType, value == null))
                throw new NotSupportedException("Cannot cast");

            if (value == null || !toType.IsValueType)
                return value;

            Debug.Assert(GetImplicitCastingTable(value.GetType()).Contains(toType));

            return Convert.ChangeType(value, toType);
        }

        public static bool IsValidUnaryArgument(Type type)
        {
            Require.NotNull(type, "type");

            return ImplicitCastingTable.ContainsKey(type);
        }

        public static Type GetBuiltInType(string name, ExpressionLanguage language)
        {
            Require.NotNull(name, "name");

            Type result;

            switch (language)
            {
                case ExpressionLanguage.Flee:
                    FleeBuiltInTypes.TryGetValue(name, out result);
                    break;

                case ExpressionLanguage.Csharp:
                    CsharpBuiltInTypes.TryGetValue(name, out result);
                    break;

                case ExpressionLanguage.VisualBasic:
                    VisualBasicBuiltInTypes.TryGetValue(name, out result);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("language");
            }

            return result;
        }

        public static bool IsCastAllowed(Type type, Type targetType)
        {
            Require.NotNull(type, "type");
            Require.NotNull(targetType, "targetType");

            if (type == targetType)
                return true;

            IList<Type> types;

            return
                ImplicitCastingTable.TryGetValue(type, out types) &&
                types.Contains(targetType);
        }

        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        private static Type GetNullableType(Type type)
        {
            Debug.Assert(type != null, "type cannot be null");
            if (type.IsValueType && !IsNullableType(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool IsBool(Type type)
        {
            return GetNonNullableType(type) == typeof(bool);
        }

        public static bool IsNumeric(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        public static bool IsInteger(Type type)
        {
            type = GetNonNullableType(type);

            if (type.IsEnum)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;

                default:
                    return false;
            }
        }


        private static bool IsArithmetic(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Double:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        private static bool IsUnsignedInt(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        private static bool IsIntegerOrBool(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Int64:
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.UInt64:
                    case TypeCode.UInt32:
                    case TypeCode.UInt16:
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        return true;
                }
            }
            return false;
        }

        public static bool AreEquivalent(Type t1, Type t2)
        {
            return t1 == t2;
        }

        private static bool AreReferenceAssignable(Type dest, Type src)
        {
            // WARNING: This actually implements "Is this identity assignable and/or reference assignable?"
            if (AreEquivalent(dest, src))
            {
                return true;
            }
            if (!dest.IsValueType && !src.IsValueType && dest.IsAssignableFrom(src))
            {
                return true;
            }
            return false;
        }

        // Checks if the type is a valid target for an instance call

        private static bool IsValidInstanceType(MemberInfo member, Type instanceType)
        {
            Type targetType = member.DeclaringType;
            if (AreReferenceAssignable(targetType, instanceType))
            {
                return true;
            }
            if (instanceType.IsValueType)
            {
                if (AreReferenceAssignable(targetType, typeof(System.Object)))
                {
                    return true;
                }
                if (AreReferenceAssignable(targetType, typeof(System.ValueType)))
                {
                    return true;
                }
                if (instanceType.IsEnum && AreReferenceAssignable(targetType, typeof(System.Enum)))
                {
                    return true;
                }
                // A call to an interface implemented by a struct is legal whether the struct has
                // been boxed or not.
                if (targetType.IsInterface)
                {
                    foreach (Type interfaceType in instanceType.GetInterfaces())
                    {
                        if (AreReferenceAssignable(targetType, interfaceType))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool HasIdentityPrimitiveOrNullableConversion(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // Identity conversion
            if (AreEquivalent(source, dest))
            {
                return true;
            }

            // Nullable conversions
            if (IsNullableType(source) && AreEquivalent(dest, GetNonNullableType(source)))
            {
                return true;
            }
            if (IsNullableType(dest) && AreEquivalent(source, GetNonNullableType(dest)))
            {
                return true;
            }
            // Primitive runtime conversions
            // All conversions amongst enum, bool, char, integer and float types
            // (and their corresponding nullable types) are legal except for
            // nonbool==>bool and nonbool==>bool?
            // Since we have already covered bool==>bool, bool==>bool?, etc, above,
            // we can just disallow having a bool or bool? destination type here.
            if (IsConvertible(source) && IsConvertible(dest) && GetNonNullableType(dest) != typeof(bool))
            {
                return true;
            }
            return false;
        }

        private static bool HasReferenceConversion(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // void -> void conversion is handled elsewhere
            // (it's an identity conversion)
            // All other void conversions are disallowed.
            if (source == typeof(void) || dest == typeof(void))
            {
                return false;
            }

            Type nnSourceType = GetNonNullableType(source);
            Type nnDestType = GetNonNullableType(dest);

            // Down conversion
            if (nnSourceType.IsAssignableFrom(nnDestType))
            {
                return true;
            }
            // Up conversion
            if (nnDestType.IsAssignableFrom(nnSourceType))
            {
                return true;
            }
            // Interface conversion
            if (source.IsInterface || dest.IsInterface)
            {
                return true;
            }
            // Variant delegate conversion
            if (IsLegalExplicitVariantDelegateConversion(source, dest))
                return true;

            // Object conversion
            if (source == typeof(object) || dest == typeof(object))
            {
                return true;
            }
            return false;
        }

        private static bool IsCovariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Covariant);
        }

        private static bool IsContravariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 != (t.GenericParameterAttributes & GenericParameterAttributes.Contravariant);
        }

        private static bool IsInvariant(Type t)
        {
            Debug.Assert(t != null);
            return 0 == (t.GenericParameterAttributes & GenericParameterAttributes.VarianceMask);
        }

        private static bool IsDelegate(Type t)
        {
            Debug.Assert(t != null);
            return t.IsSubclassOf(typeof(System.MulticastDelegate));
        }

        public static bool IsLegalExplicitVariantDelegateConversion(Type source, Type dest)
        {
            Debug.Assert(source != null && dest != null);

            // There *might* be a legal conversion from a generic delegate type S to generic delegate type  T, 
            // provided all of the follow are true:
            //   o Both types are constructed generic types of the same generic delegate type, D<X1,... Xk>.
            //     That is, S = D<S1...>, T = D<T1...>.
            //   o If type parameter Xi is declared to be invariant then Si must be identical to Ti.
            //   o If type parameter Xi is declared to be covariant ("out") then Si must be convertible
            //     to Ti via an identify conversion,  implicit reference conversion, or explicit reference conversion.
            //   o If type parameter Xi is declared to be contravariant ("in") then either Si must be identical to Ti, 
            //     or Si and Ti must both be reference types.

            if (!IsDelegate(source) || !IsDelegate(dest) || !source.IsGenericType || !dest.IsGenericType)
                return false;

            Type genericDelegate = source.GetGenericTypeDefinition();

            if (dest.GetGenericTypeDefinition() != genericDelegate)
                return false;

            Type[] genericParameters = genericDelegate.GetGenericArguments();
            Type[] sourceArguments = source.GetGenericArguments();
            Type[] destArguments = dest.GetGenericArguments();

            Debug.Assert(genericParameters != null);
            Debug.Assert(sourceArguments != null);
            Debug.Assert(destArguments != null);
            Debug.Assert(genericParameters.Length == sourceArguments.Length);
            Debug.Assert(genericParameters.Length == destArguments.Length);

            for (int iParam = 0; iParam < genericParameters.Length; ++iParam)
            {
                Type sourceArgument = sourceArguments[iParam];
                Type destArgument = destArguments[iParam];

                Debug.Assert(sourceArgument != null && destArgument != null);

                // If the arguments are identical then this one is automatically good, so skip it.
                if (AreEquivalent(sourceArgument, destArgument))
                {
                    continue;
                }

                Type genericParameter = genericParameters[iParam];

                Debug.Assert(genericParameter != null);

                if (IsInvariant(genericParameter))
                {
                    return false;
                }

                if (IsCovariant(genericParameter))
                {
                    if (!HasReferenceConversion(sourceArgument, destArgument))
                    {
                        return false;
                    }
                }
                else if (IsContravariant(genericParameter))
                {
                    if (sourceArgument.IsValueType || destArgument.IsValueType)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsConvertible(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return true;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Char:
                    return true;
                default:
                    return false;
            }
        }

        private static bool HasReferenceEquality(Type left, Type right)
        {
            if (left.IsValueType || right.IsValueType)
            {
                return false;
            }

            // If we have an interface and a reference type then we can do 
            // reference equality.

            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.

            return left.IsInterface || right.IsInterface ||
                AreReferenceAssignable(left, right) ||
                    AreReferenceAssignable(right, left);
        }

        private static bool HasBuiltInEqualityOperator(Type left, Type right)
        {
            // If we have an interface and a reference type then we can do 
            // reference equality.
            if (left.IsInterface && !right.IsValueType)
            {
                return true;
            }
            if (right.IsInterface && !left.IsValueType)
            {
                return true;
            }
            // If we have two reference types and one is assignable to the
            // other then we can do reference equality.
            if (!left.IsValueType && !right.IsValueType)
            {
                if (AreReferenceAssignable(left, right) || AreReferenceAssignable(right, left))
                {
                    return true;
                }
            }
            // Otherwise, if the types are not the same then we definitely 
            // do not have a built-in equality operator.
            if (!AreEquivalent(left, right))
            {
                return false;
            }
            // We have two identical value types, modulo nullability.  (If they were both the 
            // same reference type then we would have returned true earlier.)
            Debug.Assert(left.IsValueType);
            // Equality between struct types is only defined for numerics, bools, enums,
            // and their nullable equivalents.
            Type nnType = GetNonNullableType(left);
            if (nnType == typeof(bool) || IsNumeric(nnType) || nnType.IsEnum)
            {
                return true;
            }
            return false;
        }

        private static bool IsImplicitlyConvertible(Type source, Type destination)
        {
            return AreEquivalent(source, destination) ||                // identity conversion
                IsImplicitNumericConversion(source, destination) ||
                    IsImplicitReferenceConversion(source, destination) ||
                        IsImplicitBoxingConversion(source, destination) ||
                            IsImplicitNullableConversion(source, destination);
        }


        private static MethodInfo GetUserDefinedCoercionMethod(Type convertFrom, Type convertToType, bool implicitOnly)
        {
            // check for implicit coercions first
            Type nnExprType = GetNonNullableType(convertFrom);
            Type nnConvType = GetNonNullableType(convertToType);
            // try exact match on types
            MethodInfo[] eMethods = nnExprType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo method = FindConversionOperator(eMethods, convertFrom, convertToType, implicitOnly);
            if (method != null)
            {
                return method;
            }
            MethodInfo[] cMethods = nnConvType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            method = FindConversionOperator(cMethods, convertFrom, convertToType, implicitOnly);
            if (method != null)
            {
                return method;
            }
            // try lifted conversion
            if (!AreEquivalent(nnExprType, convertFrom) ||
                !AreEquivalent(nnConvType, convertToType))
            {
                method = FindConversionOperator(eMethods, nnExprType, nnConvType, implicitOnly);
                if (method == null)
                {
                    method = FindConversionOperator(cMethods, nnExprType, nnConvType, implicitOnly);
                }
                if (method != null)
                {
                    return method;
                }
            }
            return null;
        }

        private static MethodInfo FindConversionOperator(MethodInfo[] methods, Type typeFrom, Type typeTo, bool implicitOnly)
        {
            foreach (MethodInfo mi in methods)
            {
                if (mi.Name != "op_Implicit" && (implicitOnly || mi.Name != "op_Explicit"))
                {
                    continue;
                }
                if (!AreEquivalent(mi.ReturnType, typeTo))
                {
                    continue;
                }
                ParameterInfo[] pis = mi.GetParameters();
                if (!AreEquivalent(pis[0].ParameterType, typeFrom))
                {
                    continue;
                }
                return mi;
            }
            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static bool IsImplicitNumericConversion(Type source, Type destination)
        {
            TypeCode tcSource = Type.GetTypeCode(source);
            TypeCode tcDest = Type.GetTypeCode(destination);

            switch (tcSource)
            {
                case TypeCode.SByte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Byte:
                    switch (tcDest)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.UInt16:
                    switch (tcDest)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int32:
                    switch (tcDest)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.UInt32:
                    switch (tcDest)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (tcDest)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Char:
                    switch (tcDest)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;
                case TypeCode.Single:
                    return (tcDest == TypeCode.Double);
            }
            return false;
        }

        private static bool IsImplicitReferenceConversion(Type source, Type destination)
        {
            return destination.IsAssignableFrom(source);
        }

        private static bool IsImplicitBoxingConversion(Type source, Type destination)
        {
            if (source.IsValueType && (destination == typeof(object) || destination == typeof(System.ValueType)))
                return true;
            if (source.IsEnum && destination == typeof(System.Enum))
                return true;
            return false;
        }

        private static bool IsImplicitNullableConversion(Type source, Type destination)
        {
            if (IsNullableType(destination))
                return IsImplicitlyConvertible(GetNonNullableType(source), GetNonNullableType(destination));
            return false;
        }

        private static bool IsSameOrSubclass(Type type, Type subType)
        {
            return AreEquivalent(type, subType) || subType.IsSubclassOf(type);
        }

        private static void ValidateType(Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                throw new NotSupportedException("Type is generic");
            }
            if (type.ContainsGenericParameters)
            {
                throw new NotSupportedException("Type contains generic parameters");
            }
        }

        //from TypeHelper

        private static Type FindGenericType(Type definition, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && AreEquivalent(type.GetGenericTypeDefinition(), definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type itype in type.GetInterfaces())
                    {
                        Type found = FindGenericType(definition, itype);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        public static bool IsUnsigned(Type type)
        {
            type = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.Char:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFloatingPoint(Type type)
        {
            type = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Searches for an operator method on the type. The method must have
        /// the specified signature, no generic arguments, and have the
        /// SpecialName bit set. Also searches inherited operator methods.
        /// 
        /// NOTE: This was designed to satisfy the needs of op_True and
        /// op_False, because we have to do runtime lookup for those. It may
        /// not work right for unary operators in general.
        /// </summary>
        private static MethodInfo GetBooleanOperator(Type type, string name)
        {
            do
            {
                MethodInfo result = type.GetMethod(name, AnyStatic, null, new Type[] { type }, null);
                if (result != null && result.IsSpecialName && !result.ContainsGenericParameters)
                {
                    return result;
                }
                type = type.BaseType;
            } while (type != null);
            return null;
        }

        private static Type GetNonRefType(Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        private static readonly Assembly _mscorlib = typeof(object).Assembly;

        /// <summary>
        /// We can cache references to types, as long as they aren't in
        /// collectable assemblies. Unfortunately, we can't really distinguish
        /// between different flavors of assemblies. But, we can at least
        /// create a white list for types in mscorlib (so we get the primitives)
        /// and System.Core (so we find Func/Action overloads, etc).
        /// </summary>
        private static bool CanCache(Type t)
        {
            // Note: we don't have to scan base or declaring types here.
            // There's no way for a type in mscorlib to derive from or be
            // contained in a type from another assembly. The only thing we
            // need to look at is the generic arguments, which are the thing
            // that allows mscorlib types to be specialized by types in other
            // assemblies.

            var asm = t.Assembly;

            if (asm != _mscorlib)
            {
                // Not in mscorlib or our assembly
                return false;
            }

            if (t.IsGenericType)
            {
                foreach (Type g in t.GetGenericArguments())
                {
                    if (!CanCache(g))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
    
    public class ListExtension<T>
    {
        public static List<T> Distinct(List<T> me)
        {
            var distinctList = new List<T>();
            foreach (var element in me)
            {
                if (!distinctList.Contains(element))
                    distinctList.Add(element);
            }
            return distinctList;
        }
    }
}
