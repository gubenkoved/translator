using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Core
{
    public class MyNonterminals
    {
        public static Nonterminal EXPRESSION { get { return new Nonterminal("expression"); } }
        public static Nonterminal EXPRESSION_DASH { get { return new Nonterminal("expression_dash"); } }
        public static Nonterminal TERM { get { return new Nonterminal("term"); } }
        public static Nonterminal TERM_DASH { get { return new Nonterminal("term_dash"); } }
        public static Nonterminal FACTOR { get { return new Nonterminal("factor"); } }
        public static Nonterminal PARAM_LIST { get { return new Nonterminal("param_list"); } }
        public static Nonterminal PARAM_LIST_DASH { get { return new Nonterminal("param_list_dash"); } }
        public static Nonterminal PARAM { get { return new Nonterminal("param"); } }
        public static Nonterminal FUNCTION { get { return new Nonterminal("function"); } }
        public static Nonterminal FUNCTION_CALL { get { return new Nonterminal("function_call"); } }
        public static Nonterminal TYPE { get { return new Nonterminal("type"); } }
        public static Nonterminal ASSIGN_STATEMENT { get { return new Nonterminal("assign_statement"); } }
        public static Nonterminal STATEMENT { get { return new Nonterminal("statement"); } }
        public static Nonterminal STATEMENT_LIST { get { return new Nonterminal("statement_list"); } }
        public static Nonterminal BOOL_EXPRESSION { get { return new Nonterminal("bool_expression"); } }
        public static Nonterminal IF_STATEMENT { get { return new Nonterminal("if_statement"); } }
        public static Nonterminal IF_BLOCK { get { return new Nonterminal("if_block"); } }
        public static Nonterminal THEN_BLOCK { get { return new Nonterminal("then_block"); } }
        public static Nonterminal ELSE_BLOCK { get { return new Nonterminal("else_block"); } }
    }
}
