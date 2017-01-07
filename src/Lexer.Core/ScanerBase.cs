using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core.Validation;

namespace Lexer.Core
{
    public abstract class ScanerBase
    {
        public TokenValidator Validator { get; private set; }

        protected abstract IEnumerable<Token> GetTokensImplementation(string sourceText);
        public IEnumerable<Token> GetTokens(string sourceText, out IEnumerable<Error> lexicalErrors)
        {
            var tokens = GetTokensImplementation(AddEndMarker(sourceText))
                .Union(new[] { new Token("\u0003", TokenType.EndOfText, TokenPosition.GetTokenPosition(sourceText, sourceText.Length - 1)) });

            lexicalErrors = GetErrors(tokens);

            return tokens;
        }

        private string AddEndMarker(string s)
        {
            // END OF TEXT symbol
            return s + '\u0003';
        }

        private IEnumerable<Error> GetErrors(IEnumerable<Token> tokens)
        {            
            if (Validator != null)
                return Validator.GetAllErrors(tokens);
            else
                return new List<Error>();
        }

        protected ScanerBase(TokenValidator validator)
        {
            Validator = validator;
        }
    }
}
