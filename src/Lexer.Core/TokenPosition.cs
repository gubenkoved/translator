using System;
namespace Lexer.Core
{
    public struct TokenPosition
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public int Offset { get; set; }

        public TokenPosition(int offset, int line, int column)
            : this()
        {
            Offset = offset;

            Line = line;
            Column = column;
        }

        public static TokenPosition GetTokenPosition(string text, int startIndex)
        {
            int line = 1;

            int lastLineStart = 0;

            for (int i = 0; i < startIndex; i++)
            {
                if (text[i] == '\n')
                {
                    ++line;
                    lastLineStart = i;
                }
            }

            int collumn = Math.Max(0, startIndex - lastLineStart);

            return new TokenPosition(startIndex, line, collumn);
        }
    }
}