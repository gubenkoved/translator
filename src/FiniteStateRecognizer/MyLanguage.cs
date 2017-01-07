using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

namespace FiniteStateRecognizer
{
    public static class MyLanguage
    {
        public const int MAX_IDENTIFIER_LEN = 14;

        public static TokenRecognizer IdentifiersRecognizer;
        public static TokenRecognizer DelimetersRecognizer;
        public static TokenRecognizer StringsRecognizer;
        public static TokenRecognizer CharsRecognizer;
        public static TokenRecognizer IntegersRecognizer;
        public static TokenRecognizer FloatsRecognizer;
        public static TokenRecognizer CommentsRecognizer;
        public static TokenRecognizer MultiLineCommentsRecognizer;
        public static IEnumerable<TokenRecognizer> OperatorRecognizers;

        // At first keyword and functions will be recognized as identifiers, but
        // during post-recognition process they will be replaced (using following lists)
        public static IEnumerable<string> Keywords;
        public static IEnumerable<string> Functions;

        static MyLanguage()
        {
            #region IdentifiersRecognizer [digit | letter| _]{1,}
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Finite);

                // set low priority to avoid conflicts with integer constants
                IdentifiersRecognizer = new TokenRecognizer(TokenType.Identifier, s1, -1);

                IdentifiersRecognizer.AddTransition(new Transition(s1, s2, CharTests.IsLetter.Or(CharTests.IsDigit).Or('_')));
                IdentifiersRecognizer.AddTransition(new Transition(s2, s2, CharTests.IsLetter.Or(CharTests.IsDigit).Or('_')));
            }
            #endregion

            #region DelimetersRecognizer [delim]{1,}
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Finite);

                DelimetersRecognizer = new TokenRecognizer(TokenType.Delimiter, s1);

                DelimetersRecognizer.AddTransition(new Transition(s1, s2, CharTests.IsDelimeter));
                DelimetersRecognizer.AddTransition(new Transition(s2, s2, CharTests.IsDelimeter));
            }
            #endregion

            #region StringsRecognizer
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Finite); // accept unclosed literals
                State s3 = new State(3, StateType.Finite);

                StringsRecognizer = new TokenRecognizer(TokenType.StringConstant, s1);

                StringsRecognizer.AddTransition(new Transition(s1, s2, CharTests.IsStringMarker));
                StringsRecognizer.AddTransition(new Transition(s2, s2, CharTests.Not(CharTests.IsStringMarker.Or('\r'))));
                StringsRecognizer.AddTransition(new Transition(s2, s3, CharTests.IsStringMarker));
            }
            #endregion

            #region CharsRecognizer
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Finite); // accept unclosed char literals
                State s3 = new State(3, StateType.Finite);

                CharsRecognizer = new TokenRecognizer(TokenType.CharConstant, s1);

                CharsRecognizer.AddTransition(new Transition(s1, s2, CharTests.IsCharMarker));
                CharsRecognizer.AddTransition(new Transition(s2, s2, CharTests.Not(CharTests.IsCharMarker.Or('\r'))));
                CharsRecognizer.AddTransition(new Transition(s2, s3, CharTests.IsCharMarker));
            }
            #endregion

            #region IntegersRecognizer
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Finite);

                IntegersRecognizer = new TokenRecognizer(TokenType.IntegerConstant, s1);

                IntegersRecognizer.AddTransition(new Transition(s1, s2, CharTests.IsDigit));
                IntegersRecognizer.AddTransition(new Transition(s2, s2, CharTests.IsDigit));
            }
            #endregion

            #region FloatsRecognizer
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Nonfinite);
                State s3 = new State(3, StateType.Nonfinite);
                State s4 = new State(4, StateType.Finite);

                FloatsRecognizer = new TokenRecognizer(TokenType.FloatConstant, s1);

                FloatsRecognizer.AddTransition(new Transition(s1, s2, CharTests.IsDigit));
                FloatsRecognizer.AddTransition(new Transition(s2, s2, CharTests.IsDigit));
                FloatsRecognizer.AddTransition(new Transition(s2, s3, CharTests.IsFloatNumberDelimiter));
                FloatsRecognizer.AddTransition(new Transition(s3, s4, CharTests.IsDigit));
                FloatsRecognizer.AddTransition(new Transition(s4, s4, CharTests.IsDigit));
            }
            #endregion

            #region CommentsRecognizer
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Nonfinite);
                State s3 = new State(3, StateType.Finite);

                CommentsRecognizer = new TokenRecognizer(TokenType.Commentary, s1);

                CommentsRecognizer.AddTransition(new Transition(s1, s2, CharTests.Is('/')));
                CommentsRecognizer.AddTransition(new Transition(s2, s3, CharTests.Is('/')));
                CommentsRecognizer.AddTransition(new Transition(s3, s3, CharTests.Is('\n').Or('\r').Inverse()));
            }
            #endregion

            #region MultiLineCommentsRecognizer
            {
                State s1 = new State(1, StateType.Nonfinite);
                State s2 = new State(2, StateType.Nonfinite);
                State s3 = new State(3, StateType.Nonfinite);
                State s4 = new State(4, StateType.Nonfinite);
                State s5 = new State(5, StateType.Finite);

                MultiLineCommentsRecognizer = new TokenRecognizer(TokenType.Commentary, s1);

                MultiLineCommentsRecognizer.AddTransition(new Transition(s1, s2, CharTests.Is('/')));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s2, s3, CharTests.Is('*')));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s3, s3, CharTests.Is('*').Inverse()));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s3, s4, CharTests.Is('*')));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s4, s3, CharTests.Is('/').Or('*').Inverse()));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s4, s4, CharTests.Is('*')));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s4, s5, CharTests.Is('/')));

                // unclosed multiline comment
                MultiLineCommentsRecognizer.AddTransition(new Transition(s3, s5, CharTests.IsEndOfText));
                MultiLineCommentsRecognizer.AddTransition(new Transition(s4, s5, CharTests.IsEndOfText));
            }
            #endregion

            #region OperatorRecognizers
            OperatorRecognizers = TokenRecognizer.GenerateExactMatchRecognizers(TokenType.Operator, new[]
                    {
                        "+",
                        "-",
                        "*",
                        "/",
                        "=",
                        "==",
                        "!=",
                        ">",
                        "<",
                        "<=",
                        ">=",
                        "(",
                        ")",
                        "{",
                        "}",
                        ";",
                        ","
                    });
            #endregion

            #region Keywords
            Keywords = new List<string>(new[]
                                             {                                                 
                                                 "int",
                                                 "float",                                                 
                                                 "void",
                                                 "if",
                                                 "else",
                                                 "for"
                                             });
            #endregion

            #region Functions
            Functions = new List<string>(new[]
                                             {
                                                 "sin",
                                                 "cos",
                                                 "tan",
                                                 "exp",
                                                 "read",
                                                 "write",
                                                 "ln",
                                                 "abs"
                                             });
            #endregion
        }
    }
}
