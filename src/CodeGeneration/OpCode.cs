using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration
{
    public enum OpCode
    {
        ADD, // addition
        SUB, // substraction
        MULT, // multiplication
        DIV, // division
        CALL, // call function
        PARAM, // add param to function will called
        JMP,    // unconditional jump
        JEQ, // jump if the first operand is equal to second operand
        JNEQ, // jump if the first operand is not equal to second operand
        JLT, // jump if the first operand is less than the second operand
        JGT, // jump if the first operand is greater than the second operand
        JLE, // jump if the first operand is less than or equal to the second operand
        JGE, // jump if the first operand is greater than or equal to than second operand
        ASGN, // assignment
        LTRGT, // label target
        
        Undefinded
    }
}
