using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;

using Lexer.Core;

namespace RecursiveDescentParser
{
    /// <summary>
    /// ATTENTION: CRAPOCODE
    /// </summary>
    public class MyRecursiveDescentParser : ParserBase
    {
        private class ParseFunctionResult
        {
            public Error Error { get; private set; }
            public bool HasError
            {
                get
                {
                    return Error != null;
                }
            }

            public SyntaxTreeNode Node { get; private set; }

            public ParseFunctionResult(SyntaxTreeNode node, Error error = null)
            {
                Node = node;
                Error = error;
            }

            public ParseFunctionResult(SyntaxTreeNode node, Token token, string error)
                : this(node, new Error(token, ErrorKind.Syntax, error))
            {
            }
        }

        private ParseFunctionResult Expression(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.EXPRESSION);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!Resolve(ts, node, Term, out result)) return result;

            Resolve(ts, node, ExpressionDash, out result);

            return result;
        }

        private ParseFunctionResult ExpressionDash(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.EXPRESSION_DASH);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (ts.Current != null)
            {
                switch (ts.Current.Value)
                {
                    case "+":
                        node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

                        if (!Resolve(ts, node, Term, out result)) return result;

                        if (!Resolve(ts, node, ExpressionDash, out result)) return result;
                        break;
                    case "-": goto case "+";
                    default: break;
                }
            }

            return result;
        }

        private ParseFunctionResult Term(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.TERM);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!Resolve(ts, node, Factor, out result)) return result;

            if (!Resolve(ts, node, TermDash, out result)) return result;

            return result;
        }

        private ParseFunctionResult TermDash(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.TERM_DASH);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (ts.Current != null)
            {
                switch (ts.Current.Value)
                {
                    case "*":
                        node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

                        if (!Resolve(ts, node, Factor, out result)) return result;

                        if (!Resolve(ts, node, TermDash, out result)) return result;
                        break;
                    case "/": goto case "*";
                    default: break;
                }
            }

            return result;
        }

        private ParseFunctionResult Factor(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.FACTOR);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!CheckNotEndOfTokens(ts, node, out result)) return result;

            if (ts.Current.Value == "(")
            {
                node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

                if (!Resolve(ts, node, Expression, out result)) return result;

                ExpectValue(ts, node, ")", out result);
            }
            else if (ts.Current.HasOneOfTypes(TokenType.IntegerConstant, TokenType.FloatConstant, TokenType.Identifier))
            {
                node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));
            }
            else if (ts.Current.HasType(TokenType.Function))
            {
                Resolve(ts, node, FunctionCall, out result);
            }
            else
            {
                return new ParseFunctionResult(node, ts.Current, "Factor expected");
            }

            return result;
        }

        private ParseFunctionResult FunctionCall(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.FUNCTION_CALL);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!ExpectTokenType(ts, node, TokenType.Function, out result)) return result;

            if (!ExpectValue(ts, node, "(", out result)) return result;

            if (!Resolve(ts, node, ParamList, out result)) return result;

            ExpectValue(ts, node, ")", out result);

            return result;
        }

        private ParseFunctionResult ParamList(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.PARAM_LIST);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!CheckNotEndOfTokens(ts, node, out result)) return result;

            if (ts.Current.Value != ")")
            {                
                var param = Param(ts);
                if (param.HasError)
                    return new ParseFunctionResult(node, param.Error);
                if (!param.Node.HasNoChildren)
                {
                    node.AddChild(param.Node);

                    Resolve(ts, node, ParamListDash, out result);
                }
            }
            
            return result;
        }

        private ParseFunctionResult ParamListDash(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.PARAM_LIST_DASH);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!CheckNotEndOfTokens(ts, node, out result)) return result;

            if (ts.Current.Value == ",")
            {
                node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

                if (!Resolve(ts, node, Param, out result)) return result;

                if (!Resolve(ts, node, ParamListDash, out result)) return result;
            }

            return result;
        }

        private ParseFunctionResult Param(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.PARAM);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!CheckNotEndOfTokens(ts, node, out result)) return result;

            if (ts.Current.Value != ")")
            {            
                Resolve(ts, node, Expression, out result);
            }

            return result;
        }

        private ParseFunctionResult Type(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.TYPE);
            ParseFunctionResult result = new ParseFunctionResult(node);
            
            ExpectValues(ts, node, new[] { "int", "float", "void", "string", "char" }, out result);

            return result;
        }

        private ParseFunctionResult Function(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.FUNCTION);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!Resolve(ts, node, Type, out result)) return result;

            if (!ExpectTokenType(ts, node, TokenType.Identifier, out result)) return result;

            if (!ExpectValue(ts, node, "(", out result)) return result;

            //node.AddChild(ParamList(ts));

            if (!ExpectValue(ts, node, ")", out result)) return result;

            if (!ExpectValue(ts, node, "{", out result)) return result;

            if (!Resolve(ts, node, StatementList, out result)) return result;

            if (!ExpectValue(ts, node, "}", out result)) return result;
            
            if (!ExpectTokenType(ts, node, TokenType.EndOfText, out result)) return result;

            return result;
        }

        private ParseFunctionResult AssignStatement(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.ASSIGN_STATEMENT);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!Try(ts, Type).HasError)
                Resolve(ts, node, Type, out result);

            if (!ExpectTokenType(ts, node, TokenType.Identifier, out result)) return result;

            if (!ExpectValue(ts, node, "=", out result)) return result;

            if (!Resolve(ts, node, Expression, out result)) return result;

            return result;
        }

        private ParseFunctionResult Statement(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.STATEMENT);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!CheckNotEndOfTokens(ts, node, out result)) return result;

            if (ts.Current.HasType(TokenType.Function))
            {
                Resolve(ts, node, FunctionCall, out result);
            }
            else if (ts.Current.HasType(TokenType.Identifier) || !Try(ts, Type).HasError)
            {
                Resolve(ts, node, AssignStatement, out result);
            }
            else if (ts.Current.Value == "if")
            {
                Resolve(ts, node, IfStatement, out result);
            }
            else
            {
                return new ParseFunctionResult(node, ts.Current, "Statement expected");
            }

            return result;
        }

        private ParseFunctionResult StatementList(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.STATEMENT_LIST);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!CheckNotEndOfTokens(ts, node, out result)) return result;

            if (ts.Current.Value != "}")
            {
                if (!Resolve(ts, node, Statement, out result)) return result;

                if (!ExpectValue(ts, node, ";", out result)) return result;

                if (!Resolve(ts, node, StatementList, out result)) return result;
            }

            return result;
        }

        private ParseFunctionResult BoolExpression(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.BOOL_EXPRESSION);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!Resolve(ts, node, Expression, out result)) return result;

            if (!ExpectValues(ts, node, new[] { "==", "!=", ">", "<", ">=", "<=" }, out result)) return result;

            if (!Resolve(ts, node, Expression, out result)) return result;

            return result;
        }

        private ParseFunctionResult IfStatement(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.IF_STATEMENT);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!Resolve(ts, node, IfBlock, out result)) return result;
            
            if (!Resolve(ts, node, ThenBlock, out result)) return result;

            Resolve(ts, node, ElseBlock, out result);

            return result;
        }

        private ParseFunctionResult IfBlock(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.IF_BLOCK);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!ExpectValue(ts, node, "if", out result)) return result;

            if (!ExpectValue(ts, node, "(", out result)) return result;

            if (!Resolve(ts, node, BoolExpression, out result)) return result;

            ExpectValue(ts, node, ")", out result);

            return result;
        }

        private ParseFunctionResult ThenBlock(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.THEN_BLOCK);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (!ExpectValue(ts, node, "{", out result)) return result;

            if (!Resolve(ts, node, StatementList, out result)) return result;

            if (!ExpectValue(ts, node, "}", out result)) return result;

            return result;
        }

        private ParseFunctionResult ElseBlock(TokenStream ts)
        {
            var node = new SyntaxTreeNode(MyNonterminals.ELSE_BLOCK);
            ParseFunctionResult result = new ParseFunctionResult(node);

            if (ts.Current != null && ts.Current.Value == "else")
            {
                if (!ExpectValue(ts, node, "else", out result)) return result;

                if (!ExpectValue(ts, node, "{", out result)) return result;

                if (!Resolve(ts, node, StatementList, out result)) return result;

                if (!ExpectValue(ts, node, "}", out result)) return result;
            }

            return result;
        }

        private bool Resolve(TokenStream ts, SyntaxTreeNode node, Func<TokenStream, ParseFunctionResult> resolutionFunction, out ParseFunctionResult result)
        {
            var resolved = resolutionFunction(ts);
            if (resolved.HasError)
            {
                result = new ParseFunctionResult(node, resolved.Error);
                return false;
            }

            resolved.Node.Parent = node;
            
            node.AddChild(resolved.Node);

            result = new ParseFunctionResult(node);            
            
            return true;
        }

        private bool CheckNotEndOfTokens(TokenStream ts, SyntaxTreeNode node, out ParseFunctionResult result)
        {
            if (ts.Current == null)
            {
                result = new ParseFunctionResult(node, ts.Last, "Unexpected end");
                return false;
            }

            result = new ParseFunctionResult(node); 
       
            return true;
        }

        private bool ExpectValue(TokenStream ts, SyntaxTreeNode node, string value, out ParseFunctionResult result)
        {
            if (!CheckNotEndOfTokens(ts, node, out result)) return false;

            if (ts.Current.Value != value)
            {
                result = new ParseFunctionResult(node, ts.Current, string.Format("{0} expected", value));
                return false;
            }

            result = new ParseFunctionResult(node);

            node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

            return true;
        }

        private bool ExpectTokenType(TokenStream ts, SyntaxTreeNode node, TokenType tokenType, out ParseFunctionResult result)
        {
            if (!CheckNotEndOfTokens(ts, node, out result)) return false;
            
            if (!ts.Current.HasType(tokenType))
            {
                result = new ParseFunctionResult(node, ts.Current, string.Format("{0} expected", tokenType));
                return false;
            }

            result = new ParseFunctionResult(node);

            node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

            return true;
        }

        private bool ExpectValues(TokenStream ts, SyntaxTreeNode node, IEnumerable<string> values, out ParseFunctionResult result)
        {
            if (!CheckNotEndOfTokens(ts, node, out result)) return false;

            if (values.All(val => val != ts.Current.Value))
            {
                result = new ParseFunctionResult(node, ts.Current, string.Format("{0} expected", string.Join(" or ", values)));
                return false;
            }

            result = new ParseFunctionResult(node);

            node.AddChild(new SyntaxTreeNode(new ConcreteTerminal(ts.Pop())));

            return true;
        }

        private ParseFunctionResult Try(TokenStream ts, Func<TokenStream, ParseFunctionResult> parseFunction)
        {
            var state = ts.GetState();

            var result = parseFunction(ts);

            ts.SetState(state);

            return result;
        }

        protected override SyntaxTree ParseImplementation(TokenStream ts, out IEnumerable<Error> syntaxErrors)
        {
            var root = Function(ts);

            var errors = new List<Error>();
            syntaxErrors = errors;

            if (root.HasError)
            {
                errors.Add(root.Error);
                return null;
            }

            return new SyntaxTree(root.Node);
        }
    }
}