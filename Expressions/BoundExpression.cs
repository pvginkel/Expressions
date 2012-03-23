using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Expressions.Expressions;

namespace Expressions
{
    public sealed class BoundExpression
    {
#pragma warning disable 0649
        private readonly DynamicSignature _compiledMethod;
        private readonly DynamicExpression _dynamicExpression;
        private readonly int[] _parameterMap;

#if DEBUG
        // For unit testing
        internal IExpression ResolvedExpression { get; private set; }
#endif

        internal BoundExpression(DynamicExpression dynamicExpression, Type ownerType, Import[] imports, Type[] identifierTypes)
        {
            Require.NotNull(dynamicExpression, "dynamicExpression");
            Require.NotNull(imports, "imports");
            Require.NotNull(identifierTypes, "identifierTypes");

            _dynamicExpression = dynamicExpression;

            _parameterMap = BuildParameterMap(ownerType, identifierTypes);

            var resolvedExpression = Resolve(ownerType, imports, identifierTypes);

            _compiledMethod = CompileExpression(resolvedExpression, ownerType, imports, identifierTypes);
        }

        private IExpression Resolve(Type ownerType, Import[] imports, Type[] identifierTypes)
        {
            var resolver = new Resolver(_dynamicExpression, ownerType, imports, identifierTypes, _parameterMap);

            var resolvedTree = resolver.Resolve(_dynamicExpression.ParseResult.RootNode);

#if DEBUG
            ResolvedExpression = resolvedTree;
#endif

            return resolvedTree;
        }

        private int[] BuildParameterMap(Type ownerType, Type[] identifierTypes)
        {
            var parameterMap = new List<int>();

            if (ownerType != null)
                parameterMap.Add(-1);

            for (int i = 0; i < identifierTypes.Length; i++)
            {
                if (identifierTypes[i] != null)
                    parameterMap.Add(i);
            }

            return parameterMap.ToArray();
        }

        private DynamicSignature CompileExpression(IExpression expression, Type ownerType, Import[] imports, Type[] identifierTypes)
        {
            var method = new DynamicMethod(
                "DynamicMethod",
                typeof(object),
                new[] { typeof(object[]) },
                true /* restrictedSkipVisibility */
            );

            var il = method.GetILGenerator();

            new Compiler(il).Compile(expression);

#if DEBUG
            WriteToDisk(expression);
#endif

            return (DynamicSignature)method.CreateDelegate(typeof(DynamicSignature));
        }

#if DEBUG
        private void WriteToDisk(IExpression expression)
        {
            //var name = "Output.exe";
            //var assemblyName = new AssemblyName(name);
            //var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            //var moduleBuilder = assemblyBuilder.DefineDynamicModule(name);
            //var programClass = moduleBuilder.DefineType("Program", TypeAttributes.Public);

            //var mainmethod = programClass.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static, null, new[] { typeof(string[]) });

            //var il = mainmethod.GetILGenerator();

            //new Compiler(il).Compile(expression);

            //programClass.CreateType();

            //assemblyBuilder.SetEntryPoint(programClass.GetMethod("Main"));
            //assemblyBuilder.Save(name);
        }
#endif

        public object Invoke(IExecutionContext executionContext)
        {
            Require.NotNull(executionContext, "executionContext");

            var parameters = new object[_parameterMap.Length];

            bool ignoreCase = !DynamicExpression.IsLanguageCaseSensitive(_dynamicExpression.Language);
            var identifiers = _dynamicExpression.ParseResult.Identifiers;

            for (int i = 0; i < parameters.Length; i++)
            {
                int index = _parameterMap[i];

                if (index == -1)
                {
                    parameters[i] = executionContext.Owner;
                }
                else
                {
                    parameters[i] = executionContext.GetVariableValue(
                        identifiers[index].Name, ignoreCase
                    );
                }
            }

            return _compiledMethod(parameters);
        }

        private delegate object DynamicSignature(object[] parameters);
    }
}
