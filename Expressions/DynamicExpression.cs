using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Expressions
{
    public class DynamicExpression
    {
        internal CachedDynamicExpression Cached { get; private set; }

        public string Expression { get; private set; }

        public ExpressionLanguage Language { get; private set; }

        public DynamicExpression(string expression, ExpressionLanguage language)
        {
            Require.NotNull(expression, "expression");

            Expression = expression;
            Language = language;

            Cached = DynamicExpressionCache.GetOrCreateCachedDynamicExpression(expression, language);
        }

        public IBoundExpression Bind()
        {
            return Bind(null);
        }

        public IBoundExpression Bind(IBindingContext binder)
        {
            return Bind(binder, null);
        }

        public IBoundExpression Bind(IBindingContext binder, BoundExpressionOptions options)
        {
            if (binder == null)
                binder = new ExpressionContext();
            if (options == null)
                options = new BoundExpressionOptions();

            options.Freeze();

            return Cached.GetOrCreateBoundExpression(binder, options);
        }

        public object Invoke()
        {
            return Invoke(null);
        }

        public object Invoke(IExpressionContext expressionContext)
        {
            return Invoke(expressionContext, null);
        }

        public object Invoke(IExpressionContext expressionContext, BoundExpressionOptions options)
        {
            if (expressionContext == null)
                expressionContext = new ExpressionContext();

            return Bind(expressionContext, options).Invoke(expressionContext);
        }

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

        public static void CheckSyntax(string expression, ExpressionLanguage language)
        {
            Require.NotEmpty(expression, "expression");

            DynamicExpressionCache.ParseExpression(expression, language);
        }
    }

    public class DynamicExpression<T> : DynamicExpression
    {
        public DynamicExpression(string expression, ExpressionLanguage language)
            : base(expression, language)
        {
        }

        public new T Invoke()
        {
            return (T)base.Invoke();
        }

        public new T Invoke(IExpressionContext expressionContext)
        {
            var options = new BoundExpressionOptions
            {
                ResultType = typeof(T)
            };

            return (T)base.Invoke(expressionContext, options);
        }

        public new T Invoke(IExpressionContext expressionContext, BoundExpressionOptions options)
        {
            if (options.ResultType == null)
            {
                options = new BoundExpressionOptions(options);
                options.ResultType = typeof(T);
            }

            return (T)base.Invoke(expressionContext, options);
        }

        public new IBoundExpression<T> Bind()
        {
            return new BoundExpression<T>(base.Bind());
        }

        public new IBoundExpression<T> Bind(IBindingContext binder)
        {
            return new BoundExpression<T>(base.Bind(binder));
        }

        public new IBoundExpression<T> Bind(IBindingContext binder, BoundExpressionOptions options)
        {
            return new BoundExpression<T>(base.Bind(binder, options));
        }
    }
}
