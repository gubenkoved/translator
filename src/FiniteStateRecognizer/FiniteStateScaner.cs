using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

using Lexer.Core.Validation;

namespace FiniteStateRecognizer
{
    public class FiniteStateScaner : ScanerBase
    {
        private StateHistory _history;
        private List<TokenRecognizer> _tokenRecognizers;

        public FiniteStateScaner(IEnumerable<TokenRecognizer> recognizers)
            : base(TokenValidator.BasicValidator)
        {
            _history = new StateHistory();
            _tokenRecognizers = recognizers.ToList();
        }

        /// <summary>
        /// Recognizing keywords and functions
        /// </summary>
        private void PostRecognize(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].HasType(TokenType.Identifier))
                {
                    if (MyLanguage.Keywords.Contains(tokens[i].Value))
                    {
                        tokens[i] = new Token(tokens[i].Value, TokenType.Keyword, tokens[i].Position);
                    }

                    if (MyLanguage.Functions.Contains(tokens[i].Value))
                    {
                        tokens[i] = new Token(tokens[i].Value, TokenType.Function, tokens[i].Position);
                    }
                }
            }
        }

        protected override IEnumerable<Token> GetTokensImplementation(string sourceText)
        {
            var tokens = new List<Token>();

            int i = 0;
            int tokenStart = 0;
            
            while (i < sourceText.Length)
            {
                tokenStart = i;
                _history.Clear();

                // reset all recognizers
                _tokenRecognizers.ForEach(recognizer => recognizer.Reset());

                // go forward while achievable states are not empty
                bool isEndOfForwardPass = false;
                bool endOfInput = false;
                do
                {
                    _tokenRecognizers.ForEach(recognizer => recognizer.Move(sourceText[i]));
                    _history.AddStep(_tokenRecognizers.Select(r => new StateSet(r, r.AchievableStates)));

                    isEndOfForwardPass = _tokenRecognizers.All(recognizer => recognizer.AchievableStates.Count() == 0);

                    if (!isEndOfForwardPass && i < sourceText.Length - 1)
                        ++i;
                    else
                        endOfInput = true;

                } while (!isEndOfForwardPass && !endOfInput);

                // then, go backward (by history) to first finite state - we are found token
                Func<StateSet, bool> aq = stateSet => stateSet.HasFiniteState();
                while (!_history[i - tokenStart].Any(stateSet => stateSet.HasFiniteState()) && i > tokenStart)
                {
                    --i;
                }

                // selecting appropriate recognizer (which containts finite state)
                IEnumerable<StateSet> appropriateHistoryStep = _history[i - tokenStart];

                if (appropriateHistoryStep.Any(stateSet => stateSet.HasFiniteState()))
                {
                    var appropriateRecognizer
                        = appropriateHistoryStep.Where(stateSet => stateSet.HasFiniteState())
                        .Select(stateSet => stateSet.Recognizer)
                        .OrderByDescending(recognizer => recognizer.Priority)
                        .First();

                    TokenPosition position = TokenPosition.GetTokenPosition(sourceText, tokenStart);
                    tokens.Add(appropriateRecognizer.CreateInstance(sourceText.SubstringFromTo(tokenStart, i), position));

                    // go to next symbol
                    ++i;
                }
                else
                {
                    // finding end unrecognized token                    
                    while (_tokenRecognizers.All(recognizer => recognizer.AchievableStates.Count() == 0) && i < sourceText.Length)
                    {
                        ++i;

                        if (i < sourceText.Length)
                        {
                            _tokenRecognizers.ForEach(recognizer => recognizer.Reset());
                            _tokenRecognizers.ForEach(recognizer => recognizer.Move(sourceText[i]));
                        }
                    } 

                    TokenPosition position = TokenPosition.GetTokenPosition(sourceText, tokenStart);
                    tokens.Add(new Token(sourceText.SubstringFromTo(tokenStart, i - 1), TokenType.Unknown, position));
                }
            }

            PostRecognize(tokens);

            return tokens;
        }        
    }
}
