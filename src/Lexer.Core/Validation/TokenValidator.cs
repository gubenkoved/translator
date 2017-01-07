using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer.Core.Validation
{
    public class TokenValidator
    {
        private List<TokenValidationRule> _rules;
        public IEnumerable<TokenValidationRule> Rules
        {
            get
            {
                return _rules;
            }
        }

        public TokenValidator()
        {
            _rules = new List<TokenValidationRule>();
        }

        public void AddRule(TokenValidationRule rule)
        {
            _rules.Add(rule);
        }

        public IEnumerable<Error> GetAllErrors(IEnumerable<Token> tokens)
        {
            var errors = new List<Error>();

            foreach (var token in tokens)
            {
                foreach (var rule in _rules)
                {
                    Error error = rule.Validate(token);

                    if (error != null)
                        errors.Add(error);
                }
            }

            return errors;
        }

        public static TokenValidator BasicValidator;

        static TokenValidator()
        {
            BasicValidator = new TokenValidator();

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.Unknown,
                    (t) => true,
                    "Unrecognized token"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.Identifier,
                    (t) => t.Value.Length > 14,
                    "Identifier length must be less or equal 14"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.Identifier,
                    (t) => !char.IsLetter(t.Value[0]),
                    "Identifier must begin with the letter"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.StringConstant,
                    (t) => t.Value[t.Value.Length - 1] != '"',
                    "Not closed literal"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.IntegerConstant,
                    (t) => 
                        {
                            int i;
                            return !int.TryParse(t.Value, out i);                            
                        },
                    "Incorrect integer constant"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.Commentary,
                    (t) => t.Value.StartsWith("/*") && !t.Value.EndsWith("*/"),
                    "Unclosed multiline comment"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.CharConstant,
                    (t) => !t.Value.EndsWith("'"),
                    "Unclosed char constant"));

            BasicValidator.AddRule(new TokenValidationRule(
                    TokenType.CharConstant,
                    (t) => t.Value.EndsWith("'") && t.Value.Length != 3,
                    "Char constant must contains one symbol"));
        }
    }
}
