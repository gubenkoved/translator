using System.Collections.Generic;

using System.Linq;

namespace Lexer.Core
{
    public class Descriptor
    {
    }

    public class DescriptorsPresentation
    {
        public StringList KeywordsList { get; set; }
        public StringList OperatorsList { get; set; }
        public StringList FunctionsList { get; set; }
        public StringList IdentifiersList { get; set; }
        public StringList ConstantsList { get; set; }

        public IEnumerable<Token> Tokens { get; private set; }

        public DescriptorsPresentation(IEnumerable<Token> tokens)
        {
            Tokens = tokens;

            KeywordsList = new StringList("KW");
            OperatorsList = new StringList("OP");
            FunctionsList = new StringList("FN");
            IdentifiersList = new StringList("ID");
            ConstantsList = new StringList("CN");

            FillTables();
        }

        private void FillTables()
        {
            KeywordsList.AddRange(Tokens.Where(t => t.HasType(TokenType.Keyword)).Select(t => t.Value).Distinct());
            OperatorsList.AddRange(Tokens.Where(t => t.HasType(TokenType.Operator)).Select(t => t.Value).Distinct());
            FunctionsList.AddRange(Tokens.Where(t => t.HasType(TokenType.Function)).Select(t => t.Value).Distinct());
            IdentifiersList.AddRange(Tokens.Where(t => t.HasType(TokenType.Identifier)).Select(t => t.Value).Distinct());
            ConstantsList.AddRange(Tokens.Where(t => t.HasOneOfTypes(TokenType.CharConstant, TokenType.IntegerConstant, TokenType.FloatConstant, TokenType.StringConstant)).Select(t => t.Value.Escape()).Distinct());
        }

        public StringList GetTableFor(Token token)
        {
            if (token.HasType(TokenType.Keyword))
                return KeywordsList;
            else if (token.HasType(TokenType.Operator))
                return OperatorsList;
            else if (token.HasType(TokenType.Function))
                return FunctionsList;
            else if (token.HasType(TokenType.Identifier))
                return IdentifiersList;
            else if (token.HasOneOfTypes(TokenType.CharConstant, TokenType.IntegerConstant, TokenType.FloatConstant, TokenType.StringConstant))
                return ConstantsList;

            return null;
        }
    }
}