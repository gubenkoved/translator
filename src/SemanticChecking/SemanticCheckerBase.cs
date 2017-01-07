using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;
using Parser.Core;

namespace SemanticChecking
{
    public abstract class SemanticCheckerBase
    {
        public abstract IEnumerable<Error> GetSemanticErrors(SyntaxTree tree);
    }
}
