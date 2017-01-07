using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormalParser;
using Parser.Core;
using Lexer.Core;

namespace ParserHelperTests
{
    [TestClass]
    public class FollowTests
    {
        [TestMethod]
        public void FollowTest()
        {
            var productions = MyLanguageGrammar.ProcessedProductions;
            var FOLLOW_T = Helper.Follow(productions, FormalNonterminals.TERM, FormalNonterminals.EXPRESSION);

            Assert.IsTrue(FOLLOW_T.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.EndOfText),
                new ConcreteTerminal(new Token(")", TokenType.Operator)),
                new ConcreteTerminal(new Token("+", TokenType.Operator)),
                new ConcreteTerminal(new Token("-", TokenType.Operator))
            }));
        }

        [TestMethod]
        public void FollowTest2()
        {
            var productions = MyLanguageGrammar.ProcessedProductions;
            var FOLLOW_E = Helper.Follow(productions, FormalNonterminals.EXPRESSION, FormalNonterminals.FUNCTION);
        }        
    }
}
