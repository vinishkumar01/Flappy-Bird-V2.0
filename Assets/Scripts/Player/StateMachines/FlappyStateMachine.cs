using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyStateMachine
{
    public FlappyState _currentState { get; set; }


    public void Initialize(FlappyState startingState)
    {
        _currentState = startingState;
        _currentState.EnterState();
    }

    public void ChangeState(FlappyState newState)
    {
        _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }
}
