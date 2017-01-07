using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FiniteStateRecognizer
{
    public enum StateType
    {
        Finite,
        Nonfinite
    }

    public struct State
    {
        public StateType Type;
        public int Number;

        public State(int number, StateType type = StateType.Nonfinite)            
        {
            Number = number;

            Type = type;
        }

        public static bool operator ==(State s1, State s2)
        {
            return s1.Number == s2.Number;
        }
        public static bool operator !=(State s1, State s2)
        {
            return !(s1.Number == s2.Number);
        }

        public override string ToString()
        {
            return string.Format("({0}) {1}", Number, Type == StateType.Finite ? "finite" : "non-finite");
        }
    }
}
