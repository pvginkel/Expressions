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

        public void Parse()
        {
            prog();
        }

        public static ParseResult Parse(string expression)
        {
            try
            {
                var inputStream = new ANTLRStringStream(expression);

                var lexer = new FleeLexer(inputStream);
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new FleeParser(tokenStream);

                var progResult = parser.prog();
                var result = progResult.value;

                return new ParseResult(result, parser._identifiers);
            }
            catch (CompilationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CompilationException("Compilation failed", ex);
            }
        }

        public override object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
        {
            throw e;
        }

        protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        {
            throw new MismatchedTokenException(ttype, input);
        }

        public override void ReportError(RecognitionException ex)
        {
            throw new CompilationException("Compilation failed", ex);
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
            return ParseDecimal(text, NumberStyles.AllowHexSpecifier);
        }

        private Constant ParseDecimal(string text)
        {
            return ParseDecimal(text, NumberStyles.None);
        }

        private Constant ParseDecimal(string text, NumberStyles numberStyles)
        {
            bool isHex = (numberStyles & NumberStyles.AllowHexSpecifier) != 0;

            if (
                text.EndsWith("ul", StringComparison.OrdinalIgnoreCase) ||
                text.EndsWith("lu", StringComparison.OrdinalIgnoreCase)
            )
                return new Constant(new UnparsedNumber(text.Substring(0, text.Length - 2), typeof(ulong), numberStyles));
            else if (text.EndsWith("l", StringComparison.OrdinalIgnoreCase))
                return new Constant(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(long), numberStyles));
            else if (text.EndsWith("u", StringComparison.OrdinalIgnoreCase))
                return new Constant(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(uint), numberStyles));
            else if (
                !isHex && (
                    text.EndsWith("f", StringComparison.OrdinalIgnoreCase) ||
                    text.EndsWith("d", StringComparison.OrdinalIgnoreCase) ||
                    text.EndsWith("m", StringComparison.OrdinalIgnoreCase)
                )
            )
                return ParseFloatingPoint(text);

            Debug.Assert(isHex || Char.IsDigit(text[text.Length - 1]));

            return new Constant(new UnparsedNumber(text, typeof(int), numberStyles));
        }

        private Constant ParseFloatingPoint(string text)
        {
            char suffix = Char.ToLowerInvariant(text[text.Length - 1]);

            switch (suffix)
            {
                case 'f': return new Constant(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(float), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
                case 'd': return new Constant(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(double), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
                case 'm': return new Constant(new UnparsedNumber(text.Substring(0, text.Length - 1), typeof(decimal), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
                default:
                    Debug.Assert(Char.IsDigit(suffix) || suffix == '.');

                    return new Constant(new UnparsedNumber(text, typeof(double), NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent));
            }
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

        private IdentifierAccess CreateIdentifier(string text)
        {
            if (_identifiers.Contains(text))
            {
                return _identifiers[text];
            }
            else
            {
                var identifier = new IdentifierAccess(text);

                _identifiers.Add(identifier);

                return identifier;
            }
        }
    }
}
