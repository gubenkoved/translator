using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;
using CodeGeneration;
using Parser.Core.TreeWalker;

namespace RecursiveDescentParser
{
    public class ASTCodegenerator : GeneratorBase
    {
        private const string VAR_KEY = "var";
        private const string LEXVAL_KEY = "lexval";
        private const string LABEL_KEY = "label";
        private const string IF_BEGIN_LABEL_KEY = "if_begin_label";
        private const string IF_END_LABEL_KEY = "if_end_label";

        private SyntaxTree _syntaxTree;
        private IntermediateCode _code;

        public ASTCodegenerator()
        {
        }

        public override IntermediateCode GenerateIntermediateCode(SyntaxTree tree)
        {
            _syntaxTree = tree;
            _code = new IntermediateCode();

            ProcessTree(_syntaxTree);

            return _code;
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

        private Operand GetLexvalOrVarOperand(SyntaxTreeNode node)
        {
            if (node.Value[LEXVAL_KEY] != null)
                return new Operand(node.Value[LEXVAL_KEY]);
            else if (node.Value[VAR_KEY] != null)
                return node.Value[VAR_KEY] as Operand;

            throw new Exception("VAR and LEXVAL attributes is not definded");
        }

        private void TunnelingProcessNode(SyntaxTreeNode node)
        {
            if (!node.HasNoChildren && node.Value is Nonterminal)
            {
                var nonterminal = node.Value as Nonterminal;

                if (nonterminal.Equals(MyNonterminals.IF_STATEMENT))
                {
                    var ifBeginLabel = _code.Helper.GetTempLabel();
                    var ifEndLabel = _code.Helper.GetTempLabel();

                    node.Value.Attributes[IF_BEGIN_LABEL_KEY] = ifBeginLabel;
                    node.Value.Attributes[IF_END_LABEL_KEY] = ifEndLabel;
                }
                else if (nonterminal.Equals(MyNonterminals.THEN_BLOCK) || nonterminal.Equals(MyNonterminals.ELSE_BLOCK))
                {
                    if (nonterminal.Equals(MyNonterminals.THEN_BLOCK))
                    {
                        _code.Emit(new Instruction(OpCode.JMP, node.Parent.Value[IF_BEGIN_LABEL_KEY] as Operand, null, null));
                    }

                    var blockBeginLabel = _code.Helper.GetTempLabel();

                    node.Value.Attributes[LABEL_KEY] = blockBeginLabel;

                    _code.Emit(new Instruction(OpCode.LTRGT, blockBeginLabel, null, null));
                }
            }
        }

        private void BubblingProcessNode(SyntaxTreeNode node)
        {
            if (!node.HasNoChildren && node.Value is Nonterminal)
            {
                var nonterminal = node.Value as Nonterminal;

                if (nonterminal.Equals(MyNonterminals.FACTOR))
                {
                    if (node.ChildrenCount == 1 && node[0].Value is ConcreteTerminal) // F -> const
                        node.Value[LEXVAL_KEY] = ((ConcreteTerminal)node[0].Value).Token.Value;
                    else if (node.ChildrenCount == 3) // F -> ( E )
                        node.Value[VAR_KEY] = node[1].Value[VAR_KEY];
                    else if (node.ChildrenCount == 1 && node[0].Value is Nonterminal) // F -> FUNC_CALL
                        node.Value[VAR_KEY] = node[0].Value[VAR_KEY];

                }
                else if (nonterminal.Equals(MyNonterminals.TERM) || nonterminal.Equals(MyNonterminals.EXPRESSION))
                {
                    // T -> F T'
                    // E -> T E'

                    if (node[1].HasNoChildren)
                    {
                        if (node[0].Value[VAR_KEY] != null)
                            nonterminal[VAR_KEY] = node[0].Value[VAR_KEY];
                        else
                            nonterminal[LEXVAL_KEY] = node[0].Value[LEXVAL_KEY];
                    }
                    else
                        nonterminal[VAR_KEY] = GetLexvalOrVarOperand(node[1]); // T' or E'
                }
                else if (nonterminal.Equals(MyNonterminals.EXPRESSION_DASH) || nonterminal.Equals(MyNonterminals.TERM_DASH))
                {
                    // E' -> +- T E'
                    // T' -> */ F T'

                    Operand resultVar = _code.Helper.GetTempVariable();
                    Operand arg1 = GetLexvalOrVarOperand(node.LeftNode()); // T for E', F for T'
                    Operand arg2 = node[2].HasNoChildren ? GetLexvalOrVarOperand(node[1]) : GetLexvalOrVarOperand(node[2]);

                    nonterminal[VAR_KEY] = resultVar;

                    OpCode opCode;

                    if (node[0].Value.IsTerminalWithValue("+")) opCode = OpCode.ADD;
                    else if (node[0].Value.IsTerminalWithValue("-")) opCode = OpCode.SUB;
                    else if (node[0].Value.IsTerminalWithValue("*")) opCode = OpCode.MULT;
                    else if (node[0].Value.IsTerminalWithValue("/")) opCode = OpCode.DIV;
                    else throw new Exception("Unknown operator");

                    _code.Emit(new Instruction(opCode, arg1, arg2, resultVar));
                }
                else if (nonterminal.Equals(MyNonterminals.PARAM))
                {
                    Operand param = GetLexvalOrVarOperand(node[0]);
                    
                    _code.Emit(new Instruction(OpCode.PARAM, param, null, null));
                }
                else if (nonterminal.Equals(MyNonterminals.FUNCTION_CALL))
                {
                    Operand resultVar = _code.Helper.GetTempVariable();
                    Operand funcName = new Operand(((ConcreteTerminal)node[0].Value).Token.Value);

                    nonterminal[VAR_KEY] = resultVar;

                    _code.Emit(new Instruction(OpCode.CALL, funcName, null, resultVar));
                }
                else if (nonterminal.Equals(MyNonterminals.ASSIGN_STATEMENT))
                {
                    int idChildNum = node.ChildrenCount == 3 ? 0 : 1;
                    
                    Operand resultVar = new Operand( (node[idChildNum].Value as ConcreteTerminal).Token.Value);
                    Operand value = GetLexvalOrVarOperand(node[MyNonterminals.EXPRESSION]);

                    _code.Emit(new Instruction(OpCode.ASGN, value, null, resultVar));
                }
                else if (nonterminal.Equals(MyNonterminals.THEN_BLOCK) || nonterminal.Equals(MyNonterminals.ELSE_BLOCK))
                {
                    var ifEndLabel = node.Parent.Value[IF_END_LABEL_KEY] as Operand;

                    _code.Emit(new Instruction(OpCode.JMP, ifEndLabel, null, null));
                }
                else if (nonterminal.Equals(MyNonterminals.IF_STATEMENT))
                {
                    var bool_expression = node[MyNonterminals.IF_BLOCK][MyNonterminals.BOOL_EXPRESSION];

                    Operand thenBlockLabel = node[MyNonterminals.THEN_BLOCK].Value[LABEL_KEY] as Operand;
                    Operand elseBlockLabel = null;

                    if (!node[MyNonterminals.ELSE_BLOCK].HasNoChildren)
                        elseBlockLabel = node[MyNonterminals.ELSE_BLOCK].Value[LABEL_KEY] as Operand;

                    Operand leftBoolPart = GetLexvalOrVarOperand(bool_expression[0]);
                    Operand rightBoolPart = GetLexvalOrVarOperand(bool_expression[2]);
                    OpCode opCode;

                    if (bool_expression[1].Value.IsTerminalWithValue(">")) opCode = OpCode.JGT;
                    else if (bool_expression[1].Value.IsTerminalWithValue("<")) opCode = OpCode.JLT;
                    else if (bool_expression[1].Value.IsTerminalWithValue(">=")) opCode = OpCode.JGE;
                    else if (bool_expression[1].Value.IsTerminalWithValue("<=")) opCode = OpCode.JLE;
                    else if (bool_expression[1].Value.IsTerminalWithValue("==")) opCode = OpCode.JEQ;
                    else if (bool_expression[1].Value.IsTerminalWithValue("!=")) opCode = OpCode.JNEQ;
                    else throw new Exception("Unknown boolean operator");

                    _code.Emit(new Instruction(OpCode.LTRGT, node.Value[IF_BEGIN_LABEL_KEY] as Operand, null, null));
                    _code.Emit(new Instruction(opCode, leftBoolPart, rightBoolPart, thenBlockLabel));
                    if (elseBlockLabel != null) _code.Emit(new Instruction(OpCode.JMP, elseBlockLabel, null, null));
                    _code.Emit(new Instruction(OpCode.LTRGT, node.Value[IF_END_LABEL_KEY] as Operand, null, null));
                }
            }
        }
    }
}
