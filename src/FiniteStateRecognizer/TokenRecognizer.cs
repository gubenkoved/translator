using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

namespace FiniteStateRecognizer
{
    public class TokenRecognizer
    {
        public int Priority { get; private set; }

        public TokenType TokenType { get; private set; }

        private List<Transition> _transitions;
        public IEnumerable<Transition> Transitions
        {
            get
            {
                return _transitions;
            }
        }

        private HashSet<State> _achievableStates;
        public IEnumerable<State> AchievableStates
        {
            get
            {
                return _achievableStates;
            }
        }

        private State _initState;

        public TokenRecognizer(TokenType tokenType, State initState, int priority = 0)
        {
            Priority = priority;

            _transitions = new List<Transition>();
            _achievableStates = new HashSet<State>();

            _initState = initState;
            _achievableStates.Add(_initState);

            TokenType = tokenType;
        }

        public void Reset()
        {
            _achievableStates.Clear();

            _achievableStates.Add(_initState);
        }

        /// <summary>
        /// Calculates achievable states after specified char
        /// </summary>
        /// <param name="c"></param>
        /// <returns>True if achievable states is not empty, and flase otherwise</returns>
        public bool Move(char c)
        {
            var newStates = new HashSet<State>();

            foreach (var state in AchievableStates)
            {
                var possible = _transitions.Where(t => t.StartState == state && t.Condition(c) == true)
                    .Select(t => t.EndState);

                possible.ForEach(s => newStates.Add(s));
            }

            _achievableStates = newStates;

            return _achievableStates.Count() != 0;
        }

        public void AddTransition(Transition transition)
        {
            _transitions.Add(transition);
        }

        public static TokenRecognizer ExactMatchRecognizer(TokenType tokenType, string target)
        {
            // example: keyword int
            //      i        n        t
            // (1) ---> (2) ---> (3) ---> ((4))
            // 3 transitions, 4 states

            var recognizer = new TokenRecognizer(tokenType, new State(1));

            for (int i = 1; i <= target.Length; i++)
            {
                var from = new State(i, StateType.Nonfinite);
                var to = new State(i + 1, i == target.Length ? StateType.Finite : StateType.Nonfinite);

                var transition = new Transition(from, to, CharTests.Is(target[i - 1]));

                recognizer.AddTransition(transition);
            }

            return recognizer;
        }

        public static IEnumerable<TokenRecognizer> GenerateExactMatchRecognizers(TokenType tokenType, IEnumerable<string> targets)
        {
            List<TokenRecognizer> recognizers = new List<TokenRecognizer>();

            foreach (string target in targets)
            {
                recognizers.Add(ExactMatchRecognizer(tokenType, target));
            }

            return recognizers;
        }

        public Token CreateInstance(string value, TokenPosition position)
        {
            return new Token(value, TokenType, position);
        }
    }
}
