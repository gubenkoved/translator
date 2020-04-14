using System.Collections.Generic;
using System.Linq;
using Lexer.Core;

namespace Parser.Core
{
    public class TokenStream : IEnumerable<Token>
    {
        private List<Token> _tokens;
        private int _currentIndex;

        public TokenStream(IEnumerable<Token> tokens)
        {
            _tokens = tokens.ToList();

            _currentIndex = 0;
        }

        public Token Current
        {
            get
            {
                if (_currentIndex < _tokens.Count)
                    return _tokens[_currentIndex];
                else
                    return null;
            }
        }

        public Token Last
        {
            get
            {
                if (_tokens.Count > 0)
                    return _tokens[_tokens.Count - 1];
                else
                    return null;
            }
        }

        public Token MoveNext()
        {
            if (_currentIndex < _tokens.Count)
            {
                ++_currentIndex;
                return Current;
            }
            else
            {
                return null;
            }
        }

        public Token Pop()
        {
            var current = Current;

            MoveNext();

            return current;
        }

        public TokenStreamState GetState()
        {
            return new TokenStreamState(_currentIndex);
        }

        public void SetState(TokenStreamState state)
        {
            _currentIndex = state._currentIndex;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return _tokens.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
