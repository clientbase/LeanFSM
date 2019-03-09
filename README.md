# LeanFSM
Two Finite State Machine implementations. One with explicit transitions, one transition-less. (Unity3D)

Transition-less Example:

```
using UnityEngine;
using LeanFSM;

public class Example : MonoBehaviour
{
    private FSM<ExampleStates> fsm;

    // Start is called before the first frame update
    void Awake()
    {
        fsm = new FSM<ExampleStates>();

        var foo = new FooState(fsm);
        var bar = new BarState(fsm);

        fsm.AddState(foo);
        fsm.AddState(bar);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fsm.CurrentState.Reason(this);
        fsm.CurrentState.Act(this);
    }
}

public enum ExampleStates
{
    NullState = 0,
    FooState,
    BarState
}

public class FooState : FSMState<ExampleStates>
{
    public FooState(FSM<ExampleStates> fsm) : base(fsm) { stateID = ExampleStates.FooState; }

    public override void Act(object actor)
    {
        Debug.Log("Foo is acting.");
    }

    public override void DoBeforeEntering()
    {
        Debug.Log("Enter Foo");
    }

    public override void DoBeforeLeaving()
    {
        Debug.Log("Exit Foo");
    }

    public override void Reason(object actor)
    {
        fsm.PerformTransition(ExampleStates.BarState);
    }
}

public class BarState : FSMState<ExampleStates>
{
    public BarState(FSM<ExampleStates> fsm) : base(fsm) { stateID = ExampleStates.BarState; }

    public override void Act(object actor)
    {
        Debug.Log("Bar is acting.");
    }

    public override void DoBeforeEntering()
    {
        Debug.Log("Enter Bar");
    }

    public override void DoBeforeLeaving()
    {
        Debug.Log("Exit Bar");
    }

    public override void Reason(object actor)
    {
        fsm.PerformTransition(ExampleStates.FooState);
    }
}
```
