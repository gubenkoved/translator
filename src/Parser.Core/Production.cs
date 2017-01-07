using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Core
{
    /// <summary>
    /// Production in the context free grammar
    /// </summary>
    public class Production
    {
        // E                        ->     a b C D 
        // ^ left part nonterminal         ^^^^^^^ right part symbols chain

        public Nonterminal LeftPart { get; private set; }
        public SymbolsChain RightPart { get; private set; }

        public Production(Nonterminal leftPart, SymbolsChain rightPart)
        {
            LeftPart = leftPart;

            RightPart = rightPart;
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", LeftPart, RightPart);
        }
    }
}
