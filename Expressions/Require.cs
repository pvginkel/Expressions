using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JetBrains.Annotations;

namespace Expressions
{
    [DebuggerStepThrough]
    internal static class Require
    {
        [AssertionMethod]
        public static void NotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object param, [InvokerParameterName] string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        [AssertionMethod]
        public static void NotEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] string param, [InvokerParameterName] string paramName)
        {
            if (String.IsNullOrEmpty(param))
                throw new ArgumentException("Value cannot be null or empty.", paramName);
        }

        [AssertionMethod]
        public static void ValidEnum<T>(T param, [InvokerParameterName] string paramName)
        {
            if (!Enum.IsDefined(typeof(T), param))
                throw new ArgumentOutOfRangeException(paramName);
        }

        [AssertionMethod]
        public static void That([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, string error)
        {
            if (!condition)
                throw new ArgumentException(error);
        }

        [AssertionMethod]
        public static void That([AssertionCondition(AssertionConditionType.IS_TRUE)] bool condition, string error, string paramName)
        {
            if (!condition)
                throw new ArgumentException(error, paramName);
        }
    }
}

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    internal sealed class InvokerParameterNameAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal sealed class AssertionMethodAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    internal sealed class AssertionConditionAttribute : Attribute
    {
        public AssertionConditionAttribute(AssertionConditionType conditionType)
        {
            ConditionType = conditionType;
        }

        public AssertionConditionType ConditionType { get; private set; }
    }

    internal enum AssertionConditionType
    {
        IS_TRUE,
        IS_FALSE,
        IS_NULL,
        IS_NOT_NULL
    }
}
