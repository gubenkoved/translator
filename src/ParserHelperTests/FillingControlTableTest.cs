using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormalParser;
using Parser.Core;

namespace ParserHelperTests
{
    [TestClass]
    public class FillingControlTableTest
    {
        [TestMethod]
        public void FillingCTTest1()
        {
            var productions = MyLanguageGrammar.ProcessedProductions;
            var controlTable = new ControlTable();

            controlTable.FillByProcessedProductions(productions, FormalNonterminals.EXPRESSION);

            Assert.IsTrue(controlTable.ElementsCount == 24);
        }
    }
}
