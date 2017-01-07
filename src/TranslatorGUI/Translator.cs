using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lexer.Core;
using Parser.Core;
using SemanticChecking;
using CodeGeneration;

namespace TranslatorGUI
{
    public class Translator
    {
        public ScanerBase Scaner { get; set; }

        public ParserBase Parser { get; set; }

        public SemanticCheckerBase SemanticChecker { get; set; }

        public GeneratorBase Generator { get; set; }
    }
}
