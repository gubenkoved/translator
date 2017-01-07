using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration
{
    public class IntermediateCodeHelper
    {
        private int _tempVariableCounter;
        private int _tempLabelCounter;

        public IntermediateCodeHelper()
        {

        }

        public Operand GetTempVariable()
        {
            ++_tempVariableCounter;

            return new Operand(string.Format("$T{0}", _tempVariableCounter));
        }

        public Operand GetTempLabel()
        {
            ++_tempLabelCounter;

            return new Operand(string.Format("$LABEL{0}", _tempLabelCounter));
        }
    }
}
