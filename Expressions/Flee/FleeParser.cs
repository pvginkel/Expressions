using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Antlr.Runtime;
using Expressions.Ast;

namespace Expressions.Flee
{
    partial class FleeParser
    {
        private readonly IdentifierCollection _identifiers = new IdentifierCollection(StringComparer.OrdinalIgnoreCase);

        public RecognitionException Exception { get; private set; }

        public void Parse()
        {
            prog();
        }

        public static ParseResult Parse(string expression)
        {
            var inputStream = new ANTLRStringStream(expression);

            var lexer = new FleeLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new FleeParser(tokenStream);

            var result = parser.prog().value;

            if (parser.Exception != null)
                throw new CompilationException("Expression could not be compiled", parser.Exception);

            return new ParseResult(result, parser._identifiers);
        }

        public override void ReportError(RecognitionException ex)
        {
            Exception = ex;

            base.ReportError(ex);
        }

        private Constant ParseDateTime(string text)
        {
            Debug.Assert(text.Substring(0, 1) == "#");
            Debug.Assert(text.Substring(text.Length - 1) == "#");

            return new Constant(DateTime.Parse(text.Substring(1, text.Length - 2)));
        }

        private Constant ParseTimeSpan(string text)
        {
            Debug.Assert(text.Substring(0, 2) == "##");
            Debug.Assert(text.Substring(text.Length - 1) == "#");

            return new Constant(TimeSpan.Parse(text.Substring(2, text.Length - 3)));
        }

        private Constant ParseHex(string text)
        {
            Debug.Assert(text.Substring(0, 2) == "0x");

            return ParseDecimal(text.Substring(2), NumberStyles.AllowHexSpecifier);
        }

        private Constant ParseDecimal(string text)
        {
            return ParseDecimal(text, NumberStyles.None);
        }

        private Constant ParseDecimal(string text, NumberStyles numberStyles)
        {
            if (text.EndsWith("ul", StringComparison.OrdinalIgnoreCase))
                return new Constant(ulong.Parse(text.Substring(0, text.Length - 2), numberStyles, CultureInfo.InvariantCulture));
            else if (text.EndsWith("l", StringComparison.OrdinalIgnoreCase))
                return new Constant(long.Parse(text.Substring(0, text.Length - 1), numberStyles, CultureInfo.InvariantCulture));
            else if (text.EndsWith("u", StringComparison.OrdinalIgnoreCase))
                return new Constant(uint.Parse(text.Substring(0, text.Length - 1), numberStyles, CultureInfo.InvariantCulture));
            else
                return new Constant(int.Parse(text, numberStyles, CultureInfo.InvariantCulture));
        }

        private Constant ParseCharacter(string text)
        {
            Debug.Assert(text.Substring(0, 1) == "'");
            Debug.Assert(text.Substring(text.Length - 1) == "'");

            text = ParseEscapes(text.Substring(1, text.Length - 2));

            Debug.Assert(text.Length == 1);

            return new Constant(text[0]);
        }

        private Constant ParseString(string text)
        {
            Debug.Assert(text.Substring(0, 1) == "\"");
            Debug.Assert(text.Substring(text.Length - 1) == "\"");

            text = ParseEscapes(text.Substring(1, text.Length - 2));

            return new Constant(text);
        }

        private string ParseEscapes(string text)
        {
            var sb = new StringBuilder(text.Length);

            bool hadEscape = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (hadEscape)
                {
                    hadEscape = false;

                    switch (c)
                    {
                        case 'B':
                        case 'b':
                            sb.Append('\b');
                            break;

                        case 'T':
                        case 't':
                            sb.Append('\t');
                            break;

                        case 'N':
                        case 'n':
                            sb.Append('\n');
                            break;

                        case 'R':
                        case 'r':
                            sb.Append('\r');
                            break;

                        case 'F':
                        case 'f':
                            sb.Append('\f');
                            break;

                        case '"':
                            sb.Append('"');
                            break;

                        case '\'':
                            sb.Append('\'');
                            break;

                        case '\\':
                            sb.Append('\\');
                            break;

                        case 'u':
                        case 'U':
                            Debug.Assert(i < text.Length - 4);

                            sb.Append((char)uint.Parse(text.Substring(i + 1, 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));

                            i += 4;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(text);
                    }
                }
                else
                {
                    if (c == '\\')
                        hadEscape = true;
                    else
                        sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private Constant ParseFloatingPoint(string text)
        {
            char suffix = Char.ToLowerInvariant(text[text.Length - 1]);

            switch (suffix)
            {
                case 'f': return new Constant(float.Parse(text.Substring(0, text.Length - 1), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture));
                case 'd': return new Constant(double.Parse(text.Substring(0, text.Length - 1), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture));
                case 'm': return new Constant(decimal.Parse(text.Substring(0, text.Length - 1), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture));
                default: return new Constant(double.Parse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture));
            }
        }

        private Identifier CreateIdentifier(string text)
        {
            if (_identifiers.Contains(text))
            {
                return _identifiers[text];
            }
            else
            {
                var identifier = new Identifier(text);

                _identifiers.Add(identifier);

                return identifier;
            }
        }
    }
}
