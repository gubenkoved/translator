using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;
using Lexer.Core;
using System.Reflection;
using Parser.Core.TreeWalker;

namespace FormalParser
{
    public class LL1Parser : ParserBase
    {
        enum StepResults
        {
            AcceptInput,
            RejectInput,
            InProgress
        }

        private ControlTable _controlTable;

        private Nonterminal _axiom;

        private Stack<Symbol> _stack;

        private TokenStream _tokenStream;

        private SyntaxTree _syntaxTree;

        public LL1Parser(Nonterminal axiom, ControlTable controlTable)
        {
            _axiom = axiom;

            _controlTable = controlTable;
        }

        private void Init(TokenStream ts)
        {
            _stack = new Stack<Symbol>();
            _stack.Push(_axiom);

            _tokenStream = ts;

            // axiom - root syntax tree node
            _syntaxTree = new SyntaxTree(new SyntaxTreeNode(_axiom.CreateCopy()));

            
            _replacedTerminalNodes = new HashSet<SyntaxTreeNode>();
        }

        private bool CanBeEpsilon(Nonterminal nonterminal)
        {
            if (_controlTable[nonterminal, GeneralizedTerminal.Epsilon] != null)
                return true;

            return false;
        }

        private StepResults Step(out Error error)
        {
            if (_stack.Count == 0)
            #region When stack is empty
            {
                if (_tokenStream.Current.Type == TokenType.EndOfText)
                #region ACCEPT
                {
                    error = null;
                    return StepResults.AcceptInput;
                }
                #endregion
                else
                #region REJECT: EOF EXPECTED
                {
                    error = new Error(_tokenStream.Current, ErrorKind.Syntax, "End of input expected");
                    return StepResults.RejectInput;
                }
                #endregion
            } 
            #endregion
            else
            #region When stack is not empty
            {
                Symbol magazineSymbol = _stack.Peek();
                ConcreteTerminal inputTerminal = new ConcreteTerminal(_tokenStream.Current);

                if (magazineSymbol is Nonterminal)
                {
                    Nonterminal magazineNonterminal = (Nonterminal)magazineSymbol;
                    Production production = _controlTable[magazineNonterminal, inputTerminal];

                    if (production == null && CanBeEpsilon(magazineNonterminal))
                    {
                        _stack.Pop();
                        error = null;
                        return StepResults.InProgress;
                    }
                    
                    if (production == null) // common error handling
                    {
                        error = new Error(_tokenStream.Current, ErrorKind.Syntax, GenerateErrorMessage(magazineNonterminal, inputTerminal));
                        return StepResults.RejectInput;
                    }

                    _stack.Pop(); // delete disclosing nonterminal

                    if (!production.RightPart.IsEpsilonChain)
                        production.RightPart.ReverseForEach(s => _stack.Push(s)); // and push right parts of productions

                    ConstructSyntaxTree(production);

                    error = null;
                    return StepResults.InProgress;
                }
                else // magazineSymbol is Terminal
                {
                    Terminal magazineTerminal = (Terminal)magazineSymbol;

                    if (magazineTerminal.IsAppropriateTerminal(inputTerminal))
                    {
                        _stack.Pop();
                        _tokenStream.MoveNext();

                        SubstituteTreeTerminalByActualValue(magazineTerminal, inputTerminal);

                        error = null;
                        return StepResults.InProgress;
                    }
                    else
                    {
                        error = new Error(_tokenStream.Current, ErrorKind.Syntax, GenerateErrorMessage(magazineTerminal, inputTerminal));
                        return StepResults.RejectInput;
                    }
                }
            }
            #endregion
        }

        private string GenerateErrorMessage(Terminal expectedTerminal, ConcreteTerminal inputTerminal)
        {
            return string.Format("{0} expected, but received {1}", expectedTerminal, inputTerminal);
        }

        private string GenerateErrorMessage(Nonterminal disclosingNonterminal, ConcreteTerminal inputTerminal)
        {
            var expected = GetExpectedTerminalsFor(disclosingNonterminal)
                .Except(new[] { GeneralizedTerminal.Epsilon, GeneralizedTerminal.EndOfText });                

            //return string.Format("{0} expected, but received {1} (disclosing: {2})", string.Join(" or ", expected), inputTerminal, disclosingNonterminal);
            return string.Format("{0} expected, but received {1} (start with one of {{{2}}})", disclosingNonterminal, inputTerminal, string.Join(", ", expected));
        }

        private IEnumerable<Terminal> GetExpectedTerminalsFor(Nonterminal nonterminal)
        {
            var expected = new List<Terminal>();

            foreach (KeyValuePair<Terminal, Production> kvp in _controlTable[nonterminal])
            {
                expected.Add(kvp.Key);
            }

            return expected;
        }

        private void ConstructSyntaxTree(Production usedProduction)
        {
            Nonterminal nonterminal = usedProduction.LeftPart;
            SyntaxTreeNode node = _syntaxTree.FindFirst(n => n.HasNoChildren && n.Value.Equals(nonterminal));

            foreach (var symbol in usedProduction.RightPart)
            {
                node.AddChild(new SyntaxTreeNode(symbol.CreateCopy()));
            }
        }

        /// <summary>
        /// Set of terminals which has been already replaced by concrete terminal
        /// Used by ref equality comparison
        /// </summary>
        private HashSet<SyntaxTreeNode> _replacedTerminalNodes;

        private void SubstituteTreeTerminalByActualValue(Terminal treeTerminal, ConcreteTerminal actualTerminal)
        {
            // ToDo: Replace this ugly and AWFUL PERFORMANCE code!

            var node = _syntaxTree.FindFirst(
                n => n.Value is Terminal 
                    && (n.Value as Terminal).Type != TokenType.Epsilon 
                    && !_replacedTerminalNodes.Contains(n));
            _replacedTerminalNodes.Add(node);
            node.Value = actualTerminal;
            
            
            //if (treeTerminal is GeneralizedTerminal)
            //{
            //    // replace first non epsilon generalized terminal
            //    var node = _syntaxTree.FindFirst(n => n.Value is GeneralizedTerminal && !n.Value.Equals(GeneralizedTerminal.Epsilon));
            //    node.Value = actualTerminal;
            //}
            //else
            //{
            //    // if Token.Position == null, then come from production
            //    var node = _syntaxTree.FindFirst(n => n.Value is ConcreteTerminal && (n.Value as ConcreteTerminal).Token.Position == null);
            //    node.Value = actualTerminal;
            //}
        }

        protected override SyntaxTree ParseImplementation(TokenStream ts, out IEnumerable<Error> syntaxErrors)
        {
            var errors = new List<Error>();
            syntaxErrors = errors;

            Init(ts);

            Error error;
            StepResults result;

            do
            {
                result = Step(out error);
            } while (result == StepResults.InProgress); // steping to end state (accept or reject)

            if (result == StepResults.RejectInput)
            {
                errors.Add(error);
                return null;
            }

            return _syntaxTree;
        }
    }
}