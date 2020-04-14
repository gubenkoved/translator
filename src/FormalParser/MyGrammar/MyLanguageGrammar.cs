using System.Collections.Generic;
using Lexer.Core;
using Parser.Core;

namespace FormalParser.MyGrammar
{
    public static class MyLanguageGrammar
    {
        public static List<Production> Productions;

        static MyLanguageGrammar()
        {
            Productions = new List<Production>();

            // E -> T E'
            Productions.Add(new Production(MyNonterminals.EXPRESSION, 
                new SymbolsChain(
                    MyNonterminals.TERM, 
                    MyNonterminals.EXPRESSION_DASH)));
            // E' -> + T E'
            Productions.Add(new Production(MyNonterminals.EXPRESSION_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("+", TokenType.Operator)), 
                    MyNonterminals.TERM, 
                    MyNonterminals.EXPRESSION_DASH)));
            // E' -> - T E'
            Productions.Add(new Production(MyNonterminals.EXPRESSION_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("-", TokenType.Operator)), 
                    MyNonterminals.TERM, 
                    MyNonterminals.EXPRESSION_DASH)));
            // E' -> epsilon
            Productions.Add(new Production(MyNonterminals.EXPRESSION_DASH, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));
            
            // T -> F T'
            Productions.Add(new Production(MyNonterminals.TERM, 
                new SymbolsChain(
                    MyNonterminals.FACTOR, 
                    MyNonterminals.TERM_DASH)));
            // T' -> * F T'
            Productions.Add(new Production(MyNonterminals.TERM_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("*", TokenType.Operator)), 
                    MyNonterminals.FACTOR, 
                    MyNonterminals.TERM_DASH)));
            // T' -> / F T'
            Productions.Add(new Production(MyNonterminals.TERM_DASH, 
                new SymbolsChain(new ConcreteTerminal(new Token("/", TokenType.Operator)), 
                    MyNonterminals.FACTOR, 
                    MyNonterminals.TERM_DASH)));
            // T' -> epsilon
            Productions.Add(new Production(MyNonterminals.TERM_DASH, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // F -> floatConstant
            Productions.Add(new Production(MyNonterminals.FACTOR, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.FloatConstant))));
            // F -> integerConstant
            Productions.Add(new Production(MyNonterminals.FACTOR, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.IntegerConstant))));
            // F -> id
            Productions.Add(new Production(MyNonterminals.FACTOR, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.Identifier))));
            // F -> ( E )
            Productions.Add(new Production(MyNonterminals.FACTOR, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("(", TokenType.Operator)), 
                    MyNonterminals.EXPRESSION, 
                    new ConcreteTerminal(new Token(")", TokenType.Operator)))));
            // F -> function-call
            Productions.Add(new Production(MyNonterminals.FACTOR, 
                new SymbolsChain(
                    MyNonterminals.FUNCTION_CALL)));

            // function-call -> function param-block
            Productions.Add(new Production(MyNonterminals.FUNCTION_CALL, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.Function),                     
                    MyNonterminals.PARAM_BLOCK
                    )));

            // param-block -> ( param-list )
            Productions.Add(new Production(MyNonterminals.PARAM_BLOCK,
                new SymbolsChain(                    
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    MyNonterminals.PARAM_LIST,
                    new ConcreteTerminal(new Token(")", TokenType.Operator)))));

            // param-list -> param param-list'
            Productions.Add(new Production(MyNonterminals.PARAM_LIST, 
                new SymbolsChain(
                    MyNonterminals.PARAM, 
                    MyNonterminals.PARAM_LIST_DASH)));
            // param-list -> epsilon
            Productions.Add(new Production(MyNonterminals.PARAM_LIST, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // param-list' -> , param param-list'
            Productions.Add(new Production(MyNonterminals.PARAM_LIST_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token(",", TokenType.Operator)), 
                    MyNonterminals.PARAM, 
                    MyNonterminals.PARAM_LIST_DASH)));
            // param-list' -> epsilon
            Productions.Add(new Production(MyNonterminals.PARAM_LIST_DASH, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // param -> E
            Productions.Add(new Production(MyNonterminals.PARAM, 
                new SymbolsChain(
                    MyNonterminals.EXPRESSION)));

            // id-declaration -> type id
            Productions.Add(new Production(MyNonterminals.ID_DECLARATION,
                new SymbolsChain(
                    MyNonterminals.TYPE,
                    new GeneralizedTerminal(TokenType.Identifier))));

            // assign-statement -> id-declaration = E
            Productions.Add(new Production(MyNonterminals.ASSIGN_STATEMENT, 
                new SymbolsChain(
                    MyNonterminals.ID_DECLARATION, 
                    new ConcreteTerminal(new Token("=", TokenType.Operator)), 
                    MyNonterminals.EXPRESSION)));
            // assign-statement -> id = E
            Productions.Add(new Production(MyNonterminals.ASSIGN_STATEMENT, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.Identifier), 
                    new ConcreteTerminal(new Token("=", TokenType.Operator)), 
                    MyNonterminals.EXPRESSION)));

            // type -> int
            Productions.Add(new Production(MyNonterminals.TYPE, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("int", TokenType.Keyword)))));
            // type -> float
            Productions.Add(new Production(MyNonterminals.TYPE, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("float", TokenType.Keyword)))));
            // type -> void
            Productions.Add(new Production(MyNonterminals.TYPE, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("void", TokenType.Keyword)))));

            // if-statement -> if-block then-block else-block
            Productions.Add(new Production(MyNonterminals.IF_STATEMENT,
                new SymbolsChain(
                    MyNonterminals.IF_BLOCK,
                    MyNonterminals.THEN_BLOCK,
                    MyNonterminals.ELSE_BLOCK)));

            // if-block -> if ( bool-expression )
            Productions.Add(new Production(MyNonterminals.IF_BLOCK,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("if", TokenType.Keyword)),
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    MyNonterminals.BOOL_EXPRESSION,
                    new ConcreteTerminal(new Token(")", TokenType.Operator)))));

            // then-block -> statements-block
            Productions.Add(new Production(MyNonterminals.THEN_BLOCK,
                new SymbolsChain(
                    MyNonterminals.STATEMENTS_BLOCK)));

            // else-block -> else statements-block
            Productions.Add(new Production(MyNonterminals.ELSE_BLOCK,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("else", TokenType.Keyword)),
                    MyNonterminals.STATEMENTS_BLOCK)));
            // else-block -> epsilon
            Productions.Add(new Production(MyNonterminals.ELSE_BLOCK,
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // bool-expression -> E bool-operator E
            Productions.Add(new Production(MyNonterminals.BOOL_EXPRESSION,
                new SymbolsChain(
                    MyNonterminals.EXPRESSION,
                    MyNonterminals.BOOL_OPERATOR,
                    MyNonterminals.EXPRESSION)));

            // bool-operator -> ==
            Productions.Add(new Production(MyNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("==", TokenType.Operator))
                    )));
            // bool-operator -> !=
            Productions.Add(new Production(MyNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("!=", TokenType.Operator)))));
            // bool-operator -> >=
            Productions.Add(new Production(MyNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token(">=", TokenType.Operator)))));
            // bool-operator -> <=
            Productions.Add(new Production(MyNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("<=", TokenType.Operator)))));
            // bool-operator -> >
            Productions.Add(new Production(MyNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token(">", TokenType.Operator)))));
            // bool-operator -> <
            Productions.Add(new Production(MyNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("<", TokenType.Operator)))));

            // for-init-block -> assign-statement
            Productions.Add(new Production(MyNonterminals.FOR_INIT_BLOCK,
                new SymbolsChain(
                    MyNonterminals.ASSIGN_STATEMENT)));

            // for-test-block -> bool-expression
            Productions.Add(new Production(MyNonterminals.FOR_TEST_BLOCK,
                new SymbolsChain(
                    MyNonterminals.BOOL_EXPRESSION)));

            // for-step-block -> assign-statement
            Productions.Add(new Production(MyNonterminals.FOR_STEP_BLOCK,
                new SymbolsChain(
                    MyNonterminals.ASSIGN_STATEMENT)));

            // for-body-block -> statements-block
            Productions.Add(new Production(MyNonterminals.FOR_BODY_BLOCK,
                new SymbolsChain(
                    MyNonterminals.STATEMENTS_BLOCK)));

            // for-statement -> for ( for-init-block ; for-test-block ; for-step-block ) for-body-block
            Productions.Add(new Production(MyNonterminals.FOR_STATEMENT,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("for", TokenType.Keyword)),
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    MyNonterminals.FOR_INIT_BLOCK,
                    new ConcreteTerminal(new Token(";", TokenType.Operator)),
                    MyNonterminals.FOR_TEST_BLOCK,
                    new ConcreteTerminal(new Token(";", TokenType.Operator)),
                    MyNonterminals.FOR_STEP_BLOCK,
                    new ConcreteTerminal(new Token(")", TokenType.Operator)),
                    MyNonterminals.FOR_BODY_BLOCK)));

            // statement -> assign-statement
            Productions.Add(new Production(MyNonterminals.STATEMENT, 
                new SymbolsChain(
                    MyNonterminals.ASSIGN_STATEMENT)));
            // statement -> function-call
            Productions.Add(new Production(MyNonterminals.STATEMENT,
                new SymbolsChain(
                    MyNonterminals.FUNCTION_CALL)));
            // statement -> if-statement
            Productions.Add(new Production(MyNonterminals.STATEMENT,
                new SymbolsChain(
                    MyNonterminals.IF_STATEMENT)));
            // statement -> for-statement
            Productions.Add(new Production(MyNonterminals.STATEMENT,
                new SymbolsChain(
                    MyNonterminals.FOR_STATEMENT)));

            // statements-list -> statement ; statement-list
            Productions.Add(new Production(MyNonterminals.STATEMENTS_LIST, 
                new SymbolsChain(
                    MyNonterminals.STATEMENT, 
                    new ConcreteTerminal(new Token(";", TokenType.Operator)), 
                    MyNonterminals.STATEMENTS_LIST)));
            // statements-list -> epsilon
            Productions.Add(new Production(MyNonterminals.STATEMENTS_LIST, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // statements-block -> { statements-list }
            Productions.Add(new Production(MyNonterminals.STATEMENTS_BLOCK,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("{", TokenType.Operator)), 
                    MyNonterminals.STATEMENTS_LIST, 
                    new ConcreteTerminal(new Token("}", TokenType.Operator)))));

            // function -> type id ( ) statements-block
            Productions.Add(new Production(MyNonterminals.FUNCTION, 
                new SymbolsChain(
                    MyNonterminals.TYPE,
                    new GeneralizedTerminal(TokenType.Identifier),
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    new ConcreteTerminal(new Token(")", TokenType.Operator)),
                    MyNonterminals.STATEMENTS_BLOCK)));
        }
    }
}
