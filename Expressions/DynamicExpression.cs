using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Expressions
{
    public sealed class DynamicExpression
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

        public BoundExpression Bind()
        {
            return Bind(null);
        }

        public BoundExpression Bind(IBindingContext binder)
        {
            return Bind(binder, null);
        }

        public BoundExpression Bind(IBindingContext binder, BoundExpressionOptions options)
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
            CheckSyntax(expression, language, null);
        }

        public static void CheckSyntax(string expression, ExpressionLanguage language, CultureInfo parsingCulture)
        {
            Require.NotEmpty(expression, "expression");

            DynamicExpressionCache.ParseExpression(expression, language);
        }
    }
}
