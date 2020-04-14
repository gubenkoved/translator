using CodeGeneration;
using Lexer.Core;
using Parser.Core;
using Parser.Core.TreeWalker;


namespace FormalParser.MyGrammar
{
    public class MyGenerator : GeneratorBase
    {
        private const string VALUE_KEY = "value";
        private const string END_BLOCK_LABEL_KEY = "end_block_label";
        private const string BEGIN_BLOCK_LABEL_KEY = "begin_block_label";

        private SyntaxTree _syntaxTree;
        private IntermediateCode _code;

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
                else
                    TunnelingProcessNode(item.Node);
            }
        }

        private void TunnelingProcessNode(SyntaxTreeNode node)
        {
            if (node.Value is Nonterminal && !node.IsEpsilon)
            {
                var nonterminal = node.Value as Nonterminal;

                if (nonterminal.Equals(MyNonterminals.IF_STATEMENT))
                {                    
                    node.Value[END_BLOCK_LABEL_KEY] = _code.Helper.GetTempLabel();

                    if (!node[MyNonterminals.ELSE_BLOCK].IsEpsilon)
                        node[MyNonterminals.ELSE_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY] = _code.Helper.GetTempLabel();
                }
                
                if (nonterminal.Equals(MyNonterminals.ELSE_BLOCK))
                {
                    _code.Emit(new Instruction(OpCode.LTRGT, (Operand)node.Value[BEGIN_BLOCK_LABEL_KEY], null, null));
                }

                if (nonterminal.Equals(MyNonterminals.FOR_STATEMENT))
                {
                    node.Value[END_BLOCK_LABEL_KEY] = _code.Helper.GetTempLabel();

                    node[MyNonterminals.FOR_TEST_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY] = _code.Helper.GetTempLabel();
                    node[MyNonterminals.FOR_STEP_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY] = _code.Helper.GetTempLabel();
                    node[MyNonterminals.FOR_BODY_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY] = _code.Helper.GetTempLabel();
                }

                if (nonterminal.Equals(MyNonterminals.FOR_TEST_BLOCK) || nonterminal.Equals(MyNonterminals.FOR_STEP_BLOCK) || nonterminal.Equals(MyNonterminals.FOR_BODY_BLOCK))
                {
                    _code.Emit(new Instruction(OpCode.LTRGT, (Operand)node.Value[BEGIN_BLOCK_LABEL_KEY], null, null));
                }
            }
        }

        private Operand GetOperand(SyntaxTreeNode node)
        {
            if (node == null)
                return null;

            var val = GetValue(node);
            if (val != null)
                return new Operand(val);
            else
                return null;
        }

        private object GetValue(SyntaxTreeNode node)
        {
            return node.Value[VALUE_KEY];
        }

        private OpCode GetCondJumpInvertOpCode(Symbol boolOp)
        {
            OpCode opCode = OpCode.Undefinded;
            
            if (boolOp.IsTerminalWithValue(">")) opCode = OpCode.JLE;
            else if (boolOp.IsTerminalWithValue("<")) opCode = OpCode.JGE;
            else if (boolOp.IsTerminalWithValue(">=")) opCode = OpCode.JLT;
            else if (boolOp.IsTerminalWithValue("<=")) opCode = OpCode.JGT;
            else if (boolOp.IsTerminalWithValue("==")) opCode = OpCode.JNEQ;
            else if (boolOp.IsTerminalWithValue("!=")) opCode = OpCode.JEQ;

            return opCode;
        }

        private void BubblingProcessNode(SyntaxTreeNode node)
        {
            if (node.Value is Nonterminal && !node.IsEpsilon)
            {
                var nonterminal = (Nonterminal)node.Value;

                if (nonterminal.Equals(MyNonterminals.FACTOR))
                {
                    // F -> const | ( E ) | func-call | id
                    if (node[TokenType.IntegerConstant] != null)
                        nonterminal[VALUE_KEY] = ((ConcreteTerminal)node[TokenType.IntegerConstant].Value).Token;
                    else if (node[TokenType.FloatConstant] != null)
                        nonterminal[VALUE_KEY] = ((ConcreteTerminal)node[TokenType.FloatConstant].Value).Token;
                    else if (node[TokenType.Identifier] != null)
                        nonterminal[VALUE_KEY] = ((ConcreteTerminal)node[TokenType.Identifier].Value).Token;
                    else if (node[MyNonterminals.EXPRESSION] != null)
                        nonterminal[VALUE_KEY] = GetValue(node[MyNonterminals.EXPRESSION]);
                    else 
                        nonterminal[VALUE_KEY] = GetValue(node[MyNonterminals.FUNCTION_CALL]);
                }

                if (nonterminal.Equals(MyNonterminals.TERM) || nonterminal.Equals(MyNonterminals.EXPRESSION))
                {
                    // T -> F T', E -> T E'
                    nonterminal[VALUE_KEY] = GetValue(node[1]) ?? GetValue(node[0]);
                }

                if (nonterminal.Equals(MyNonterminals.EXPRESSION_DASH) || nonterminal.Equals(MyNonterminals.TERM_DASH))
                {
                    // E' -> + T E' | - T E', T' -> * F T' | / F T'
                    Operand resultVar = _code.Helper.GetTempVariable();
                    Operand arg1 = GetOperand(node.Parent[MyNonterminals.TERM]) ?? GetOperand(node.Parent[MyNonterminals.FACTOR]); // T for E', F for T'
                    Operand arg2 = GetOperand(node[2]) ?? GetOperand(node[1]);

                    nonterminal[VALUE_KEY] = resultVar;

                    OpCode opCode = OpCode.Undefinded;
                    ConcreteTerminal arithmeticOp = node[TokenType.Operator].Value as ConcreteTerminal;

                    if (arithmeticOp.Token.Value == "+") opCode = OpCode.ADD;
                    else if (arithmeticOp.Token.Value == "-") opCode = OpCode.SUB;
                    else if (arithmeticOp.Token.Value == "*") opCode = OpCode.MULT;
                    else if (arithmeticOp.Token.Value == "/") opCode = OpCode.DIV;

                    _code.Emit(new Instruction(opCode, arg1, arg2, resultVar));
                }

                if (nonterminal.Equals(MyNonterminals.PARAM))
                {
                    // param -> E
                    Operand param = GetOperand(node[MyNonterminals.EXPRESSION]);

                    _code.Emit(new Instruction(OpCode.PARAM, param, null, null));
                }

                if (nonterminal.Equals(MyNonterminals.FUNCTION_CALL))
                {
                    // func-call -> function ( param-list )
                    Operand resultVar = _code.Helper.GetTempVariable();
                    Operand funcName = new Operand(((ConcreteTerminal)node[TokenType.Function].Value).Token.Value);
                    
                    nonterminal[VALUE_KEY] = resultVar;

                    _code.Emit(new Instruction(OpCode.CALL, funcName, null, resultVar));
                }

                #region ASSIGN-STATEMENT
                if (nonterminal.Equals(MyNonterminals.ASSIGN_STATEMENT))
                {
                    // assign -> id-decl = E | id = E
                    var idSymbol = node[MyNonterminals.ID_DECLARATION] != null ?
                        node[MyNonterminals.ID_DECLARATION][TokenType.Identifier]
                        : node[TokenType.Identifier];

                    Operand resultVar = new Operand(((ConcreteTerminal)idSymbol.Value).Token.Value);
                    Operand value = GetOperand(node[MyNonterminals.EXPRESSION]);

                    _code.Emit(new Instruction(OpCode.ASGN, value, null, resultVar));
                } 
                #endregion

                #region IF-STATEMENT
                if (nonterminal.Equals(MyNonterminals.IF_BLOCK))
                {
                    // bool-E -> E op E
                    // condition will be inverted, and when it equals to false goto else block (or end of if block)

                    var boolExp = node[MyNonterminals.BOOL_EXPRESSION];
                    var boolOp = boolExp[MyNonterminals.BOOL_OPERATOR][TokenType.Operator].Value;

                    Operand falseDestination;
                    if (!node.Parent[MyNonterminals.ELSE_BLOCK].IsEpsilon)
                        falseDestination = (Operand)node.Parent[MyNonterminals.ELSE_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY];
                    else
                        falseDestination = (Operand)node.Parent.Value[END_BLOCK_LABEL_KEY];

                    _code.Emit(new Instruction(GetCondJumpInvertOpCode(boolOp), GetOperand(boolExp[0]), GetOperand(boolExp[2]), falseDestination));
                }

                if (nonterminal.Equals(MyNonterminals.IF_STATEMENT))
                {
                    _code.Emit(new Instruction(OpCode.LTRGT, (Operand)node.Value[END_BLOCK_LABEL_KEY], null, null));
                }

                if (nonterminal.Equals(MyNonterminals.THEN_BLOCK))
                {
                    _code.Emit(new Instruction(OpCode.JMP, (Operand)node.Parent.Value[END_BLOCK_LABEL_KEY], null, null));
                } 
                #endregion

                #region FOR-STATEMENT
                if (nonterminal.Equals(MyNonterminals.FOR_STATEMENT))
                {
                    _code.Emit(new Instruction(OpCode.LTRGT, (Operand)node.Value[END_BLOCK_LABEL_KEY], null, null));
                }

                if (nonterminal.Equals(MyNonterminals.FOR_TEST_BLOCK))
                {
                    var boolExp = node[MyNonterminals.BOOL_EXPRESSION];
                    var boolOp = boolExp[MyNonterminals.BOOL_OPERATOR][TokenType.Operator].Value;

                    var bodyBeginLabel = node.Parent[MyNonterminals.FOR_BODY_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY];
                    var endForLabel = node.Parent.Value[END_BLOCK_LABEL_KEY];

                    _code.Emit(new Instruction(GetCondJumpInvertOpCode(boolOp), GetOperand(boolExp[0]), GetOperand(boolExp[2]), (Operand)endForLabel));
                    _code.Emit(new Instruction(OpCode.JMP, (Operand)bodyBeginLabel, null, null));
                }

                if (nonterminal.Equals(MyNonterminals.FOR_BODY_BLOCK))
                {
                    var stepBeginLabel = node.Parent[MyNonterminals.FOR_STEP_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY];

                    _code.Emit(new Instruction(OpCode.JMP, (Operand)stepBeginLabel, null, null));
                }

                if (nonterminal.Equals(MyNonterminals.FOR_STEP_BLOCK))
                {
                    var testBeginLabel = node.Parent[MyNonterminals.FOR_TEST_BLOCK].Value[BEGIN_BLOCK_LABEL_KEY];

                    _code.Emit(new Instruction(OpCode.JMP, (Operand)testBeginLabel, null, null));
                }
                #endregion
            }
        }
    }
}
