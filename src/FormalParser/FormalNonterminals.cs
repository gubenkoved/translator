using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;

namespace FormalParser
{
    public class FormalNonterminals
    {
        public static readonly Nonterminal EXPRESSION           = new Nonterminal("expression");
        public static readonly Nonterminal EXPRESSION_DASH      = new Nonterminal("expression_dash");
        public static readonly Nonterminal TERM                 = new Nonterminal("term");
        public static readonly Nonterminal TERM_DASH            = new Nonterminal("term_dash");
        public static readonly Nonterminal FACTOR               = new Nonterminal("factor");
        public static readonly Nonterminal PARAM_BLOCK          = new Nonterminal("param_block");
        public static readonly Nonterminal PARAM_LIST           = new Nonterminal("param_list");
        public static readonly Nonterminal PARAM_LIST_DASH      = new Nonterminal("param_list_dash");
        public static readonly Nonterminal PARAM                = new Nonterminal("param");
        public static readonly Nonterminal FUNCTION             = new Nonterminal("function");
        public static readonly Nonterminal FUNCTION_CALL        = new Nonterminal("function_call");
        public static readonly Nonterminal TYPE                 = new Nonterminal("type");
        public static readonly Nonterminal ASSIGN_STATEMENT     = new Nonterminal("assign_statement");
        public static readonly Nonterminal ID_DECLARATION       = new Nonterminal("id_declaration");
        public static readonly Nonterminal STATEMENT            = new Nonterminal("statement");
        public static readonly Nonterminal STATEMENTS_LIST      = new Nonterminal("statements_list");
        public static readonly Nonterminal STATEMENTS_BLOCK     = new Nonterminal("statements_block");
        public static readonly Nonterminal BOOL_EXPRESSION      = new Nonterminal("bool_expression");
        public static readonly Nonterminal BOOL_OPERATOR        = new Nonterminal("bool_operator");
        public static readonly Nonterminal IF_STATEMENT         = new Nonterminal("if_statement");
        public static readonly Nonterminal IF_BLOCK             = new Nonterminal("if_block");
        public static readonly Nonterminal THEN_BLOCK           = new Nonterminal("then_block");
        public static readonly Nonterminal ELSE_BLOCK           = new Nonterminal("else_block");
        public static readonly Nonterminal FOR_STATEMENT        = new Nonterminal("for_statement");
        public static readonly Nonterminal FOR_INIT_BLOCK       = new Nonterminal("for_init_block");
        public static readonly Nonterminal FOR_TEST_BLOCK       = new Nonterminal("for_test_block");
        public static readonly Nonterminal FOR_STEP_BLOCK       = new Nonterminal("for_step_block");
        public static readonly Nonterminal FOR_BODY_BLOCK       = new Nonterminal("for_body_block");
    }
}
