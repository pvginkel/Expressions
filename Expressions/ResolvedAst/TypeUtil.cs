using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Expressions.ResolvedAst
{
    internal static class TypeUtil
    {
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

        private static readonly Dictionary<string, Type> BuiltInTypes = new Dictionary<string, Type>
        {
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

        public static IList<Type> GetImplicitCastingTable(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            IList<Type> result;

            ImplicitCastingTable.TryGetValue(type, out result);

            return result;
        }

        public static bool IsNumeric(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return ImplicitCastingTable.ContainsKey(type);
        }

        public static Type GetBuiltInType(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            Type result;

            BuiltInTypes.TryGetValue(name, out result);

            return result;
        }
    }
}
