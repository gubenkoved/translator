using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;
using Lexer.Core;
using SemanticChecking;
using Parser.Core.TreeWalker;


namespace RecursiveDescentParser
{
    public class ASTValidator : SemanticCheckerBase
    {
        private SyntaxTree _syntaxTree;
        private List<Error> _errors;
        private HashSet<string> _defindedVariables;

        public ASTValidator()
        {
            
        }

        private void TunnelingProcessNode(SyntaxTreeNode node)
        {
            if (node.Value is Nonterminal)
            {
                Nonterminal nonterminal = node.Value as Nonterminal;
            } else
            {
                ConcreteTerminal terminal = node.Value as ConcreteTerminal;
            }
        }

        private void BubblingProcessNode(SyntaxTreeNode node)
        {
            if (node.Value is Nonterminal)
            {
                Nonterminal nonterminal = node.Value as Nonterminal;
                
                if (nonterminal.Equals(MyNonterminals.ASSIGN_STATEMENT) && node.ChildrenCount == 4)
                {
                    var idToken = (node[1].Value as ConcreteTerminal).Token;
                    string defVariableName = idToken.Value;

                    if (!_defindedVariables.Contains(defVariableName))
                        _defindedVariables.Add(defVariableName);
                    else
                        _errors.Add(new Error(idToken, ErrorKind.Semantic, "Variable redefinition"));
                }
            } else
            {
                ConcreteTerminal terminal = node.Value as ConcreteTerminal;

                if (terminal.Token.HasType(TokenType.Identifier) && !_defindedVariables.Contains(terminal.Token.Value))
                {
                    if (((Nonterminal)node.Parent.Value).Equals(MyNonterminals.FACTOR))
                    {
                        _errors.Add(new Error(terminal.Token, ErrorKind.Semantic, "Undefined variable"));
                    }
                }
            }
        }

        private void ProcessTree(SyntaxTree tree)
        {
            var walkOrder = DirectOrderWalker.GetWalkOrder(tree);

            foreach (var item in walkOrder)
            {
                if (item.Direction == WalkStepDirection.Bubbling)
                    BubblingProcessNode(item.Node);

                if (item.Direction == WalkStepDirection.Tunelling)
                    TunnelingProcessNode(item.Node);
            }
        }

        public override IEnumerable<Error> GetSemanticErrors(SyntaxTree tree)
        {
            _syntaxTree = tree;
            _errors = new List<Error>();
            _defindedVariables = new HashSet<string>();

            ProcessTree(_syntaxTree);

            return _errors;
        }
    }
}
