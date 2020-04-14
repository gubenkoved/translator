using System.Collections.Generic;
using System.Diagnostics;
using FormalParser.MyGrammar;
using Lexer.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser.Core;

namespace ParserHelperTests
{
    [TestClass]
    public class FirstTest
    {
        [TestMethod]
        public void FirstFunctionTest()
        {
            var productions = MyLanguageGrammar.Productions;
            var FIRST_E = Helper.First(productions, MyNonterminals.EXPRESSION);
            var FIRST_E_DASH = Helper.First(productions, MyNonterminals.EXPRESSION_DASH);
            var FIRST_T = Helper.First(productions, MyNonterminals.TERM);
            var FIRST_T_DASH = Helper.First(productions, MyNonterminals.TERM_DASH);
            var FIRST_F = Helper.First(productions, MyNonterminals.FACTOR);

            Assert.IsTrue(FIRST_E.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.FloatConstant),
                new GeneralizedTerminal(TokenType.IntegerConstant),
                new GeneralizedTerminal(TokenType.Identifier),
                new GeneralizedTerminal(TokenType.Function),
                new ConcreteTerminal(new Token("(", TokenType.Operator))
            }));

            Assert.IsTrue(FIRST_E.SetEquals(FIRST_T));
            Assert.IsTrue(FIRST_E.SetEquals(FIRST_F));

            Assert.IsTrue(FIRST_E_DASH.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.Epsilon),
                new ConcreteTerminal(new Token("+", TokenType.Operator)),
                new ConcreteTerminal(new Token("-", TokenType.Operator))
            }));

            Assert.IsTrue(FIRST_T_DASH.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.Epsilon),
                new ConcreteTerminal(new Token("*", TokenType.Operator)),
                new ConcreteTerminal(new Token("/", TokenType.Operator))
            }));
        }

        [TestMethod]
        public void FirstFunctionForChainTest()
        {
            var productions = MyLanguageGrammar.Productions;
            var FIRST_E_DASH_T = Helper.First(productions, new SymbolsChain(MyNonterminals.EXPRESSION_DASH, MyNonterminals.TERM));

            Assert.IsTrue(FIRST_E_DASH_T.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.FloatConstant),
                new GeneralizedTerminal(TokenType.IntegerConstant),
                new GeneralizedTerminal(TokenType.Identifier),
                new GeneralizedTerminal(TokenType.Function),
                new ConcreteTerminal(new Token("(", TokenType.Operator)),
                new ConcreteTerminal(new Token("+", TokenType.Operator)),
                new ConcreteTerminal(new Token("-", TokenType.Operator))
            }));
        }

        [TestMethod]
        public void TraceSelectSet()
        {
            // to show SELECT set, run test in debug mode and see output from Debug (window Output)
            Trace.WriteLine("SELECT SET DUMP");

            foreach (var production in MyLanguageGrammar.Productions)
            {
                var first = Helper.First(MyLanguageGrammar.Productions, production.RightPart);

                Trace.WriteLine(string.Format("Select({0})={{{1}}}", production, string.Join(", ", first)));
            }
            
            Trace.Flush();
        }
    }
}
