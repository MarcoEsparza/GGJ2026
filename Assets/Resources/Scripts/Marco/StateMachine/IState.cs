using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public StateMachine StateController { get; set; }
    public GameObject Owner { get; set; }

    //Method executed when a state has been entered.
    void OnStateEnter();

    //Method executed every frame for the current state.
    void OnExecuteState();

    //Method executed at the end of a state.
    void OnExitState();

    //Method executed to check if the current state should change state.
    void CheckStateConditions();
}
