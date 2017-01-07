using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;
using Lexer.Core;

namespace FormalParser
{
    public static class MyLanguageGrammar
    {
        public static List<Production> ProcessedProductions;

        static MyLanguageGrammar()
        {
            ProcessedProductions = new List<Production>();

            // E -> T E'
            ProcessedProductions.Add(new Production(FormalNonterminals.EXPRESSION, 
                new SymbolsChain(
                    FormalNonterminals.TERM, 
                    FormalNonterminals.EXPRESSION_DASH)));
            // E' -> + T E'
            ProcessedProductions.Add(new Production(FormalNonterminals.EXPRESSION_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("+", TokenType.Operator)), 
                    FormalNonterminals.TERM, 
                    FormalNonterminals.EXPRESSION_DASH)));
            // E' -> - T E'
            ProcessedProductions.Add(new Production(FormalNonterminals.EXPRESSION_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("-", TokenType.Operator)), 
                    FormalNonterminals.TERM, 
                    FormalNonterminals.EXPRESSION_DASH)));
            // E' -> epsilon
            ProcessedProductions.Add(new Production(FormalNonterminals.EXPRESSION_DASH, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));
            
            // T -> F T'
            ProcessedProductions.Add(new Production(FormalNonterminals.TERM, 
                new SymbolsChain(
                    FormalNonterminals.FACTOR, 
                    FormalNonterminals.TERM_DASH)));
            // T' -> * F T'
            ProcessedProductions.Add(new Production(FormalNonterminals.TERM_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("*", TokenType.Operator)), 
                    FormalNonterminals.FACTOR, 
                    FormalNonterminals.TERM_DASH)));
            // T' -> / F T'
            ProcessedProductions.Add(new Production(FormalNonterminals.TERM_DASH, 
                new SymbolsChain(new ConcreteTerminal(new Token("/", TokenType.Operator)), 
                    FormalNonterminals.FACTOR, 
                    FormalNonterminals.TERM_DASH)));
            // T' -> epsilon
            ProcessedProductions.Add(new Production(FormalNonterminals.TERM_DASH, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // F -> floatConstant
            ProcessedProductions.Add(new Production(FormalNonterminals.FACTOR, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.FloatConstant))));
            // F -> integerConstant
            ProcessedProductions.Add(new Production(FormalNonterminals.FACTOR, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.IntegerConstant))));
            // F -> id
            ProcessedProductions.Add(new Production(FormalNonterminals.FACTOR, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.Identifier))));
            // F -> ( E )
            ProcessedProductions.Add(new Production(FormalNonterminals.FACTOR, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("(", TokenType.Operator)), 
                    FormalNonterminals.EXPRESSION, 
                    new ConcreteTerminal(new Token(")", TokenType.Operator)))));
            // F -> function-call
            ProcessedProductions.Add(new Production(FormalNonterminals.FACTOR, 
                new SymbolsChain(
                    FormalNonterminals.FUNCTION_CALL)));

            // function-call -> function param-block
            ProcessedProductions.Add(new Production(FormalNonterminals.FUNCTION_CALL, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.Function),                     
                    FormalNonterminals.PARAM_BLOCK
                    )));

            // param-block -> ( param-list )
            ProcessedProductions.Add(new Production(FormalNonterminals.PARAM_BLOCK,
                new SymbolsChain(                    
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    FormalNonterminals.PARAM_LIST,
                    new ConcreteTerminal(new Token(")", TokenType.Operator)))));

            // param-list -> param param-list'
            ProcessedProductions.Add(new Production(FormalNonterminals.PARAM_LIST, 
                new SymbolsChain(
                    FormalNonterminals.PARAM, 
                    FormalNonterminals.PARAM_LIST_DASH)));
            // param-list -> epsilon
            ProcessedProductions.Add(new Production(FormalNonterminals.PARAM_LIST, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // param-list' -> , param param-list'
            ProcessedProductions.Add(new Production(FormalNonterminals.PARAM_LIST_DASH, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token(",", TokenType.Operator)), 
                    FormalNonterminals.PARAM, 
                    FormalNonterminals.PARAM_LIST_DASH)));
            // param-list' -> epsilon
            ProcessedProductions.Add(new Production(FormalNonterminals.PARAM_LIST_DASH, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // param -> E
            ProcessedProductions.Add(new Production(FormalNonterminals.PARAM, 
                new SymbolsChain(
                    FormalNonterminals.EXPRESSION)));

            // id-declaration -> type id
            ProcessedProductions.Add(new Production(FormalNonterminals.ID_DECLARATION,
                new SymbolsChain(
                    FormalNonterminals.TYPE,
                    new GeneralizedTerminal(TokenType.Identifier))));

            // assign-statement -> id-declaration = E
            ProcessedProductions.Add(new Production(FormalNonterminals.ASSIGN_STATEMENT, 
                new SymbolsChain(
                    FormalNonterminals.ID_DECLARATION, 
                    new ConcreteTerminal(new Token("=", TokenType.Operator)), 
                    FormalNonterminals.EXPRESSION)));
            // assign-statement -> id = E
            ProcessedProductions.Add(new Production(FormalNonterminals.ASSIGN_STATEMENT, 
                new SymbolsChain(
                    new GeneralizedTerminal(TokenType.Identifier), 
                    new ConcreteTerminal(new Token("=", TokenType.Operator)), 
                    FormalNonterminals.EXPRESSION)));

            // type -> int
            ProcessedProductions.Add(new Production(FormalNonterminals.TYPE, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("int", TokenType.Keyword)))));
            // type -> float
            ProcessedProductions.Add(new Production(FormalNonterminals.TYPE, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("float", TokenType.Keyword)))));
            // type -> void
            ProcessedProductions.Add(new Production(FormalNonterminals.TYPE, 
                new SymbolsChain(
                    new ConcreteTerminal(new Token("void", TokenType.Keyword)))));

            // if-statement -> if-block then-block else-block
            ProcessedProductions.Add(new Production(FormalNonterminals.IF_STATEMENT,
                new SymbolsChain(
                    FormalNonterminals.IF_BLOCK,
                    FormalNonterminals.THEN_BLOCK,
                    FormalNonterminals.ELSE_BLOCK)));

            // if-block -> if ( bool-expression )
            ProcessedProductions.Add(new Production(FormalNonterminals.IF_BLOCK,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("if", TokenType.Keyword)),
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    FormalNonterminals.BOOL_EXPRESSION,
                    new ConcreteTerminal(new Token(")", TokenType.Operator)))));

            // then-block -> statements-block
            ProcessedProductions.Add(new Production(FormalNonterminals.THEN_BLOCK,
                new SymbolsChain(
                    FormalNonterminals.STATEMENTS_BLOCK)));

            // else-block -> else statements-block
            ProcessedProductions.Add(new Production(FormalNonterminals.ELSE_BLOCK,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("else", TokenType.Keyword)),
                    FormalNonterminals.STATEMENTS_BLOCK)));
            // else-block -> epsilon
            ProcessedProductions.Add(new Production(FormalNonterminals.ELSE_BLOCK,
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // bool-expression -> E bool-operator E
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_EXPRESSION,
                new SymbolsChain(
                    FormalNonterminals.EXPRESSION,
                    FormalNonterminals.BOOL_OPERATOR,
                    FormalNonterminals.EXPRESSION)));

            // bool-operator -> ==
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("==", TokenType.Operator))
                    )));
            // bool-operator -> !=
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("!=", TokenType.Operator)))));
            // bool-operator -> >=
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token(">=", TokenType.Operator)))));
            // bool-operator -> <=
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("<=", TokenType.Operator)))));
            // bool-operator -> >
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token(">", TokenType.Operator)))));
            // bool-operator -> <
            ProcessedProductions.Add(new Production(FormalNonterminals.BOOL_OPERATOR,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("<", TokenType.Operator)))));

            // for-init-block -> assign-statement
            ProcessedProductions.Add(new Production(FormalNonterminals.FOR_INIT_BLOCK,
                new SymbolsChain(
                    FormalNonterminals.ASSIGN_STATEMENT)));

            // for-test-block -> bool-expression
            ProcessedProductions.Add(new Production(FormalNonterminals.FOR_TEST_BLOCK,
                new SymbolsChain(
                    FormalNonterminals.BOOL_EXPRESSION)));

            // for-step-block -> assign-statement
            ProcessedProductions.Add(new Production(FormalNonterminals.FOR_STEP_BLOCK,
                new SymbolsChain(
                    FormalNonterminals.ASSIGN_STATEMENT)));

            // for-body-block -> statements-block
            ProcessedProductions.Add(new Production(FormalNonterminals.FOR_BODY_BLOCK,
                new SymbolsChain(
                    FormalNonterminals.STATEMENTS_BLOCK)));

            // for-statement -> for ( for-init-block ; for-test-block ; for-step-block ) for-body-block
            ProcessedProductions.Add(new Production(FormalNonterminals.FOR_STATEMENT,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("for", TokenType.Keyword)),
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    FormalNonterminals.FOR_INIT_BLOCK,
                    new ConcreteTerminal(new Token(";", TokenType.Operator)),
                    FormalNonterminals.FOR_TEST_BLOCK,
                    new ConcreteTerminal(new Token(";", TokenType.Operator)),
                    FormalNonterminals.FOR_STEP_BLOCK,
                    new ConcreteTerminal(new Token(")", TokenType.Operator)),
                    FormalNonterminals.FOR_BODY_BLOCK)));

            // statement -> assign-statement
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENT, 
                new SymbolsChain(
                    FormalNonterminals.ASSIGN_STATEMENT)));
            // statement -> function-call
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENT,
                new SymbolsChain(
                    FormalNonterminals.FUNCTION_CALL)));
            // statement -> if-statement
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENT,
                new SymbolsChain(
                    FormalNonterminals.IF_STATEMENT)));
            // statement -> for-statement
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENT,
                new SymbolsChain(
                    FormalNonterminals.FOR_STATEMENT)));

            // statements-list -> statement ; statement-list
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENTS_LIST, 
                new SymbolsChain(
                    FormalNonterminals.STATEMENT, 
                    new ConcreteTerminal(new Token(";", TokenType.Operator)), 
                    FormalNonterminals.STATEMENTS_LIST)));
            // statements-list -> epsilon
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENTS_LIST, 
                new SymbolsChain(
                    GeneralizedTerminal.Epsilon)));

            // statements-block -> { statements-list }
            ProcessedProductions.Add(new Production(FormalNonterminals.STATEMENTS_BLOCK,
                new SymbolsChain(
                    new ConcreteTerminal(new Token("{", TokenType.Operator)), 
                    FormalNonterminals.STATEMENTS_LIST, 
                    new ConcreteTerminal(new Token("}", TokenType.Operator)))));

            // function -> type id ( ) statements-block
            ProcessedProductions.Add(new Production(FormalNonterminals.FUNCTION, 
                new SymbolsChain(
                    FormalNonterminals.TYPE,
                    new GeneralizedTerminal(TokenType.Identifier),
                    new ConcreteTerminal(new Token("(", TokenType.Operator)),
                    new ConcreteTerminal(new Token(")", TokenType.Operator)),
                    FormalNonterminals.STATEMENTS_BLOCK)));
        }
    }
}
