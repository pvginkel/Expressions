using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Expressions
{
    /// <summary>
    /// Represents a parsed expression.
    /// </summary>
    public class DynamicExpression
    {
        internal CachedDynamicExpression Cached { get; private set; }

        /// <summary>
        /// Get the text of the parsed expression.
        /// </summary>
        public string Expression { get; private set; }

        /// <summary>
        /// Get the language of the parsed expression.
        /// </summary>
        public ExpressionLanguage Language { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicExpression"/>
        /// class with the specified expression and language.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="language">The language of the expression to parse.</param>
        public DynamicExpression(string expression, ExpressionLanguage language)
        {
            Require.NotNull(expression, "expression");

            Expression = expression;
            Language = language;

            Cached = DynamicExpressionCache.GetOrCreateCachedDynamicExpression(expression, language);
        }

        /// <summary>
        /// Binds the compiled expression.
        /// </summary>
        /// <returns>The bound expression.</returns>
        public IBoundExpression Bind()
        {
            return Bind(null);
        }

        /// <summary>
        /// Binds the compiled expression with the specified binding context.
        /// </summary>
        /// <param name="binder">The binding context used to bind the expression.</param>
        /// <returns>The bound expression.</returns>
        public IBoundExpression Bind(IBindingContext binder)
        {
            return Bind(binder, null);
        }

        /// <summary>
        /// Binds the compiled expression with the specified binding context and options.
        /// </summary>
        /// <param name="binder">The binding context used to bind the expression.</param>
        /// <param name="options">The options used to bind the expression.</param>
        /// <returns>The bound expression.</returns>
        public IBoundExpression Bind(IBindingContext binder, BoundExpressionOptions options)
        {
            if (binder == null)
                binder = new ExpressionContext();
            if (options == null)
                options = new BoundExpressionOptions();

            options.Freeze();

            return Cached.GetOrCreateBoundExpression(binder, options);
        }

        /// <summary>
        /// Invokes the expression.
        /// </summary>
        /// <returns>The result of the expression.</returns>
        public object Invoke()
        {
            return Invoke(null);
        }

        /// <summary>
        /// Invokes the expression with the specified expression context.
        /// </summary>
        /// <param name="expressionContext">The expression context used to
        /// bind and execute the expression.</param>
        /// <returns>The result of the expression.</returns>
        public object Invoke(IExpressionContext expressionContext)
        {
            return Invoke(expressionContext, null);
        }

        /// <summary>
        /// Invokes the expression with the specified expression context and
        /// binding options.
        /// </summary>
        /// <param name="expressionContext">The expression context used to
        /// bind and execute the expression.</param>
        /// <param name="options">The options used to bind the expression.</param>
        /// <returns>The result of the expression.</returns>
        public object Invoke(IExpressionContext expressionContext, BoundExpressionOptions options)
        {
            if (expressionContext == null)
                expressionContext = new ExpressionContext();

            return Bind(expressionContext, options).Invoke(expressionContext);
        }

        /// <summary>
        /// Determines whether the specified language is a case sensitive language.
        /// </summary>
        /// <param name="language">The language to determine case sensitivity for.</param>
        /// <returns>True when the specified language is case sensitive; false otherwise.</returns>
        public static bool IsLanguageCaseSensitive(ExpressionLanguage language)
        {
            switch (language)
            {
                case ExpressionLanguage.Flee: return false;
                case ExpressionLanguage.VisualBasic: return false;
                case ExpressionLanguage.Csharp: return true;
                default: throw new ArgumentOutOfRangeException("language");
            }
        }

        /// <summary>
        /// Checks the syntax of the specified expression.
        /// </summary>
        /// <param name="expression">The expression of which to check the syntax.</param>
        /// <param name="language">The language used when checking the syntax.</param>
        public static void CheckSyntax(string expression, ExpressionLanguage language)
        {
            Require.NotEmpty(expression, "expression");

            DynamicExpressionCache.ParseExpression(expression, language);
        }
    }

    /// <summary>
    /// Represents a parsed expression.
    /// </summary>
    /// <typeparam name="T">The type of the result of the expression.</typeparam>
    public class DynamicExpression<T> : DynamicExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicExpression"/>
        /// class with the specified expression and language.
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="language">The language of the expression to parse.</param>
        public DynamicExpression(string expression, ExpressionLanguage language)
            : base(expression, language)
        {
        }

        /// <summary>
        /// Invokes the expression.
        /// </summary>
        /// <returns>The result of the expression.</returns>
        public new T Invoke()
        {
            return (T)base.Invoke();
        }

        /// <summary>
        /// Invokes the expression with the specified expression context.
        /// </summary>
        /// <param name="expressionContext">The expression context used to
        /// bind and execute the expression.</param>
        /// <returns>The result of the expression.</returns>
        public new T Invoke(IExpressionContext expressionContext)
        {
            var options = new BoundExpressionOptions
            {
                ResultType = typeof(T)
            };

            return (T)base.Invoke(expressionContext, options);
        }

        /// <summary>
        /// Invokes the expression with the specified expression context and
        /// binding options.
        /// </summary>
        /// <param name="expressionContext">The expression context used to
        /// bind and execute the expression.</param>
        /// <param name="options">The options used to bind the expression.</param>
        /// <returns>The result of the expression.</returns>
        public new T Invoke(IExpressionContext expressionContext, BoundExpressionOptions options)
        {
            if (options.ResultType == null)
            {
                options = new BoundExpressionOptions(options);
                options.ResultType = typeof(T);
            }

            return (T)base.Invoke(expressionContext, options);
        }

        /// <summary>
        /// Binds the compiled expression.
        /// </summary>
        /// <returns>The bound expression.</returns>
        public new IBoundExpression<T> Bind()
        {
            return new BoundExpression<T>(base.Bind());
        }

        /// <summary>
        /// Binds the compiled expression with the specified binding context.
        /// </summary>
        /// <param name="binder">The binding context used to bind the expression.</param>
        /// <returns>The bound expression.</returns>
        public new IBoundExpression<T> Bind(IBindingContext binder)
        {
            return new BoundExpression<T>(base.Bind(binder));
        }

        /// <summary>
        /// Binds the compiled expression with the specified binding context and options.
        /// </summary>
        /// <param name="binder">The binding context used to bind the expression.</param>
        /// <param name="options">The options used to bind the expression.</param>
        /// <returns>The bound expression.</returns>
        public new IBoundExpression<T> Bind(IBindingContext binder, BoundExpressionOptions options)
        {
            return new BoundExpression<T>(base.Bind(binder, options));
        }
    }
}
