using Lexer.Core;

namespace Parser.Core
{
    public abstract class Terminal : Symbol
    {
        public abstract TokenType Type { get; }

        public bool IsAppropriateTerminal(Terminal terminal)
        {
            if (Equals(terminal)) // both terminal with same value
                return true;

            if (this is GeneralizedTerminal && terminal.Type == this.Type)
                return true;

            return false;
        }

        protected static string GetUserFrendlyTypeName(TokenType tokenType)
        {
            var type = typeof(TokenType);
            var memInfo = type.GetMember(tokenType.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(UserFrendlyNameAttribute), false);

            if (attributes.Length != 0)
                return ((UserFrendlyNameAttribute)attributes[0]).Description;

            return null;
        }
    }
}
