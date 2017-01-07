using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

namespace Parser.Core
{
    public abstract class ParserBase
    {
        public SyntaxTree Parse(IEnumerable<Token> tokens, out IEnumerable<Error> syntaxErrors)
        {
            var valuableTokens = tokens.Where(t => !(t.HasOneOfTypes(TokenType.Delimiter, TokenType.Commentary)));

            var syntaxTree = ParseImplementation(new TokenStream(valuableTokens), out syntaxErrors);

            return syntaxTree;
        }

        protected abstract SyntaxTree ParseImplementation(TokenStream ts, out IEnumerable<Error> syntaxErrors);
    }
}
