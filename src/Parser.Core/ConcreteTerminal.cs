using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

namespace Parser.Core
{
    public class ConcreteTerminal : Terminal
    {
        public bool IsEpsilon
        {
            get
            {
                return Token.Type == TokenType.Epsilon;
            }
        }

        public Token Token { get; private set; }
        public override TokenType Type
        {
            get
            {
                return Token.Type;
            }
        }

        public ConcreteTerminal(Token token)
        {
            Token = token;
        }

        public override string ToString()
        {
            var userFrendlyTypeName = GetUserFrendlyTypeName(Type);

            if (userFrendlyTypeName != null)
                return string.Format("{0} [{1}]", Token.Value, userFrendlyTypeName);

            return string.Format("{0} [{1}]", Token.Value, Type.ToString());            
        }

        public override Symbol CreateCopy()
        {
            return new ConcreteTerminal(Token);
        }

        internal static readonly LambdaEqualityComparer<ConcreteTerminal> ByValueEqualityComprarer
            = new LambdaEqualityComparer<ConcreteTerminal>(
                (ct1, ct2) => ct1.Token.Value == ct2.Token.Value,
                (ct) => ct.Token.Value.GetHashCode());
    }
}
