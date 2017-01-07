using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration
{
    public class IntermediateCode
    {
        private List<Instruction> _instructions;
        public IEnumerable<Instruction> Instructions
        {
            get
            {
                return _instructions;
            }
        }

        public IntermediateCodeHelper Helper { get; private set; }

        public IntermediateCode()
        {
            _instructions = new List<Instruction>();

            Helper = new IntermediateCodeHelper();
        }

        public void Emit(Instruction instruction)
        {
            _instructions.Add(instruction);
        }
    }
}
