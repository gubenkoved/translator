using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer.Core.Validation
{
    public class TokenValidationRule
    {
        public TokenType TokenType { get; private set; }
        
        public Func<Token, bool> IsInvalid { get; private set; }

        public string ErrorMessage { get; private set; }

        public TokenValidationRule(TokenType tokenType, Func<Token, bool> isInvalid, string errorMessage)
        {
            TokenType = tokenType;

            IsInvalid = isInvalid;

            ErrorMessage = errorMessage;
        }

        public Error Validate(Token token)
        {            
            if (token.HasType(TokenType))
            {
                if (IsInvalid.Invoke(token))
                    return new Error(token, ErrorKind.Lexical, ErrorMessage);
                else
                    return null;
            }

            return null;
        }
    }
}
