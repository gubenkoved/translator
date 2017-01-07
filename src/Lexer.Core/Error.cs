namespace Lexer.Core
{
    public enum ErrorKind
    {
        Lexical,
        Syntax,
        Semantic
    }

    public class Error
    {
        public ErrorKind Kind { get; private set; }

        public Token Token { get; private set; }
        public string ErrorMessage { get; private set; }
        public string FormattedMessage
        {
            get 
            {
                if (Token != null)
                    return GerErrorText();
                else
                    return ErrorMessage;
            }
        }

        public Error(Token token, ErrorKind kind, string message)
        {
            Token = token;

            Kind = kind;

            ErrorMessage = message;
        }

        private string GerErrorText()
        {
            return string.Format("[L{1:D3}, C{2:D3}] {0} in '{3}'",
                                 ErrorMessage,                                 
                                 Token.Position.Value.Line,
                                 Token.Position.Value.Column,
                                 Token.Value.Escape());
        }
    }
}