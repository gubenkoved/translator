using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;

namespace FormalParser
{
    public class ControlTable
    {
        private Dictionary<Nonterminal, Dictionary<Terminal, AutomateAction>> _table;
        public int ElementsCount
        {
            get
            {
                return _table.Sum(kvp => kvp.Value.Count);
            }
        }

        public ControlTable()
        {
            _table = new Dictionary<Nonterminal, Dictionary<Terminal, AutomateAction>>();
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
            foreach (var production in productions)
            {
                var useProductionAction = AutomateAction.FromProduction(production);

                // FIRST(alpha)
                var first = Helper.First(productions, production.RightPart);
                foreach (var terminal in first)
                {
                    this[production.LeftPart, terminal] = useProductionAction;
                }

                if (first.Contains(GeneralizedTerminal.Epsilon))
                {
                    // FOLLOW(A)
                    var follow = Helper.Follow(productions, production.LeftPart, axiom);
                    foreach (var terminal in follow)
                    {
                        this[production.LeftPart, terminal] = useProductionAction;
                    }                    
                }
            }

        }

        public AutomateAction this[Nonterminal nt, Terminal t]
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
                    _table.Add(nt, new Dictionary<Terminal, AutomateAction>());

                _table[nt][t] = value;
            }
        }

        public IDictionary<Terminal, AutomateAction> this[Nonterminal nt]
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
