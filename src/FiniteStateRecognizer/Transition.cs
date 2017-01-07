using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateRecognizer
{
    public class Transition
    {
        public State StartState { get; private set; }
        public State EndState { get; private set; }

        public CharTests.CharTest Condition { get; private set; }

        public Transition(State startState, State endState, CharTests.CharTest condition)
        {
            StartState = startState;

            EndState = endState;

            Condition = condition;                
        }
    }
}
