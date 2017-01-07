using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;

namespace Parser.Core
{
    public class GeneralizedTerminal : Terminal
    {
        private TokenType _type;
        public override TokenType Type
        {
            get
            {
                return _type;
            }
        }

        public GeneralizedTerminal(TokenType type)
        {
            _type = type;
        }

        /// <summary>
        /// Returns new Epsilon generalized terminal
        /// </summary>
        static public GeneralizedTerminal Epsilon
        {
            get
            {
                return new GeneralizedTerminal(TokenType.Epsilon);
            }
        }
        static public GeneralizedTerminal EndOfText
        {
            get
            {
                return new GeneralizedTerminal(TokenType.EndOfText);
            }
        }

        public override string ToString()
        {
            var userFrendlyTypeName = GetUserFrendlyTypeName(_type);

            if (userFrendlyTypeName != null)
                return userFrendlyTypeName;

            return string.Format("{0}", _type);
        }

        public override Symbol CreateCopy()
        {
            return new GeneralizedTerminal(_type);
        }

        internal static readonly LambdaEqualityComparer<GeneralizedTerminal> ByTypeEqualityComprarer
            = new LambdaEqualityComparer<GeneralizedTerminal>(
                (gt1, gt2) => gt1.Type == gt2.Type,
                (gt) => gt.Type.GetHashCode());
    }
}
