using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Parser.Core;
using FormalParser;
using Lexer.Core;
using System.Diagnostics;

namespace ParserHelperTests
{
    [TestClass]
    public class FIRSTTest
    {
        [TestMethod]
        public void FirstFunctionTest()
        {
            var productions = MyLanguageGrammar.ProcessedProductions;
            var FIRST_E = Helper.First(productions, FormalNonterminals.EXPRESSION);
            var FIRST_E_DASH = Helper.First(productions, FormalNonterminals.EXPRESSION_DASH);
            var FIRST_T = Helper.First(productions, FormalNonterminals.TERM);
            var FIRST_T_DASH = Helper.First(productions, FormalNonterminals.TERM_DASH);
            var FIRST_F = Helper.First(productions, FormalNonterminals.FACTOR);

            Assert.IsTrue(FIRST_E.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.FloatConstant),
                new GeneralizedTerminal(TokenType.IntegerConstant),
                new GeneralizedTerminal(TokenType.Identifier),
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
            var productions = MyLanguageGrammar.ProcessedProductions;
            var FIRST_E_DASH_T = Helper.First(productions, new SymbolsChain(FormalNonterminals.EXPRESSION_DASH, FormalNonterminals.TERM));

            Assert.IsTrue(FIRST_E_DASH_T.SetEquals(new HashSet<Terminal>()
            {
                new GeneralizedTerminal(TokenType.FloatConstant),
                new GeneralizedTerminal(TokenType.IntegerConstant),
                new GeneralizedTerminal(TokenType.Identifier),
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

            foreach (var production in MyLanguageGrammar.ProcessedProductions)
            {
                var first = Helper.First(MyLanguageGrammar.ProcessedProductions, production.RightPart);

                Trace.WriteLine(string.Format("Select({0})={{{1}}}", production, string.Join(", ", first)));
            }
            
            Trace.Flush();
        }
    }
}
