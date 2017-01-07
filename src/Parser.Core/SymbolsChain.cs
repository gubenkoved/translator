using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Core
{
    public class SymbolsChain : IEnumerable<Symbol>
    {
        private List<Symbol> _symbols;
        public int Length
        {
            get
            {
                return _symbols.Count;
            }
        }

        public SymbolsChain(params Symbol[] symbols)
        {
            _symbols = new List<Symbol>();

            foreach (var symbol in symbols)
            {
                _symbols.Add(symbol);
            }
        }
        public SymbolsChain(IEnumerable<Symbol> symbols)
        {
            _symbols = new List<Symbol>(symbols);
        }

        public bool IsEpsilonChain
        {
            get
            {
                return _symbols.Count == 1 
                    && (_symbols[0] is Terminal)
                    && ((Terminal)_symbols[0]).Type == Lexer.Core.TokenType.Epsilon;
            }
        }

        public Symbol this[int index]
        {
            get
            {
                return _symbols[index];
            }
        }
        public Symbol Last
        {
            get
            {
                return _symbols[_symbols.Count - 1];
            }
        }

        public SymbolsChain Subchain(int fromIndex)
        {
            if (fromIndex >= Length || fromIndex < 0)
                return null;

            return new SymbolsChain(_symbols.Skip(fromIndex));
        }
        public int FindIndex(Symbol symbol)
        {
            return _symbols.FindIndex(s => s.Equals(symbol));
        }

        public IEnumerator<Symbol> GetEnumerator()
        {
            return _symbols.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(" ", _symbols);
        }
    }
}