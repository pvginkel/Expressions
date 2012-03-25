using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    internal static class CompilerUtil
    {
        public static bool InEnumerable(object arg, IEnumerable set)
        {
            Require.NotNull(set, "set");

            var dictionary = set as IDictionary;

            if (dictionary != null)
                return dictionary.Contains(arg);

            Type argType = null;

            if (arg != null)
                argType = arg.GetType();

            foreach (var item in set)
            {
                if (arg == null)
                {
                    if (item == null)
                        return true;
                }
                else if (item != null)
                {
                    var targetType = item.GetType();

                    if (argType != targetType)
                    {
                        if (TypeUtil.CanCastImplicitely(argType, targetType, false))
                        {
                            if (Equals(TypeUtil.CastImplicitely(arg, targetType), item))
                                return true;
                        }
                        else if (TypeUtil.CanCastImplicitely(targetType, argType, false))
                        {
                            if (Equals(arg, TypeUtil.CastImplicitely(item, argType)))
                                return true;
                        }
                    }
                    else
                    {
                        if (Equals(arg, item))
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool InSet(object arg, params object[] set)
        {
            return InEnumerable(arg, set);
        }
    }
}
