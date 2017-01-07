using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration
{
    public class Instruction
    {
        public OpCode OpCode { get; private set; }

        public Operand Arg1 { get; private set; }
        public Operand Arg2 { get; private set; }
        public Operand Result { get; private set; }

        public Instruction(OpCode operation, Operand arg1, Operand arg2, Operand result)
        {
            OpCode = operation;

            Arg1 = arg1;
            Arg2 = arg2;
            Result = result;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", OpCode, Arg1, Arg2, Result);
        }
    }
}
