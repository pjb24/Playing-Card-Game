using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine
{
    private IGameState currentState;
    public IGameState CurrentState => currentState;

    public void ChangeState(IGameState newState)
    {
        if (currentState != null)
        {
            Debug.Log($"Exiting state: {currentState.GetType().Name}");
            currentState.Exit();
        }

        currentState = newState;

        if (currentState != null)
        {
            Debug.Log($"Entering state: {currentState.GetType().Name}");
            currentState.Enter();
        }
    }

    public void Update()
    {
        currentState?.Update();
    }
}
