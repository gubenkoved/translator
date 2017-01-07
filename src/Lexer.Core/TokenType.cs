using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexer.Core
{
    public enum TokenType
    {
        [UserFrendlyName("char constant")]
        CharConstant,
        [UserFrendlyName("float constant")]
        FloatConstant,
        [UserFrendlyName("int constant")]
        IntegerConstant,
        [UserFrendlyName("string constant")]
        StringConstant,
        [UserFrendlyName("commentary")]
        Commentary,
        [UserFrendlyName("delimeter")]
        Delimiter,
        [UserFrendlyName("identifier")]
        Identifier,
        [UserFrendlyName("keyword")]
        Keyword,
        [UserFrendlyName("operator")]
        Operator,
        [UserFrendlyName("function")]
        Function,        
        Unknown,

        Epsilon,
        EndOfText
    }
}
