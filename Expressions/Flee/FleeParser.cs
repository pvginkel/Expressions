using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace Expressions.Flee
{
    partial class FleeParser
    {
        private Dictionary<string, int> memory = new Dictionary<string, int>();
        private bool _success = true;

        public bool Success
        {
            get { return _success; }
        }

        public AstParserRuleReturnScope<CommonTree, CommonToken> Parse()
        {
            return prog();
        }

        public override void ReportError(RecognitionException ex)
        {
            _success = false;

            Console.WriteLine("({0},{1}): {2}", ex.Token.Line, ex.Token.CharPositionInLine, ex.Message);

            base.ReportError(ex);
        }
    }
}
