# Expressions

LGPL License.

[Download from NuGet](http://nuget.org/packages/Expressions).

## Introduction

Expressions is an expression parser and evaluator with support for C#, VB.NET and Flee.

The Expressions project is heavily based on [Flee](http://flee.codeplex.com/). Flee is
a library which parses and evaluates the Flee expression language by compiling it into
MSIL using the
[DynamicMethod](http://msdn.microsoft.com/en-us/library/system.reflection.emit.dynamicmethod.aspx)
and [Emit](http://msdn.microsoft.com/en-us/library/system.reflection.emit.aspx)
infrastructure introduced in .NET 2.0.

Even though Flee is a very capable library to perform these tasks, it has a number
limitations, specifically:

* Flee expressions must be written in a custom language;

* The Flee library requires expressions to be bound to a runtime context in a very early
  stage. This prohibits easy caching of compiled expressions and requires a compiled
  expression to keep a reference to an instance of an object, preventing it from being
  collected.

The Expressions library solves these issues by introduction the following new
functionality:

* Expressions can be written in C#, VB.NET and Flee;

* Compiling, binding and invoking expressions is split up into three different phases.
  Compiled and bound expressions are automatically cached and a previous parse tree
  or compiled expression is automatically retrieved for you when it is already
  available;

* No references to actual objects are required when binding an expression. This has
  the advantage that no references to actual objects are being kept and that the
  objects used when invoking an expressions are free to be garbage collected.

## Invoking expressions

The actual process of compiling and invoking an expression is built up out of
three stages.

### Parsing

The first step of invoking an expression is parsing it. Parsing an expression is
done by creating a new instance of the DynamicExpression class:

    var expression = new DynamicExpression("a + b", ExpressionLanguage.Csharp);

This action performs the following steps:

* An internal cache is queried to see whether a compilation result for the
  provided expression already exists. If so, the cached compilation result is
  used instead of re-compiling the expression;

* If no cached compilation result exists, the expression is compiled and
  cached.

This phase of the process performs syntax checking. It is also possible to
do a syntax check without the compilation result being cached. This is done
with the CheckSyntax method:

    DynamicMethod.CheckSyntax("a + b", ExpressionLanguage.Csharp);

When the syntax check fails, an exception is thrown describing the issue with
the provided expression.

### Binding

The second phase of invoking an expression is the binding phase. Binding an
expression means that the different syntax elements of an expression are resolved
into methods, variables and other known elements and then compiled.
When your expression makes use of input parameters, these can be provided in this
phase.

There are two ways in which you can provide this information to the binding
phase:

* The IBindingContext interface describes a number of methods which the
  binding phase uses to discover the context of an expression. By implementing
  this interface, you have full control over the context Expressions uses
  to bind the expression. You can provide the following information to the
  binding process:

  * OwnerType: The owner of an expression is an object which kind of functions
    like this this (Me in VB.NET) of an expression. Static and instance properties,
    methods and fields are automatically made available in the expression;

  * Imports: Imports are additional types of which the static methods, properties
    and fields are made available in an expression;

  * GetVariableType: Variables are made available to expressions and kind of
    work like the parameters of a normal method call. This method returns
    the type of such a variable.

* The ExpressionContext class implements the IBindingContext interface. It works
  by providing you with a number of collections and properties which you can
  set and are then used to provide the information for the binding context.

Binding an expression is done as follows:

    var context = new ExpressionContext();

    context.Variables.Add("a", 10);
    context.Variables.Add("b", 20);

    var boundExpression = expression.Bind(context);

By providing a BoundExpressionOptions instance when calling Bind, the binding
process can be customized. The following options are available:

* AllowPrivateAccess: Determines whether protected and provide methods,
  properties and fields are available in an expression (defaults to false);

* ResultType: Determines the type of the result of the expression. When the
  expression doesn't already result in this type, it is casted to this type
  before being returned. The generic overloads of Bind and Invoke automatically
  set the ResultType for you if not already provided (defaults to "typeof(object)");

* Checked: Whether arithmetic operations are checked (defaults to false);

* RestrictedSkipVisibility: Tells DynamicMethod whether to skip JIT visibility
  checkes on types and members accessed by the MSIL of the dynamic method.
  See the [documentation](http://msdn.microsoft.com/en-us/library/system.reflection.emit.dynamicmethod.aspx)
  for more information.

### Invoking

When the expression has been bound and compiled, it can be invoked. Just
like the binding phase has an IBindingContext, the invoking phase has an
IExecutionContext. Where the IBindingContext interface provides the types of
the different parts of the context of an expression, the IExecutionContext
interface provides the actual objects.

The following information can be provided when invoking an expression:

* Owner: The actual owner of the expression;

* GetVariableValue: The actual value of the variables of the expression.

When you use the ExpressionContext class instead of implementing the above
interface, this same instance can be used when invoking the expression.
Between different calls to invoke, you are free to change the contexts of
the ExpressionContext. Note however that once the expression is bound,
you must only provide data that is compatible with the data used when
binding the expression. When e.g. a variable was an Integer when it was
bound, and you replace it with a Double, a runtime exception will be
thrown.

### Combined binding and invocation

The DynamicExpression class also has an Invoke method. This Invoke
method combines the binding and invocation of an expression. It takes a
IExpressionContext (an interface that combines IBindingContext and
IExecutionContext) and a BoundExpressionOptions. Using this method instead
of explicitly binding an expression first allows you to freely change
the context of an ExpressionContext instance without having to worry about
runtime cast errors. Whenever the context of the expression is changed
in a way that is incompatible with an already cached expression, it
it automatically recompiled with the new context.

## Language support

For C# and VB.NET, most of the expressions that are supported by that
language, are supported by Expressions. For detailed information about
the supported expressions, the ANTLR grammar files are available. One
specific addition made to the C# parser is the use of literal dates.
This is a feature from VB.NET which I was compelled to include in the
C# parser. The Flee language also supports ## for specifying TimeSpan's.
This feature is included in both the C# and VB.NET parser.

The Flee language is very well documented and can be found on the
[Language Reference](http://flee.codeplex.com/wikipage?title=LanguageReference)
page of the Flee wiki.

## Differences with Flee

Even though the Expressions library attempts to be compatible with the
Flee library, there are a number of unsupported features and limitations:

* Flee supports culture sensitive parsing and providing the string quote
  character of an expression. Since Expressions is based on ANTLR, this
  is not supported. For C# and VB.NET, numbers and dates are written just
  like they in the language itself. For Flee, the default settings of the
  Flee parser are used;

* Flee throws an exception when invalid imports are added to its context.
  Expressions delays this until the actual binding process;

* In Flee, when an expression is used as a parameter for another expression,
  the expression already has a context with the variables and owner
  of that expression. Because Expressions doesn't have this context
  available until the expression is invoked, it is not possible to
  provide variables and an owner in this context;

* With flee, null is automatically coerced to 0 when the result type
  of an expression is Integer;

* Flee can be configured to be fully case sensitive. This is not possible with
  the Expressions version of the Flee language. However, the ExpressionContext
  class does support an ignoreCase parameter which can be used to specify how
  the case of variables is treated. This argument defaults to false for all
  languages supported by Expressions;

* Expressions is more relaxed in what casts are supported when the ResultType
  (or the generic type parameter for DynamicExpression) has been provided.
  With Flee, only implicit casts are allowed. Expressions treats specifying the
  ResultType of an expression like the VB.NET CType construct;

* The Flee calculation engine is not part of the Expressions library.

The above list of unsupported features are features that will not be
implemented in the Expression library. There are however a number
of features which are scheduled. These are created as issues in
the [issues section](https://github.com/pvginkel/Expressions/issues).
