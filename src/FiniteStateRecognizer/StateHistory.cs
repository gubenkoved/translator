using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateRecognizer
{
    public struct StateSet
    {
        public TokenRecognizer Recognizer;
        public HashSet<State> States;

        public StateSet(TokenRecognizer recognizer, IEnumerable<State> states)
        {
            Recognizer = recognizer;

            States = new HashSet<State>();

            foreach (var state in states)
            {
                States.Add(state);
            }
        }

        public bool HasFiniteState()
        {
            foreach (var state in States)
            {
                if (state.Type == StateType.Finite)
                    return true;
            }

            return false;
        }
    }

    public class StateHistory
    {
        private List<List<StateSet>> _history;
        public int Count
        {
            get
            {
                return _history.Count;
            }
        }

        public StateHistory()
        {
            _history = new List<List<StateSet>>();
        }

        public void AddStep(IEnumerable<StateSet> states)
        {
            _history.Add(states.ToList());
        }

        public void Clear()
        {
            _history.Clear();
        }

        public IEnumerable<StateSet> this[int i]
        {
            get
            {
                return _history[i].AsEnumerable();
            }
        }
    }
}
