using System.Collections.Generic;
using System.Linq;
using Lexer.Core;
using Parser.Core;
using Parser.Core.TreeWalker;
using SemanticChecking;

namespace FormalParser.MyGrammar
{
    public class MySemanticChecker : SemanticCheckerBase
    {
        private const string DEFINDED_VARIABLES_ATTRIBUTE_KEY = "definded_vars";

        private List<Error> _errors;
        private Dictionary<string, int> _arity = new Dictionary<string, int>
        {
            {"sin", 1},
            {"cos", 1},
            {"tan", 1},
            {"exp", 1},
            {"ln", 1},
            {"abs", 1}
        };

        public MySemanticChecker()
        {
            _errors = new List<Error>();
        }

        public override IEnumerable<Error> GetSemanticErrors(SyntaxTree tree)
        {
            _errors.Clear();
            
            ProcessTree(tree);

            return _errors;
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

        private void TunnelingProcessNode(SyntaxTreeNode node)
        {
            if (node.Value.Equals(MyNonterminals.STATEMENTS_BLOCK))
            {
                node.Value[DEFINDED_VARIABLES_ATTRIBUTE_KEY] = new List<Token>();
            }
        }

        private void BubblingProcessNode(SyntaxTreeNode node)
        {
            if (node.Value.Equals(MyNonterminals.ASSIGN_STATEMENT))
            {
                var idDeclaration = node[MyNonterminals.ID_DECLARATION];

                if (idDeclaration != null)
                {
                    var activeScope = Parser.Core.TreeWalker.WalkHelper.FindParentWithNonterminal(node, MyNonterminals.STATEMENTS_BLOCK);
                    var idToken = (idDeclaration[TokenType.Identifier].Value as ConcreteTerminal).Token;

                    AddDefindedVariableAttrubute(activeScope, idToken);

                    var typeToken = (idDeclaration[MyNonterminals.TYPE][0].Value as ConcreteTerminal).Token;

                    if (typeToken.Value == "void")
                        _errors.Add(new Error(typeToken, ErrorKind.Semantic, "Void can not be used as variable type"));
                }
                else // id = E
                {
                    var id = (node[TokenType.Identifier].Value as ConcreteTerminal).Token;

                    CheckVariableDefinition(node, id);
                }
            }

            if (node.Value.Equals(MyNonterminals.FACTOR) && node[TokenType.Identifier] != null)
            {
                var id = (node[TokenType.Identifier].Value as ConcreteTerminal).Token;

                CheckVariableDefinition(node, id);
            }

            if (node.Value.Equals(MyNonterminals.FUNCTION_CALL))
            {
                CheckArity(node);
            }
        }

        private void CheckArity(SyntaxTreeNode funcCallNode)
        {
            var funcToken = (funcCallNode[TokenType.Function].Value as ConcreteTerminal).Token;
            var paramListNode = funcCallNode[MyNonterminals.PARAM_BLOCK][MyNonterminals.PARAM_LIST];
            int arity = CalcArity(paramListNode);

            if (_arity.ContainsKey(funcToken.Value) && _arity[funcToken.Value] != arity)
            {
                _errors.Add(new Error(funcToken, ErrorKind.Semantic, string.Format("Arity discrepancy: {0} allows {1} parameters", funcToken.Value, _arity[funcToken.Value])));
            }
        }

        private int CalcArity(SyntaxTreeNode node)
        {
            if (node.IsEpsilon)
                return 0;
            else
                return 1 + CalcArity(node[MyNonterminals.PARAM_LIST_DASH]);
        }

        private void CheckVariableDefinition(SyntaxTreeNode node, Token id)
        {
            if (!VariableDefinded(node, id))
                _errors.Add(new Error(id, ErrorKind.Semantic, "Using undefinded in this scope variable"));
        }

        private bool VariableDefinded(SyntaxTreeNode node, Token id)
        {
            var activeScopes = Parser.Core.TreeWalker.WalkHelper.GetAllParents(node)
                .Select(n => n.Value)
                .Where(s => s.Equals(MyNonterminals.STATEMENTS_BLOCK))
                .ToList();

            if (node.Value.Equals(MyNonterminals.STATEMENTS_BLOCK))
                activeScopes.Add(node.Value);

            return activeScopes.Any(scope =>
                {
                    var defindedVars = scope[DEFINDED_VARIABLES_ATTRIBUTE_KEY] as List<Token>;
                    return defindedVars.Where(v => v.Value == id.Value).Count() != 0;
                });
        }

        private void AddDefindedVariableAttrubute(SyntaxTreeNode statementsBlockNode, Token id)
        {
            var defindedVars = statementsBlockNode.Value[DEFINDED_VARIABLES_ATTRIBUTE_KEY] as List<Token>;

            if (!VariableDefinded(statementsBlockNode, id))
                defindedVars.Add(id);
            else
                _errors.Add(new Error(id, ErrorKind.Semantic, "Active scope already contains variable with specified name"));
        }


    }
}
