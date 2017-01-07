using Lexer.Core;
using System.Collections.Generic;
using System.Linq;

namespace Lexer.Core
{
    public class Token
    {
        public TokenType Type { get; private set; }
        public TokenPosition? Position { get; private set; }

        public string Value { get; private set; }
        public string EscapedValue
        {
            get
            {
                return Value.Escape();
            }
        }

        public Token(string value, TokenType type)
            : this(value, type, null)
        {
        }

        public Token(string value, TokenType type, TokenPosition? position)
        {
            Value = value;

            Type = type;

            Position = position;
        }

        public bool HasType(TokenType type)
        {
            return Type == type;
        }

        public bool HasOneOfTypes(params TokenType[] types)
        {
            return types.Any(t => t == Type);
        }

        public override string ToString()
        {
            return EscapedValue;
        }
    }
}