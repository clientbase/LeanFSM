using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
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
                //Debug.Log("Checking state " + state.ID.ToString() + " against " + s.ID.ToString() + "\n");
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

            // Check if the currentState has the transition passed as argument
            S id = currentState.GetOutputState(trans);
            if (EqualityComparer<S>.Default.Equals(id, (S)typeof(S).GetEnumValues().GetValue(0)))
            {
                Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " +
                               " for transition " + trans.ToString());
                return;
            }

            // Update the currentStateID and currentState		
            currentStateID = id;
            foreach (FSMState<S> state in states)
            {
                if (EqualityComparer<S>.Default.Equals(state.ID, currentStateID))
                {
                    // Do the post processing of the state before setting the new one
                    currentState.DoBeforeLeaving();

                    currentState = state;

                    // Reset the state to its desired condition before it can reason or act
                    currentState.DoBeforeEntering();
                    break;
                }
            }

        }
    }

    public abstract class FSMState<S>
        where S : struct, IConvertible, IComparable
    {
        protected FSM<S> fsm;
        protected S nullState = (S)typeof(S).GetEnumValues().GetValue(0);
        protected S nullTransistion = (S)typeof(S).GetEnumValues().GetValue(0);
        protected List<S> transitions = new List<S>();
        protected S stateID;
        public S ID { get { return stateID; } }

        public FSMState(FSM<S> fsm)
        {
            this.fsm = fsm;
        }

        public abstract void Act(object actor);

        public void AddTransition(S trans)
        {
            if (!typeof(S).IsEnum)
            {
                throw new ArgumentException("T must be an enumeration");
            }

            // Check if the current transition is already mapped
            if (transitions.Contains(trans))
            {
                Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
                               "Impossible to assign to another state");
                return;
            }

            transitions.Add(trans);
        }

        public void DeleteTransition(S trans)
        {
            // Check for NullTransition
            if (EqualityComparer<S>.Default.Equals(trans, nullTransistion))
            {
                Debug.LogError("FSMState ERROR: NullTransition is not allowed");
                return;
            }

            // Validate
            if (transitions.Contains(trans))
            {
                transitions.Remove(trans);
                return;
            }
            Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
                           " was not on the state's transition list");
        }

        public abstract void DoBeforeEntering();
        public abstract void DoBeforeLeaving();

        public S GetOutputState(S trans)
        {
            // Check if the map has this transition
            if (transitions.Contains(trans))
            {
                return trans;
            }
            return (S)typeof(S).GetEnumValues().GetValue(0);
        }

        public abstract void Reason(object actor);
    }
}