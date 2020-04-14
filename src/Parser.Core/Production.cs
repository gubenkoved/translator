namespace Parser.Core
{
    /// <summary>
    /// Production in the context free grammar
    /// </summary>
    public class Production
    {
        // E                        ->     a b C D 
        // ^ left part nonterminal         ^^^^^^^ right part symbols chain

        public Nonterminal NonTerminal { get; private set; }
        public SymbolsChain Replacement { get; private set; }

        public Production(Nonterminal nonTerminal, SymbolsChain replacement)
        {
            NonTerminal = nonTerminal;
            Replacement = replacement;
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", NonTerminal, Replacement);
        }
    }
}
