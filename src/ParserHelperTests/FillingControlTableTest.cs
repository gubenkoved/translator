using FormalParser;
using FormalParser.MyGrammar;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParserHelperTests
{
    [TestClass]
    public class FillingControlTableTest
    {
        [TestMethod]
        public void FillingCTTest1()
        {
            var productions = MyLanguageGrammar.Productions;
            var controlTable = new ControlTable();

            controlTable.FillByProcessedProductions(productions, MyNonterminals.EXPRESSION);

            Assert.IsTrue(controlTable.ElementsCount == 122);
        }
    }
}
