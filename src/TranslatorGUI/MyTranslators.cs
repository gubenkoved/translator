using FiniteStateRecognizer;
using FormalParser.MyGrammar;
using Lexer.Core;
using RecursiveDescentParser;

namespace TranslatorGUI
{
    public static class MyTranslators
    {
        public static Translator InformalTranslator { get; private set; }
        public static Translator FormalTranslator { get; private set; }

        static MyTranslators()
        {
            InformalTranslator = new Translator()
            {
                Scaner = new SimpleScaner(),
                Parser = new MyRecursiveDescentParser(),
                SemanticChecker = new ASTValidator(),
                Generator = new ASTCodegenerator()
            };

            FormalTranslator = new Translator()
            {
                Scaner = MyLanguageFiniteStateScaner.Instance,
                Parser = MyLL1Parser.Instance,
                SemanticChecker = new MySemanticChecker(),
                Generator = new MyGenerator()
            };
        }
    }
}
