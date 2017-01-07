using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration
{
    //public enum OperandKind
    //{
    //    Label,
    //    Variable,
    //    Constant
    //}

    public class Operand
    {
        //public OperandKind Kind { get; private set; }
        public object Value { get; private set; }

        public Operand(/*OperandKind kind,*/ object value)
        {
            //Kind = kind;

            Value = value;
        }

        public override string ToString()
        {
            //if (Value == null)
            //    return "{null}";

            return Value.ToString();
        }
    }
}
