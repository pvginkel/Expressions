using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;

namespace Expressions.Flee
{
    partial class FleeLexer
    {
        public override void ReportError(RecognitionException e)
        {
            throw new ExpressionsException("Invalid syntax", ExpressionsExceptionType.SyntaxError, e);
        }

        protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        {
            throw new MismatchedTokenException(ttype, input);
        }

        public override object RecoverFromMismatchedSet(IIntStream input, RecognitionException e, BitSet follow)
        {
            throw e;
        }
    }
}
