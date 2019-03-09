using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeanFSM
{
    public class FSM<S>
    where S : struct, IConvertible, IComparable
    {
        List<FSMState<S>> states = new List<FSMState<S>>();

        private S currentStateID;
        public S CurrentStateID { get { return currentStateID; } }
        private FSMState<S> currentState;
        public FSMState<S> CurrentState { get { return currentState; } }

        public FSM()
        {
        }

        public void AddState(FSMState<S> s)
        {
            // Check for Null reference before deleting
            if (s == null)
            {
                Debug.LogError("FSM ERROR: Null reference is not allowed");
            }

            // First state inserted is also the Initial State
            if (states.Count == 0)
            {
                states.Add(s);
                currentState = s;
                currentStateID = s.ID;
                Debug.Log("FSM: Adding state " + s.ID.ToString() + ".");
                return;
            }

            // Add the state to the list, if it's not already there
            foreach (FSMState<S> state in states)
            {
                if (state.ID.CompareTo(s.ID) == 0)
                {
                    Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() +
                                   " because state has already been added");
                    return;
                }
            }
            Debug.Log("FSM: Adding state " + s.ID.ToString() + ".");
            states.Add(s);
        }

        public void DeleteState(S id)
        {
            // Check for NullState before deleting
            if (EqualityComparer<S>.Default.Equals(id, (S)typeof(S).GetEnumValues().GetValue(0)))
            {
                Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
                return;
            }

            // Search the List and delete the state if present
            foreach (FSMState<S> state in states)
            {
                if (EqualityComparer<S>.Default.Equals(state.ID, id))
                {
                    states.Remove(state);
                    return;
                }
            }
            Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                           ". It was not on the list of states");
        }

        public void PerformTransition(S trans)
        {
            // Check for NullTransition before changing the current state
            if (EqualityComparer<S>.Default.Equals(trans, (S)typeof(S).GetEnumValues().GetValue(0)))
            {
                Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
                return;
            }


            // Check if we found a state
            var foundState = false;            
            foreach (FSMState<S> state in states)
            {
                if (EqualityComparer<S>.Default.Equals(state.ID, trans))
                {
                    // Update the currentStateID and currentState
                    currentStateID = trans;

                    // Do the post processing of the state before setting the new one
                    currentState.DoBeforeLeaving();

                    currentState = state;

                    // Reset the state to its desired condition before it can reason or act
                    currentState.DoBeforeEntering();

                    // We found a state
                    foundState = true;
                    break;
                }
            }

            if(!foundState)
                Debug.LogError("FSM ERROR: State " + trans.ToString() + " has not been added to fsm type: " + trans.GetType().ToString());

        }
    }

    public abstract class FSMState<S>
        where S : struct, IConvertible, IComparable
    {
        protected FSM<S> fsm;
        protected S nullState = (S)typeof(S).GetEnumValues().GetValue(0);
        protected S stateID;
        public S ID { get { return stateID; } }

        public FSMState(FSM<S> fsm)
        {
            this.fsm = fsm;
        }

        public abstract void Act(object actor);

        public abstract void DoBeforeEntering();
        public abstract void DoBeforeLeaving();

        public abstract void Reason(object actor);
    }
}
