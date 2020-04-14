using System.Collections.Generic;
using FormalParser.MyGrammar;
using Lexer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser.Core;
using System.Linq;

namespace ParserHelperTests
{
    [TestClass]
    public class FollowTests
    {
        [TestMethod]
        public void FollowTest()
        {
            var productions = MyLanguageGrammar.Productions;
            var FOLLOW_T = Helper.Follow(productions, MyNonterminals.TERM, MyNonterminals.EXPRESSION);

            Assert.IsTrue(FOLLOW_T.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.EndOfText),
                new ConcreteTerminal(new Token(")", TokenType.Operator)),
                new ConcreteTerminal(new Token("+", TokenType.Operator)),
                new ConcreteTerminal(new Token("-", TokenType.Operator)),
                new ConcreteTerminal(new Token(",", TokenType.Operator)),
                new ConcreteTerminal(new Token(";", TokenType.Operator)),
                new ConcreteTerminal(new Token("==", TokenType.Operator)),
                new ConcreteTerminal(new Token("!=", TokenType.Operator)),
                new ConcreteTerminal(new Token(">=", TokenType.Operator)),
                new ConcreteTerminal(new Token("<=", TokenType.Operator)),
                new ConcreteTerminal(new Token(">", TokenType.Operator)),
                new ConcreteTerminal(new Token("<", TokenType.Operator)),
            }));
        }

        [TestMethod]
        public void FollowTest2()
        {
            var productions = MyLanguageGrammar.Productions;
            var FOLLOW_E = Helper.Follow(productions, MyNonterminals.EXPRESSION, MyNonterminals.FUNCTION);
        }
    }
}
