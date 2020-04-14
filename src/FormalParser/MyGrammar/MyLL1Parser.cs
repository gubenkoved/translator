using Parser.Core;

namespace FormalParser.MyGrammar
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
                
                Nonterminal axiom = MyNonterminals.FUNCTION;
                ControlTable ct = new ControlTable();

                ct.FillByProcessedProductions(MyLanguageGrammar.Productions, axiom);

                _instance = new LL1Parser(axiom, ct);

                return _instance;
            }
        }
    }
}
