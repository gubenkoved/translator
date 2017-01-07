using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateRecognizer
{
    public static class CharTests
    {
        public delegate bool CharTest(char c);

        public static CharTest IsDelimeter = (c) => c == ' ' || c == '\t' || c == '\r' || c == '\n' || IsEndOfText(c);
        public static CharTest IsDigit = (c) => char.IsDigit(c);
        public static CharTest IsStringMarker = (c) => c == '\"';
        public static CharTest IsCharMarker = (c) => c == '\'';
        public static CharTest IsFloatNumberDelimiter = (c) => c == '.';
        public static CharTest IsLetter = (c) => char.ToLower(c) >= 'a' && char.ToLower(c) <= 'z';
        public static CharTest IsEndOfText = (c) => c == '\u0003';

        public static CharTest And(this CharTest test1, CharTest test2)
        {
            return (c) => test1(c) && test2(c);
        }

        public static CharTest And(this CharTest test1, char c)
        {
            return (ch) => test1(ch) && Is(c)(ch);
        }

        public static CharTest Or(this CharTest test1, CharTest test2)
        {
            return (c) => test1(c) || test2(c);
        }

        public static CharTest Or(this CharTest test1, char c)
        {
            return (ch) => test1(ch) || Is(c)(ch);
        }

        public static CharTest Not(this CharTest test)
        {
            return (c) => !test(c);
        }

        public static CharTest Inverse(this CharTest test)
        {
            return test.Not();
        }

        public static CharTest Is(char c)
        {
            return (ch) => ch == c;
        }
    }
}
