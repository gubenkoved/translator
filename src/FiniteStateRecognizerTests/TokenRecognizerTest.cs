using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FiniteStateRecognizer;
using Lexer.Core;


namespace FiniteStateRecognizerTests
{
    [TestClass]
    public class TokenRecognizerTest
    {
        [TestMethod]
        public void KeywordsRecognizerTest1()
        {

            var keywordsRecognizer = TokenRecognizer.ExactMatchRecognizer(TokenType.Keyword, "str");

            keywordsRecognizer.Move('s');
            keywordsRecognizer.Move('t');
            keywordsRecognizer.Move('r');

            Assert.IsTrue(keywordsRecognizer.AchievableStates.Count() == 1, "Must be only one state");
            Assert.IsTrue(keywordsRecognizer.AchievableStates.Single().Type == StateType.Finite, "State must be finite");
        }

        [TestMethod]
        public void KeywordsRecognizerTest2()
        {
            var keywordsRecognizer = TokenRecognizer.ExactMatchRecognizer(TokenType.Keyword, "str");

            keywordsRecognizer.Move('s');
            keywordsRecognizer.Move('t');
            keywordsRecognizer.Move('r');
            keywordsRecognizer.Move(' ');

            Assert.IsTrue(keywordsRecognizer.AchievableStates.Count() == 0, "Must be empty for non-keyword token");
        }

        [TestMethod]
        public void KeywordsRecognizerTest3()
        {
            var keywordsRecognizer = TokenRecognizer.ExactMatchRecognizer(TokenType.Keyword, "str");

            keywordsRecognizer.Move('s');
            keywordsRecognizer.Move('t');

            Assert.IsTrue(keywordsRecognizer.AchievableStates.Count() == 1, "Must be only one state");
            Assert.IsTrue(keywordsRecognizer.AchievableStates.Single().Type == StateType.Nonfinite, "State must be non finite");
        }
    }
}
