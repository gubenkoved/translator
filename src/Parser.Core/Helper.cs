using System.Collections.Generic;
using System.Linq;

namespace Parser.Core
{
    public static class Helper
    {
        private static ISet<Terminal> ExceptTerminal(this ISet<Terminal> set, Terminal terminal)
        {
            return new HashSet<Terminal>(set.Except(new[] { terminal }));
        }

        private static HashSet<Terminal> CreateNewEmptyTerminalSet()
        {
            return new HashSet<Terminal>();
        }

        public static ISet<Terminal> First(IEnumerable<Production> productions, Symbol symbol)
        {
            var first = CreateNewEmptyTerminalSet();

            if (symbol is Terminal)
            {
                first.Add(symbol as Terminal);
            }
            else
            {
                // A -> ..., A - specified symbol
                var appropriateProductions = productions.Where(p => p.NonTerminal.Kind == (symbol as Nonterminal).Kind);

                foreach (var prod in appropriateProductions)
                {
                    IEnumerator<Symbol> enumerator = prod.Replacement.GetEnumerator();
                    bool allSymbolsIsEpsilonGenerating = true;

                    while (enumerator.MoveNext())
                    {
                        var curSymbolFirst = First(productions, enumerator.Current);

                        first.UnionWith(curSymbolFirst.ExceptTerminal(GeneralizedTerminal.Epsilon));

                        if (!curSymbolFirst.Contains(GeneralizedTerminal.Epsilon))
                        {
                            allSymbolsIsEpsilonGenerating = false;
                            break;
                        }
                    }

                    if (allSymbolsIsEpsilonGenerating)
                    {
                        first.Add(GeneralizedTerminal.Epsilon);
                    }
                }
            }

            return first;
        }

        public static ISet<Terminal> First(IEnumerable<Production> productions, SymbolsChain chain)
        {
            var first = CreateNewEmptyTerminalSet();

            bool allSymbolsIsEpsilonGenerating = true;
            var enumerator = chain.GetEnumerator();

            while (enumerator.MoveNext())
            {
                ISet<Terminal> curFirst = First(productions, enumerator.Current);
                first.UnionWith(curFirst.ExceptTerminal(GeneralizedTerminal.Epsilon));

                if (!curFirst.Contains(GeneralizedTerminal.Epsilon))
                {
                    allSymbolsIsEpsilonGenerating = false;
                    break;
                }
            }

            if (allSymbolsIsEpsilonGenerating)
                first.Add(GeneralizedTerminal.Epsilon);

            return first;
        }

        public static ISet<Terminal> Follow(IEnumerable<Production> productions, Nonterminal nonterminal, Nonterminal axiom)
        {
            var follow = CreateNewEmptyTerminalSet();

            if (nonterminal.Equals(axiom))
            {
                follow.Add(GeneralizedTerminal.EndOfText);
            }

            // X -> ... A ..., A - specified symbol
            var appropriateProductions = productions.Where(p => p.Replacement.Contains(nonterminal));

            foreach (var production in appropriateProductions)
            {
                // X -> ... A
                if (production.Replacement.Last.Equals(nonterminal))
                {
                    // preventing infinite follow call
                    if (!production.NonTerminal.Equals(nonterminal))
                        follow.UnionWith(Follow(productions, production.NonTerminal, axiom));
                }

                foreach (var rightSideChain in RightSideChains(production.Replacement, nonterminal))
                //var rightSideChain = FirstRightSideChain(production.RightPart, nonterminal);
                {
                    // X -> ... A g
                    //if (!production.RightPart.Last.Equals(nonterminal))
                    //{
                        follow.UnionWith(First(productions, rightSideChain).ExceptTerminal(GeneralizedTerminal.Epsilon));
                    //}

                    // X -> ... A h, where h is epsilon generating chain
                    if (First(productions, rightSideChain).Contains(GeneralizedTerminal.Epsilon))
                    {
                        // preventing infinite follow call
                        if (!production.NonTerminal.Equals(nonterminal))
                            follow.UnionWith(Follow(productions, production.NonTerminal, axiom));
                    }
                }
            }

            return follow;
        }

        private static SymbolsChain FirstRightSideChain(SymbolsChain chain, Nonterminal nonterminal)
        {
            int index = chain.FindIndex(nonterminal);

            if (index == -1)
                return null;

            return chain.Subchain(index + 1);
        }

        private static IEnumerable<SymbolsChain> RightSideChains(SymbolsChain chain, Nonterminal nonterminal)
        {
            var rightChains = new List<SymbolsChain>();

            SymbolsChain current = chain;

            do
            {
                current = FirstRightSideChain(current, nonterminal);

                if (current != null)
                    rightChains.Add(current);
                else
                    break;

            } while (true);

            return rightChains;
        }
    }
}
