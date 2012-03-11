using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expressions.Test
{
    internal static class TypeExtensions
    {
        public static MethodInfo[] GetMethods(Type type, string name, BindingFlags bindingFlags)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");

            var methods = new List<MethodInfo>();

            foreach (var method in type.GetMethods(bindingFlags))
            {
                if (method.Name == name)
                    methods.Add(method);
            }

            return methods.ToArray();
        }
    }
}
