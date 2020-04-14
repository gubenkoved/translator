using System.Collections.Generic;
using System.Linq;
using Parser.Core;

namespace FormalParser
{
    public class ControlTable
    {
        private Dictionary<Nonterminal, Dictionary<Terminal, Production>> _table;
        public int ElementsCount
        {
            get
            {
                return _table.Sum(kvp => kvp.Value.Count);
            }
        }

        public ControlTable()
        {
            _table = new Dictionary<Nonterminal, Dictionary<Terminal, Production>>();
        }

        /// <summary>
        /// Productions MUST BE:
        /// - left factored
        /// - without left recursions
        /// </summary>
        /// <param name="productions">Processed productions</param>
        public void FillByProcessedProductions(IEnumerable<Production> productions, Nonterminal axiom)
        {
            // A -> alpha
            foreach (Production production in productions)
            {
                // FIRST(alpha)
                ISet<Terminal> first = Helper.First(productions, production.Replacement);

                foreach (Terminal terminal in first)
                    this[production.NonTerminal, terminal] = production;

                if (first.Contains(GeneralizedTerminal.Epsilon))
                {
                    // FOLLOW(A)
                    ISet<Terminal> follow = Helper.Follow(productions, production.NonTerminal, axiom);

                    foreach (Terminal terminal in follow)
                        this[production.NonTerminal, terminal] = production;
                }
            }

        }

        public Production this[Nonterminal nt, Terminal t]
        {
            get
            {
                if (_table.ContainsKey(nt))
                {
                    foreach (var item in _table[nt])
                    {
                        if (item.Key.IsAppropriateTerminal(t))
                            return item.Value;
                    }
                }
                
                return null;
            }

            set
            {
                if (!_table.ContainsKey(nt))
                    _table.Add(nt, new Dictionary<Terminal, Production>());

                _table[nt][t] = value;
            }
        }

        public IDictionary<Terminal, Production> this[Nonterminal nt]
        {
            get
            {
                if (_table.ContainsKey(nt))
                    return _table[nt];

                return null;
            }
        }
    }
}
