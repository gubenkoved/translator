using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;

namespace FormalParser
{
    public static class MyLL1Parser
    {
        private static LL1Parser _instance;
        public static LL1Parser Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                
                Nonterminal axiom = FormalNonterminals.FUNCTION;
                ControlTable ct = new ControlTable();

                ct.FillByProcessedProductions(MyLanguageGrammar.ProcessedProductions, axiom);

                _instance = new LL1Parser(axiom, ct);

                return _instance;
            }
        }
    }
}
