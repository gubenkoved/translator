using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Lexer.Core.Validation;

namespace Lexer.Core
{
    public class SimpleScaner : ScanerBase
    {
        private enum SimpleScanerTokenType
        {
            Identifier,
            Literal,
            Number,
            Operator,
            Delimiter,
            Unknown,
            CommentLine
        }

        private List<string> _keywords;
        private List<char> _delimiters;
        private List<string> _operators;
        private List<string> _functions;

        public SimpleScaner()
            : base(TokenValidator.BasicValidator)
        {
            _keywords = new List<string>(new[]
                                             {                                                 
                                                 "int",
                                                 "string",
                                                 "char",
                                                 "float",
                                                 "bool",
                                                 "goto",
                                                 "switch",
                                                 "case",
                                                 "if",
                                                 "else",
                                                 "return",
                                                 "void",
                                                 "default",
                                                 "break"
                                             });

            _delimiters = new List<char>(new[]
                                             {
                                                 ' ',
                                                 '\t',
                                                 '\n',
                                                 '\r',
                                                 '\u0003'
                                             });

            _operators = new List<string>(new[]
                                             {
                                                "+",
                                                "-",
                                                "*",
                                                "/",
                                                "=",
                                                "==",
                                                "!=",
                                                ">",
                                                "<",
                                                "<=",
                                                ">=",
                                                "(",
                                                ")",
                                                "{",
                                                "}",
                                                ";",
                                                ","
                                             });

            _functions = new List<string>(new[]
                                             {
                                                 "sqrt",
                                                 "log",
                                                 "ln",
                                                 "nearby",
                                                 "readln",
                                                 "writeln",
                                                 "cos",
                                                 "abs",
                                                 "tanh"
                                             });
        }

        protected override IEnumerable<Token> GetTokensImplementation(string sourceText)
        {
            var tokens = new List<Token>();

            int startIndex = 0;
            int currentIndex = 0;

            do
            {
                if (currentIndex >= sourceText.Length || startIndex >= sourceText.Length)
                {
                    break;
                }

                string firstCharOfToken = sourceText.Substring(startIndex, 1);

                SimpleScanerTokenType expectedToken;

                if (sourceText.Length - startIndex > 1 && sourceText.Substring(startIndex, 2) == "//")
                    expectedToken = SimpleScanerTokenType.CommentLine;
                else if (_delimiters.Contains(firstCharOfToken[0]))
                    expectedToken = SimpleScanerTokenType.Delimiter;
                else if (firstCharOfToken == "\"")
                    expectedToken = SimpleScanerTokenType.Literal;
                else if (_operators.Any(op => op.StartsWith(firstCharOfToken)))
                    expectedToken = SimpleScanerTokenType.Operator;
                else if (Regex.IsMatch(firstCharOfToken, "[A-Za-z_]"))
                    expectedToken = SimpleScanerTokenType.Identifier;
                else if (Regex.IsMatch(firstCharOfToken, "[0-9]"))
                    expectedToken = SimpleScanerTokenType.Number;
                else
                    expectedToken = SimpleScanerTokenType.Unknown;

                switch (expectedToken)
                {
                    case SimpleScanerTokenType.CommentLine:
                        while (sourceText[currentIndex + 1] != '\n')
                        {
                            ++currentIndex;
                        }

                        string comment = sourceText.Substring(startIndex, currentIndex - startIndex);

                        tokens.Add(new Token(comment, TokenType.Commentary,  TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;

                    case SimpleScanerTokenType.Delimiter:
                        while (currentIndex < sourceText.Length - 1 && _delimiters.Contains(sourceText[currentIndex + 1]))
                        {
                            ++currentIndex;
                        }

                        string delimiters = sourceText.Substring(startIndex, currentIndex - startIndex + 1);

                        tokens.Add(new Token(delimiters, TokenType.Delimiter, TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;

                    case SimpleScanerTokenType.Identifier:
                        while (Regex.IsMatch(sourceText[currentIndex + 1].ToString(), "[A-Za-z0-9_]"))
                        {
                            ++currentIndex;
                        }

                        string identifier = sourceText.Substring(startIndex, currentIndex - startIndex + 1);

                        if (_keywords.Contains(identifier))
                            tokens.Add(new Token(identifier, TokenType.Keyword, TokenPosition.GetTokenPosition(sourceText, startIndex)));
                        else if (_functions.Contains(identifier))
                            tokens.Add(new Token(identifier, TokenType.Function, TokenPosition.GetTokenPosition(sourceText, startIndex)));
                        else
                            tokens.Add(new Token(identifier, TokenType.Identifier, TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;

                    case SimpleScanerTokenType.Literal:
                        ++currentIndex;

                        while (sourceText[currentIndex + 1] != '"' && sourceText[currentIndex + 1] != '\n')
                        {
                            ++currentIndex;
                        }

                        ++currentIndex;

                        string literal = sourceText.Substring(startIndex, currentIndex - startIndex + 1);

                        tokens.Add(new Token(literal, TokenType.StringConstant, TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;
                    case SimpleScanerTokenType.Number:
                        while (Regex.IsMatch(sourceText[currentIndex + 1].ToString(), "[0-9.]"))
                        {
                            ++currentIndex;
                        }

                        string number = sourceText.Substring(startIndex, currentIndex - startIndex + 1);

                        if (Regex.IsMatch(number, @"^[0-9]+.[0-9]*$"))
                            tokens.Add(new Token(number, TokenType.FloatConstant, TokenPosition.GetTokenPosition(sourceText, startIndex)));
                        else if (Regex.IsMatch(number, "^[0-9]+$"))
                            tokens.Add(new Token(number, TokenType.IntegerConstant, TokenPosition.GetTokenPosition(sourceText, startIndex)));
                        else
                            tokens.Add(new Token(number, TokenType.Unknown, TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;
                    case SimpleScanerTokenType.Operator:
                        while (_operators.Any(op => op.StartsWith(sourceText.Substring(startIndex, currentIndex - startIndex + 2))))
                        {
                            ++currentIndex;
                        }

                        string oper = sourceText.Substring(startIndex, currentIndex - startIndex + 1);

                        tokens.Add(new Token(oper, TokenType.Operator, TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;
                    case SimpleScanerTokenType.Unknown:
                        while (!_delimiters.Contains(sourceText[currentIndex + 1]))
                        {
                            ++currentIndex;
                        }

                        string unknownToken = sourceText.Substring(startIndex, currentIndex - startIndex + 1);

                        tokens.Add(new Token(unknownToken, TokenType.Unknown, TokenPosition.GetTokenPosition(sourceText, startIndex)));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                startIndex = currentIndex + 1;

            } while (true);

            return tokens;
        }
    }
}